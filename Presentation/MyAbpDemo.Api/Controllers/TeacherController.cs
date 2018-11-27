using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyAbpDemo.Application;

namespace MyAbpDemo.Api.Controllers
{

    public class TeacherController : ApiControllerBase
    {
        private readonly ITeacherAppService _teacherAppService;

        public TeacherController(ITeacherAppService teacherAppService)
        {
            _teacherAppService = teacherAppService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var list = _teacherAppService.GetTeacherList();
            return   Ok(list);
        }
    }
}