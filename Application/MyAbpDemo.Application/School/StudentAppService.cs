using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Auditing;
using Abp.AutoMapper;
using Abp.BackgroundJobs;
using Abp.Collections.Extensions;
using Abp.Runtime.Caching;
using Abp.UI;
using AutoMapper.QueryableExtensions;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyAbpDemo.ApplicationDto;
using MyAbpDemo.Core;
using MyAbpDemo.Infrastructure;
using OfficeOpenXml;

namespace MyAbpDemo.Application
{
    public class StudentAppService : AppServiceBase, IStudentAppService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentDomainService _studentDomainService;
        private readonly ICacheManager _cacheManager;
        private readonly IBackgroundJobManager _backgroundJobManager;

        public StudentAppService(IStudentRepository studentRepository, ITeacherRepository teacherRepository,
            IStudentDomainService studentDomainService, ICacheManager cacheManager, IBackgroundJobManager backgroundJobManager)
        {
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _studentDomainService = studentDomainService;
            _cacheManager = cacheManager;
            _backgroundJobManager = backgroundJobManager;
        }

        public async Task<Result<List<GetStudentListOutput>>> GetStudentListAsync()
        {
            var result = await _studentRepository.GetAll().ProjectTo<GetStudentListOutput>().ToListAsync();
            //测试后台任务作业
            var args = new ApiDataSyncJobArgs
            {
                TargetParkId = 99
            };
            await _backgroundJobManager.EnqueueAsync<ApiDataSyncJob, ApiDataSyncJobArgs>(args);
            //Logger.Info("测试后台任务作业"+DateTime.Now.ToLocalTime());
            return Result.FromData(result);
        }

        public async Task<Result<GetStudentListOutput>> GetSingleStudentAsync(int id)
        {
            var student = new GetStudentListOutput();
            var result = Result.FromData(student);

            if (_cacheManager.GetStudent(id.ToString())!=null)
            {
                student = _cacheManager.GetStudent(id.ToString());
            }
            else //数据取
            {
                if (_studentRepository.GetAll().Any(a => a.Id == id))
                {
                    student = (await _studentRepository.GetAll().FirstAsync(a => a.Id == id)).MapTo<GetStudentListOutput>();
                }

                _cacheManager.SetStudent(id.ToString(), student);
            }

            result.Data = student;
            return result;
        }

        /// <summary>
        /// 导出
        /// </summary>
        /// <returns></returns>
        public async Task<List<ExportStudent>> GetExportStudentListAsync()
        {
            var result = await GetStudentListAsync();
            return result.Data.MapToList<ExportStudent>();
        }

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> CreateStudentAsync(CreateStudentInput input)
        {
            var student = input.MapTo<Student>();
            await _studentDomainService.CreateStudent(student);
            return Result.Ok();
        }


        #region 导入、导出
        /// <summary>
        ///  excel导入
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        public async Task<Result> ImportAsync(IFormFile uploadedFile)
        {
            var result = new Result(ResultCode.Ok);

            if (uploadedFile == null)
            {
                result.Message = "请选择上传excel";
                result.Code = ResultCode.ParameterFailed;
                return result;
            }

            if (!uploadedFile.FileName.EndsWith("xlsx"))
            {
                result.Message = "请选择上传后缀xlsx格式的excel";
                result.Code = ResultCode.ParameterFailed;
                return result;
            }

            //逻辑验证
            var list = new List<ImportStudent>();
            var validatorErrorInfos = GetValidatorErrorInfo(uploadedFile, list);
            if (validatorErrorInfos.Any())
            {
                result.Code = ResultCode.ParameterFailed;
                result.Message = validatorErrorInfos.GetValidatorErrorStr();
                return result;
            }

            //去重
            list = list.Distinct().ToList();
            //保存
            var mapToList = list.MapToList<Student>();
            foreach (var item in mapToList)
            {
                if (!_studentRepository.GetAll().Select(a => a.Name).Contains(item.Name))
                {
                    await _studentDomainService.CreateStudent(item);
                }
            }

            return result;
        }

        /// <summary>
        /// 返回导出错误excel(增加错误列)
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        public Result<List<ExportWithError>> GetExportWithValidateError(IFormFile uploadedFile)
        {
            var getData = new List<ExportWithError>();
            var result = Result.FromData(getData);

            if (uploadedFile == null)
            {
                result.Message = "请选择上传excel";
                result.Code = ResultCode.ParameterFailed;
                return result;
            }
            if (!uploadedFile.FileName.EndsWith("xlsx"))
            {
                result.Message = "请选择上传后缀xlsx格式的excel";
                result.Code = ResultCode.ParameterFailed;
                return result;
            }

            //逻辑验证
            var importList = new List<ImportStudent>();
            var validatorErrorInfos = GetValidatorErrorInfo(uploadedFile, importList);
            if (validatorErrorInfos.Count==1&& string.IsNullOrEmpty(validatorErrorInfos.First().Row))//excel模板、数据格式错误
            {
                result.Code = ResultCode.ParameterFailed;
                result.Message = validatorErrorInfos.GetValidatorErrorStr();
                return result;
            }
       
            getData = importList.MapToList<ExportWithError>();
            foreach (var item in getData)
            {
                string row = (getData.IndexOf(item) + 1).ToString();
                if (validatorErrorInfos.Any(a=>a.Row== row))
                {
                    item.ErrorMessage = validatorErrorInfos.First(a => a.Row == row).ErrorDetails
                        .Select(a => a.ErrorMsg).JoinAsString(",");
                }
            }
         
            result.Data = getData;
            return result;
        }

