using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace MyAbpDemo.Infrastructure
{
    /// <summary>
    /// 用于api模型验证错误自定义输出
    /// </summary>
    public static class InvalidModelStateExecutor
    {
        public static Func<ActionContext, IActionResult> Executer = (context) =>
        {
            var firstErrors = context.ModelState.First(o => o.Value.Errors.Any()).Value.Errors;

            var errorMessage = string.Join(" ", firstErrors.Select(o => o.ErrorMessage));

            return new BadRequestObjectResult(new Result(ResultCode.ParameterFailed, errorMessage));
        };
    }
}
