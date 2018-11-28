using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAbpDemo.Application;
using MyAbpDemo.ApplicationDto;

namespace MyAbpDemo.Api.Controllers
{
    public class StudentController : AbpController
    {
        private readonly IStudentAppService _studentAppService;
        public StudentController(IStudentAppService studentAppService)
        {
            _studentAppService = studentAppService;
        }


        /// <summary>
        /// 获取老师信息
        /// </summary>
        /// <returns></returns>
        /// <response code="200">成功</response>
        /// <response code="400">失败返回Result对象</response>  
        [HttpGet("students")]
        [ProducesResponseType(typeof(List<GetStudentListOutput>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
      
        public async Task<IActionResult>  Index()
        {
            var result =await _studentAppService.GetStudentList();
            return Ok(result);
        }
    }
}