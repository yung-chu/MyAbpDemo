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
        /// getSingle
        /// </summary>
        /// <returns></returns>
        Task<Result<GetStudentListOutput>> GetSingleStudent(int id);

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
        ///  excel分组导入返回合并单元格信息
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        Result<Dictionary<List<ImportGroupStudent>, List<CellPosition>>> GroupImport(IFormFile uploadedFile);

        /// <summary>
        /// excel导入后返回导出错误excel
        /// </summary>
        /// <param name="uploadedFile"></param>
        /// <returns></returns>
        Result<List<ExportWithError>> GetExportWithValidateError(IFormFile uploadedFile);
    }
}
