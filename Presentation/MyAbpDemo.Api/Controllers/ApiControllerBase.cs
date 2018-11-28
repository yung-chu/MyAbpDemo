using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Controllers;
using Abp.Dependency;
using Abp.Web.Models;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc;

namespace MyAbpDemo.Api.Controllers
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [DontWrapResult]  //去除默认返回包装类型
    [ApiController]
    public abstract class ApiControllerBase: AbpController
    {

    }
}
