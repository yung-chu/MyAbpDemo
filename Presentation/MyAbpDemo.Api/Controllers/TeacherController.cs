using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyAbpDemo.Application;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using MyAbpDemo.ApplicationDto;
using MyAbpDemo.Infrastructure;

namespace MyAbpDemo.Api.Controllers
{
 
    public class TeacherController : ApiControllerBase
    {
        private readonly ITeacherAppService _teacherAppService;

        public TeacherController(ITeacherAppService teacherAppService)
        {
            _teacherAppService = teacherAppService;
        }

        /// <summary>
        /// 获取老师信息
        /// </summary>
        /// <returns></returns>
        /// <response code="200">成功</response>
        /// <response code="400">失败返回Result对象</response>  
        [HttpGet("teachers")]
        [ProducesResponseType(typeof(List<GetTeacherListOutput>),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Result),StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTeacher()
        {
            //Logger.Info("记录日志: "+DateTime.Now );
            var result =await  _teacherAppService.GetTeacherListAsync();
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(new Result(result.Code, result.Message));
        }
    }
}