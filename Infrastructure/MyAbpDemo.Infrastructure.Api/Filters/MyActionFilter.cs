using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyAbpDemo.Infrastructure.Api
{
    public class MyActionFilter:IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            Console.WriteLine("执行前");
            var resultContext = await next();
            Console.WriteLine("执行后");
        }
    }
}
