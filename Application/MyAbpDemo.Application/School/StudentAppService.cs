using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyAbpDemo.ApplicationDto;
using MyAbpDemo.Core;
using MyAbpDemo.Infrastructure;

namespace MyAbpDemo.Application
{
    public class StudentAppService:AppServiceBase,IStudentAppService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly IStudentDomainService _studentDomainService;
        public StudentAppService(IStudentRepository studentRepository, ITeacherRepository teacherRepository, IStudentDomainService studentDomainService)
        {
            _studentRepository = studentRepository;
            _teacherRepository = teacherRepository;
            _studentDomainService = studentDomainService;
        }

        public async Task<Result<List<GetStudentListOutput>>> GetStudentList()
        {
            var result =await _studentRepository.GetAll().ProjectTo<GetStudentListOutput>().ToListAsync();
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
        public async Task<Result> Import(IFormFile uploadedFile)
        {
            Result result = new Result(ResultCode.Ok);

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

            StringBuilder stringBuilder = new StringBuilder();
            List<ImportStudent> list = EpplusHelper.Import<ImportStudent>(uploadedFile, ref stringBuilder);

            if (stringBuilder.Length == 0)
            {
                //校验老师编码
                bool CheckTeacherId(int teacherId) => _teacherRepository.GetAll().Select(a => a.Id).Contains(teacherId);
                //校验学生等级
                bool CkeckLearnLevel(string learnLevel) => new LearnLevel().GetEnumInfoList().Select(a => a.DisplayName)
                    .Contains(learnLevel);
                //导入数据检验
                stringBuilder = new StudentValidator(CkeckLearnLevel,CheckTeacherId).GetObjectValidatorError(list);
            }

            if (stringBuilder.Length > 0)
            {
                var error = stringBuilder.ToString();
                result.Message = error;
                result.Code = ResultCode.ParameterFailed;
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
    }
}
