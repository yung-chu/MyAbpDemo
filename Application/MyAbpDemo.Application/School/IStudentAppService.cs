using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MyAbpDemo.ApplicationDto;
using MyAbpDemo.Infrastructure;

namespace MyAbpDemo.Application
{
    public interface IStudentAppService
    {
        
        /// <summary>
        /// getList
        /// </summary>
        /// <returns></returns>
        Task<Result<List<GetStudentListOutput>>> GetStudentList();

        /// <summary>
        /// createStudent
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<Result> CreateStudent(CreateStudentInput input);

        /// <summary>
        /// excel导出
        /// </summary>
        /// <returns></returns>
        Task<List<ExportStudent>> GetExportStudentList();

        /// <summary>
        /// excel导入
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        Task<Result> Import(IFormFile uploadedFile);

        /// <summary>
        /// excel合并单元格分组导入
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        Result GroupImport(IFormFile uploadedFile);
    }
}
