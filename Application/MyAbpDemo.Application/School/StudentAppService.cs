using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using Abp.Runtime.Caching;
using Abp.UI;
using AutoMapper.QueryableExtensions;
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

       public StudentAppService(IStudentRepository studentRepository, ITeacherRepository teacherRepository,
            IStudentDomainService studentDomainService)
        {
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _studentDomainService = studentDomainService;
        }

        public async Task<Result<List<GetStudentListOutput>>> GetStudentList()
        {
            var result = await _studentRepository.GetAll().ProjectTo<GetStudentListOutput>().ToListAsync();
            return Result.FromData(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<List<ExportStudent>> GetExportStudentList()
        {
            var result = (await GetStudentList());
            return result.MapToList<ExportStudent>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<Result> CreateStudent(CreateStudentInput input)
        {
            var student = input.MapTo<Student>();
            await _studentDomainService.CreateStudent(student);
            return Result.Ok();
        }

        /// <summary>
        ///  excel导入
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        public async Task<Result<List<ValidatorErrorInfo>>> Import(IFormFile uploadedFile)
        {
            var result = Result.Fail<List<ValidatorErrorInfo>>(ResultCode.Fail);

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

            var validatorErrorInfos = new List<ValidatorErrorInfo>();
            List<ImportStudent> list = EpplusHelper.Import<ImportStudent>(uploadedFile, validatorErrorInfos);

            //逻辑校验
            if (!validatorErrorInfos.Any())
            {
                //校验老师编码
                bool CheckTeacherId(int teacherId) => _teacherRepository.GetAll().Select(a => a.Id).Contains(teacherId);

                //校验学生等级
                bool CkeckLearnLevel(string learnLevel) => new LearnLevel().GetEnumInfoList().Select(a => a.DisplayName)
                    .Contains(learnLevel);

                //导入数据检验
                validatorErrorInfos = new StudentValidator(CkeckLearnLevel, CheckTeacherId).GetObjectValidatorError(list);
            }

            if (validatorErrorInfos.Any())
            {
                result.Code = ResultCode.ParameterFailed;
                result.Data = validatorErrorInfos;
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
        /// 测试分组导入
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        public  Result GroupImport(IFormFile uploadedFile)
        {
            return Result.Ok();
        }

        /// <summary>
        /// 方案A
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private static List<List<GroupImport>> ListGroupByCount(List<GroupImport> list)
        {
            List<GroupImport> getImports = new List<GroupImport>();
            var getImportList = new List<List<GroupImport>>();

            foreach (var item in list)
            {
                int currentIndex = list.IndexOf(item);
                if (currentIndex == 0)
                {
                    getImports.Add(item);
                }

                if (currentIndex != 0)
                {
                    if (item.Count == 0)
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
                        getImports = new List<GroupImport> { item };
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
        /// 方案B
        /// https://stackoverflow.com/questions/36978100/how-to-use-epplus-with-cells-containing-few-rows
        /// </summary>
        /// <returns></returns>
        static List<GroupImport> ImportRecords(IFormFile uploadedFile)
        {
            var ret = new List<GroupImport>();
            using (var excel = new ExcelPackage(uploadedFile.OpenReadStream()))
            {
                var wks = excel.Workbook.Worksheets["Sheet1"];
                var lastRow = wks.Dimension.End.Row;

                for (int i = 2; i <= lastRow; i++)
                {
                    var importedRecord = new GroupImport
                    {
                        Count =Convert.ToInt32(GetCellValueFromPossiblyMergedCell(wks, i, 1)) ,
                        BarCode = wks.Cells[i, 2].Value.ToString(),
                        Price = Convert.ToDecimal( wks.Cells[i, 3].Value.ToString())
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

    }
}
