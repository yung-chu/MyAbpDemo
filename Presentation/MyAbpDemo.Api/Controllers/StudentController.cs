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
        [HttpGet("students")]
        [ProducesResponseType(typeof(List<GetStudentListOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> Index()
        {
            var result = await _studentAppService.GetStudentList();
            if (result.IsSuccess)
            {
                return Ok(result);
            }
            return BadRequest(new Result(result.Code, result.Message));
        }

        /// <summary>
        /// 创建学生
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost("students")]
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
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string path = $"TempExport\\学生-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            try
            {
                FileInfo fileInfo = new FileInfo(Path.Combine(sWebRootFolder, path));
                if (!Directory.Exists(fileInfo.DirectoryName))
                {
                    Directory.CreateDirectory(path);
                }
                var list = await _studentAppService.GetExportStudentList();
                EpplusHelper.Export(list, fileInfo);
                new FileExtensionContentTypeProvider().TryGetContentType(path, out string contentType);
                return File(path, contentType, Path.GetFileName(path));
            }
            catch (Exception e)
            {
                return BadRequest("导出失败");
            }
        }


        /// <summary>
        /// excel导入
        /// </summary>
        /// <param name="uploadedFile">文件对象</param>
        /// <returns></returns>
        /// <response code="200">成功</response>
        /// <response code="400">失败返回错误列表</response>  
        [HttpPost]
        [Route("import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(List<ValidatorErrorInfo>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Import(IFormFile uploadedFile)
        {
            var result =await _studentAppService.Import(uploadedFile);
            if (result.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(result.BaseResult());
        }
    }
}