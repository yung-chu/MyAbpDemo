using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using MyAbpDemo.Application;
using MyAbpDemo.ApplicationDto;
using MyAbpDemo.Infrastructure;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.ExceptionHandling;
using Abp.Auditing;
using Castle.LoggingFacility.MsLogging;
using EasyNetQ;
using Microsoft.Net.Http.Headers;
using MyAbpDemo.Core;
using MyAbpDemo.Infrastructure.Api;

namespace MyAbpDemo.Api.Controllers
{
    public class StudentController : ApiControllerBase
    {
        private readonly IStudentAppService _studentAppService;
        private readonly ITeacherAppService _teacherAppService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public StudentController(IStudentAppService studentAppService, 
            IHostingEnvironment hostingEnvironment, ITeacherAppService teacherAppService)
        {
             _studentAppService = studentAppService;
            _hostingEnvironment = hostingEnvironment;
            _teacherAppService = teacherAppService;
        }

        /// <summary>
        /// 获取学生信息
        /// 并执行后台工作BackgroundJob
        /// </summary>
        /// <returns></returns>
        /// <response code="200">成功</response>
        /// <response code="400">失败返回Result对象</response>  
        [HttpGet("students")]
        [ProducesResponseType(typeof(List<GetStudentListOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> Index()
        {
            var result = await _studentAppService.GetStudentListAsync();
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
            var result = await _studentAppService.CreateStudentAsync(input);
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
             var result = await _studentAppService.GetSingleStudentAsync(id);
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
        [HttpGet("exports")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Export()
        {
            string fileName = "学生";
            var list = await _studentAppService.GetExportStudentListAsync();

            //var list = new List<ExportStudent>();
            //for (int i = 1; i <= 21; i++)
            //{
            //    list.Add(new ExportStudent
            //    {
            //        Name="学生"+i,
            //        Age=10,
            //        LearnLevel="优",
            //        TeacherName="老师"+i
            //    });
            //}

            return  CommomExport(fileName, list,new List<CellPosition>());
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
            var result = await _studentAppService.ImportAsync(uploadedFile);
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
                return  CommomExport(path, result.Data,new List<CellPosition>());
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
            string fileName = "学生验证错误";
            var result = _studentAppService.GroupImport(uploadedFile);

            if (result.IsSuccess)
            {
                var dictionary = result.Data;

                return  CommomExport(fileName, dictionary.Keys.First(), dictionary.Values.First());
            }

            return BadRequest(result.BaseResult());
        }


        /// <summary>
        /// 导出两个单元格
        /// </summary>
        /// <returns></returns>
        /// <response code="200">成功</response>
        /// <response code="400">失败返回Result对象</response>  
        [HttpGet("exportMultiple")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ExportMultiple() 
        {
            string fileName = "学生验证错误";
            var students = await _studentAppService.GetExportStudentListAsync();
            var teachers = await _teacherAppService.GetTeacherListAsync();

            var exportSheetOne = new ExportSheet<ExportStudent>
            {
                 CellPositions=new List<CellPosition>(),
                 SheetName="学生",
                 Data= students
            };
            var exportSheetTwo = new ExportSheet<GetTeacherListOutput>
            {
                CellPositions = new List<CellPosition>(),
                SheetName = "老师",
                Data = teachers.Data
            };

            return MultipleSheetExport(fileName, exportSheetOne, exportSheetTwo);
        }

        /// <summary>
        /// 通用单个sheet导出公共方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="list"></param>
        /// <param name="cellPositions"></param>
        /// <returns></returns>
        private  IActionResult CommomExport<T>(string fileName, IEnumerable<T> list, List<CellPosition> cellPositions) where T : new()
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string directoryName = "TempExport";
            string directoryPath = Path.Combine(sWebRootFolder, directoryName);
            const int maxCount = 10;

            try
            {
                var currentTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                IActionResult actionResult = null;
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                //多文件压缩
                if (list.Count()> maxCount)
                {
                    int fileCount = list.Count() / maxCount + 1;
                    var filenamesAndStream = new ConcurrentDictionary<string, Stream>();

                    for (int i = 1; i <= fileCount; i++)
                    {
                        //导出excel
                        var fileFullName = $"{fileName}-{i}-{currentTime}.xlsx";
                        var fileInfo = new FileInfo(Path.Combine(sWebRootFolder, Path.Combine(directoryName, fileFullName)));
                        var filterList = list.Skip((i - 1) * maxCount).Take(maxCount);
                        EpplusHelper.Export(filterList, fileInfo, cellPositions);
                        
                        //构造 filenamesAndStream
                        var fileStream = System.IO.File.OpenRead(fileInfo.FullName);
                        filenamesAndStream.TryAdd(fileFullName, fileStream);
                    }

                    actionResult =ReturnZipFileResult(filenamesAndStream,fileName);
                }
                else//单文件
                {
                    var fileNamePath = Path.Combine(directoryName, $"{fileName}-{currentTime}.xlsx");
                    actionResult = ExportExcel(sWebRootFolder,fileNamePath, list,cellPositions);
                }

                return actionResult;
            }
            catch (Exception e)
            {
                Logger.Error("导出失败"+e.Message);
                return BadRequest("导出失败");
            }
        }

        /// <summary>
        ///多文件压缩
        /// </summary>
        /// <param name="filenamesAndStream"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private IActionResult ReturnZipFileResult(ConcurrentDictionary<string, Stream> filenamesAndStream,string fileName)
        {
            return new FileCallbackActionResult(new MediaTypeHeaderValue("application/octet-stream"), async (outputStream, _) =>
            {
                using (var zipArchive = new ZipArchive(new WriteOnlyStreamWrapper(outputStream), ZipArchiveMode.Create))
                {
                    foreach (var kvp in filenamesAndStream)
                    {
                        var zipEntry = zipArchive.CreateEntry(kvp.Key);
                        using (var zipStream = zipEntry.Open())
                        {
                            await kvp.Value.CopyToAsync(zipStream);
                        }
                    }
                }
            })
            {
                FileDownloadName = $"{fileName}.zip"
            };
        }

        /// <summary>
        /// 单文件导出
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webRootFolder">根目录</param>
        /// <param name="path">相对路径</param>
        /// <param name="list">数据源</param>
        /// <param name="cellPositions"></param>
        /// <returns></returns>
        private IActionResult ExportExcel<T>(string webRootFolder, string path, IEnumerable<T> list,List<CellPosition> cellPositions) where T : new()
        {
            FileInfo fileInfo = new FileInfo(Path.Combine(webRootFolder, path));
            EpplusHelper.Export(list, fileInfo, cellPositions);
            new FileExtensionContentTypeProvider().TryGetContentType(path, out string contentType);
            return File(path, contentType, Path.GetFileName(path));
        }


        /// <summary>
        /// 多个sheet导出公共方法
        /// </summary>
        /// <typeparam name="TS"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="exportSheetOne"></param>
        /// <param name="exportSheetTwo"></param>
        /// <returns></returns>
        private IActionResult MultipleSheetExport<TS, T>(string fileName, ExportSheet<TS> exportSheetOne, ExportSheet<T> exportSheetTwo)
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string path = $"TempExport\\{fileName}-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

            try
            {
                FileInfo fileInfo = new FileInfo(Path.Combine(sWebRootFolder, path));
                if (!Directory.Exists(fileInfo.DirectoryName))
                {
                    Directory.CreateDirectory(fileInfo.DirectoryName);
                }

                EpplusHelper.Export(exportSheetOne, exportSheetTwo,fileInfo);
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