        /// <summary>
        /// 行分组导入测试
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        public Result<Dictionary<List<ImportGroupStudent>, List<CellPosition>>> GroupImport(IFormFile uploadedFile)
        {
            var dictionary=new Dictionary<List<ImportGroupStudent>, List<CellPosition>>();
            var result = Result.FromData(dictionary);
            var validatorErrorInfos = new List<ValidatorErrorInfo>();
            var list = EpplusHelper.Import<ImportGroupStudent>(uploadedFile, validatorErrorInfos);

            if (validatorErrorInfos.Any())
            {
                result.Code = ResultCode.ParameterFailed;
                result.Message = validatorErrorInfos.GetValidatorErrorStr();
                return result;
            }

            //方法A 获取分组多个对象
            var newResult = ListGroupByCount(list);
            //根据对象数量返回列起始跨度
            var getCellPositions = GetCellPositions(newResult);

            dictionary.Add(list, getCellPositions);
            result.Data = dictionary;
            return result;
        }

        /// <summary>
        /// 获取逻辑验证错误信息
        /// </summary>
        private List<ValidatorErrorInfo> GetValidatorErrorInfo(IFormFile uploadedFile, List<ImportStudent> list)
        {
            var validatorErrorInfos = new List<ValidatorErrorInfo>();
            list.AddRange(EpplusHelper.Import<ImportStudent>(uploadedFile, validatorErrorInfos));

            //逻辑校验
            if (!validatorErrorInfos.Any()|| validatorErrorInfos.Select(a=>a.ErrorDetails).Count()>1)
            {
                //校验老师编码
                bool CheckTeacherId(int teacherId) =>teacherId==0 || _teacherRepository.GetAll().Select(a => a.Id).Contains(teacherId);

                //校验学生等级(系统已经校验不为空)
                bool CkeckLearnLevel(string learnLevel) => string.IsNullOrEmpty(learnLevel) ||new LearnLevel().GetEnumInfoList().Select(a => a.DisplayName)
                    .Contains(learnLevel);

                //导入数据检验
                validatorErrorInfos = new StudentValidator(CkeckLearnLevel, CheckTeacherId).GetObjectValidatorError(list);
            }

            return validatorErrorInfos;
        }

        /// <summary>
        /// 分组导入方案A
        /// 按组存一个对象
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<List<ImportGroupStudent>> ListGroupByCount(List<ImportGroupStudent> list) 
        {
            List<ImportGroupStudent> getImports = new List<ImportGroupStudent>();
            var getImportList = new List<List<ImportGroupStudent>>();

            foreach (var item in list)
            {
                int currentIndex = list.IndexOf(item);
                if (currentIndex == 0)
                {
                    getImports.Add(item);
                }

                if (currentIndex != 0)
                {
                    if (item.Count==0)
                    {
                        getImports.Add(item);
                        if (currentIndex == list.Count - 1)
                        {
                            getImportList.Add(getImports);
                        }
                    }
                    else
                    {
                        getImportList.Add(getImports);
                        getImports = new List<ImportGroupStudent> { item };
                        if (currentIndex == list.Count - 1)
                        {
                            getImportList.Add(getImports);
                        }
                    }
                }
            }

            return getImportList;
        }

        /// <summary>
        /// 分组导入方案B
        /// https://stackoverflow.com/questions/36978100/how-to-use-epplus-with-cells-containing-few-rows
        /// </summary>
        /// <returns></returns>
        static List<ImportGroupStudent> ImportRecords(IFormFile uploadedFile)
        {
            var ret = new List<ImportGroupStudent>();
            using (var excel = new ExcelPackage(uploadedFile.OpenReadStream()))
            {
                var wks = excel.Workbook.Worksheets["Sheet1"];
                var lastRow = wks.Dimension.End.Row;

                for (int i = 2; i <= lastRow; i++)
                {
                    var importedRecord = new ImportGroupStudent
                    {
                        Count = Convert.ToInt32(GetCellValueFromPossiblyMergedCell(wks, i, 1)),
                        BarCode = wks.Cells[i, 2].Value.ToString(),
                        Price = Convert.ToDecimal(wks.Cells[i, 3].Value.ToString())
                    };
                    ret.Add(importedRecord);
                }
            }

            return ret;
        }

        static string GetCellValueFromPossiblyMergedCell(ExcelWorksheet wks, int row, int col)
        {
            var cell = wks.Cells[row, col];
            if (cell.Merge)
            {
                var mergedId = wks.MergedCells[row, col];
                return wks.Cells[mergedId].First().Value.ToString();
            }
            else
            {
                return cell.Value.ToString();
            }
        }


        //根据对象数量返回列起始跨度
        private List<CellPosition> GetCellPositions(List<List<ImportGroupStudent>> list)
        {
            var cellPositions = new List<CellPosition>();
            var rowObjectCount = new List<int>();
            list.ForEach(a =>
            {
                rowObjectCount.Add(a.Count);
            });

            int fromRow = 0, toRow = 0;
            foreach (var item in rowObjectCount)
            {
                int index = rowObjectCount.IndexOf(item);
                if (index == 0)
                {
                    fromRow = 2;
                    toRow = fromRow + item - 1;
                }
                else
                {
                    fromRow = toRow + 1;
                    toRow = fromRow + item - 1;
                }

                cellPositions.Add(new CellPosition
                {
                    FromRow = fromRow,
                    FromCol = 1,
                    ToRow = toRow,
                    ToCol = 1
                });
            }

            return cellPositions;
        }

        #endregion
    }
}
