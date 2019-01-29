using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Abp;
using Abp.AspNetCore.Configuration;
using Abp.AspNetCore.Mvc.ExceptionHandling;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.AspNetCore.Mvc.Results;
using Abp.Authorization;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Events.Bus.Exceptions;
using Abp.Runtime.Validation;
using Abp.UI;
using Abp.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyAbpDemo.Infrastructure.Api
{
    /// <summary>
    /// http://www.cnblogs.com/myzony/archive/2018/08/11/9460021.html
    /// ABP异常源码分析
    /// </summary>
    public class MyAbpExceptionFilter: AbpExceptionFilter
    {
        private readonly IErrorInfoBuilder _errorInfoBuilder;
        public MyAbpExceptionFilter(IErrorInfoBuilder errorInfoBuilder, IAbpAspNetCoreConfiguration configuration) : base(errorInfoBuilder, configuration)
        {
               _errorInfoBuilder = errorInfoBuilder;
        }

        protected override void HandleAndWrapException(ExceptionContext context, WrapResultAttribute wrapResultAttribute)
        {
            // 判断被调用接口的返回值是否符合标准，不符合则直接返回
            if (!MyActionResultHelper.IsObjectResult(context.ActionDescriptor.GetMethodInfo().ReturnType))
            {
                return;
            }

            //错误信息
            var errorInfo = _errorInfoBuilder.BuildForException(context.Exception);
            // 设置 HTTP 上下文响应所返回的错误代码，由具体异常决定。
            context.HttpContext.Response.StatusCode = CreateResult(context, errorInfo).Code;
            //自定义返回错误
            context.Result = CreateResult(context, errorInfo).actionResult;

            // 触发异常处理事件
            EventBus.Trigger(this, new AbpHandledExceptionData(context.Exception));
            
            // 处理完成，将异常上下文的内容置为空
            context.Exception = null; //Handled!
        }



        private (int Code, IActionResult actionResult) CreateResult(ExceptionContext context,ErrorInfo errorInfo)
        {
            var httpCode = StatusCodes.Status500InternalServerError;
            var resultCode = ResultCode.Fail;
            var message = errorInfo.Message;

            if (context.Exception is AbpAuthorizationException)
            {
                httpCode = context.HttpContext.User.Identity.IsAuthenticated
                    ? StatusCodes.Status403Forbidden
                    : StatusCodes.Status401Unauthorized;
            }
            else if (context.Exception is AbpValidationException)
            {
                httpCode = StatusCodes.Status400BadRequest;
                resultCode = ResultCode.ParameterFailed;

                message = $"{message} {errorInfo.Details}";
            }
            else if (context.Exception is UserFriendlyException)
            {
                httpCode = StatusCodes.Status400BadRequest;
            }
            else if (context.Exception is EntityNotFoundException)
            {
                httpCode = StatusCodes.Status404NotFound;
            }
            else if (context.Exception is AbpDbConcurrencyException)
            {
                httpCode = StatusCodes.Status400BadRequest;
                resultCode = ResultCode.ConcurrencyRecord;

                message = "数据冲突，请重新提交";
            }

            return (httpCode, new ObjectResult(Result.Fail(resultCode, message)));
        }




        //protected override int GetStatusCode(ExceptionContext context, bool wrapOnError)
        //{
        //    if (context.Exception is AbpAuthorizationException)
        //    {
        //        return context.HttpContext.User.Identity.IsAuthenticated
        //            ? (int)HttpStatusCode.Forbidden
        //            : (int)HttpStatusCode.Unauthorized;
        //    }

        //    if (context.Exception is AbpValidationException)
        //    {
        //        return (int)HttpStatusCode.BadRequest;
        //    }

        //    if (context.Exception is EntityNotFoundException)
        //    {
        //        return (int)HttpStatusCode.NotFound;
        //    }

        //    return (int)HttpStatusCode.InternalServerError;
        //}
    }

    // <summary>
    // 判断被调用接口的返回值是否符合标准，不符合则直接返回
    //https://blog.csdn.net/jsd2honey/article/details/61920905
    //MVC中几种常用ActionResult
    // </summary>
    public static class MyActionResultHelper
    {
        public static bool IsObjectResult(Type returnType)
        {
            //Get the actual return type (unwrap Task)
            if (returnType == typeof(Task))
            {
                returnType = typeof(void);
            }
            else if (returnType.GetTypeInfo().IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                returnType = returnType.GenericTypeArguments[0];
            }

            if (typeof(IActionResult).GetTypeInfo().IsAssignableFrom(returnType))
            {
                //确定指定类型的实例是否能分配给当前类型实例。
                //https://docs.microsoft.com/zh-cn/dotnet/api/system.type.isassignablefrom?view=netframework-4.7.2
                if (returnType.IsAssignableFrom(typeof(ObjectResult)))
                {
                    return true;
                }

                return false;
            }

            return false;
        }
    }
}
