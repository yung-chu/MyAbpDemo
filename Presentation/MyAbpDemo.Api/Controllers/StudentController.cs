using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using MyAbpDemo.Application;
using MyAbpDemo.ApplicationDto;
using MyAbpDemo.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.ExceptionHandling;
using Abp.Auditing;
using Castle.LoggingFacility.MsLogging;

namespace MyAbpDemo.Api.Controllers
{
    public class StudentController : ApiControllerBase
    {
        private readonly IStudentAppService _studentAppService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public StudentController(IStudentAppService studentAppService, IHostingEnvironment hostingEnvironment)
        {
             _studentAppService = studentAppService;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 获取学生信息
        /// </summary>
        /// <returns></returns>
        /// <response code="200">成功</response>
        /// <response code="400">失败返回Result对象</response>  
        [HttpGet("student")]
        [ProducesResponseType(typeof(List<GetStudentListOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> Index()
        {
            var result = await _studentAppService.GetStudentList();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.BaseResult());
        }


        /// <summary>
        /// 创建学生
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("student")]
        [ProducesResponseType(typeof(List<GetStudentListOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody]CreateStudentInput input)
        {
            var result = await _studentAppService.CreateStudent(input);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 获取某个学生
        /// 使用redis缓存
        /// </summary>
        /// <param name="id">编号</param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(List<GetStudentListOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetStudentById(int id)
        {
             var result = await _studentAppService.GetSingleStudent(id);
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(result.BaseResult());
        }

        #region 导入、导出
        /// <summary>
        /// excel导出
        /// </summary>
        /// <returns></returns>
        /// <response code="200">成功</response>
        /// <response code="400">失败返回Result对象</response>  
        [HttpGet("export")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Export()
        {
            string path = $"TempExport\\学生-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var list = await _studentAppService.GetExportStudentList();
            return CommomExport(path, list, new List<CellPosition>());
        }


        /// <summary>
        /// excel导入
        /// </summary>
        /// <param name="uploadedFile">文件对象</param>
        /// <returns></returns>
        /// <response code="200">成功</response>
        /// <response code="400">失败返回错误列表</response>  
        [HttpPost("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ValidatorErrorInfo>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Import(IFormFile uploadedFile)
        {
            var result = await _studentAppService.Import(uploadedFile);
            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// test excel分组导入 数量分组.xlsx
        /// </summary>
        /// <param name="uploadedFile">文件对象</param>
        /// <returns></returns>
        /// <response code="200">成功</response>
        /// <response code="400">失败返回错误列表</response>  
        [HttpPost("groupImport")]
        [ProducesResponseType(typeof(List<List<ImportGroupStudent>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public IActionResult GroupImport(IFormFile uploadedFile)
        {
            var result = _studentAppService.GroupImport(uploadedFile);
            if (result.IsSuccess)
            {
                return Ok(result.Data.Keys.First());
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// excel导出校验错误信息
        /// 错误信息行标红
        /// </summary>
        /// <returns></returns>
        /// <response code="200">成功</response>
        /// <response code="400">失败返回Result对象</response>  
        [HttpPost("exportWithError")]
        [ProducesResponseType(typeof(List<ExportWithError>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public IActionResult ExportWithError(IFormFile uploadedFile) //这里是表单提交用httpPost
        {
            string path = $"TempExport\\学生验证错误-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var result = _studentAppService.GetExportWithValidateError(uploadedFile);
            if (result.IsSuccess)
            {
                return CommomExport(path, result.Data, new List<CellPosition>());
            }

            return BadRequest(result.BaseResult());

        }

        /// <summary>
        /// 导出合并单元格excel
        /// </summary>
        /// <returns></returns>
        /// <response code="200">成功</response>
        /// <response code="400">失败返回Result对象</response>  
        [HttpPost("exportMerge")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public IActionResult ExportMerge(IFormFile uploadedFile) //这里是表单提交用httpPost
        {
            string path = $"TempExport\\学生验证错误-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            var result = _studentAppService.GroupImport(uploadedFile);

            if (result.IsSuccess)
            {
                var dictionary = result.Data;
                return CommomExport(path, dictionary.Keys.First(), dictionary.Values.First());
            }

            return BadRequest(result.BaseResult());
        }

        /// <summary>
        /// 导出公共方法
        /// </summary>
        /// <param name="path"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        private IActionResult CommomExport<T>(string path, IEnumerable<T> list, List<CellPosition> cellPositions) where T : new()
        {
            try
            {
                string sWebRootFolder = _hostingEnvironment.WebRootPath;
                FileInfo fileInfo = new FileInfo(Path.Combine(sWebRootFolder, path));
                if (!Directory.Exists(fileInfo.DirectoryName))
                {
                    Directory.CreateDirectory(fileInfo.DirectoryName);
                }

                EpplusHelper.Export(list, fileInfo, cellPositions);
                new FileExtensionContentTypeProvider().TryGetContentType(path, out string contentType);
                return File(path, contentType, Path.GetFileName(path));
            }
            catch (Exception e)
            {
                return BadRequest("导出失败");
            }
        }
        #endregion


    }
}