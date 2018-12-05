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
using Abp.Events.Bus.Exceptions;
using Abp.Runtime.Validation;
using Abp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyAbpDemo.Infrastructure.Api.Filters
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

            // 设置 HTTP 上下文响应所返回的错误代码，由具体异常决定。
            int code = GetStatusCode(context,true);//wrapResultAttribute.WrapOnError
            context.HttpContext.Response.StatusCode = code;


            //错误信息
            var errorInfo = _errorInfoBuilder.BuildForException(context.Exception);
            string errorMessage = errorInfo.Message;
            if (code== (int)HttpStatusCode.BadRequest)
            {
                errorMessage = errorInfo.ValidationErrors != null && errorInfo.ValidationErrors.Any() ?
                    errorMessage + " " + errorInfo.ValidationErrors.Select(a => a.Message).First() : errorMessage;
            }


            //自定义返回错误
            context.Result = new ObjectResult(
                new Result(code== (int)HttpStatusCode.BadRequest?ResultCode.ParameterFailed: ResultCode.Fail, errorMessage)
            );

            // 触发异常处理事件
            EventBus.Trigger(this, new AbpHandledExceptionData(context.Exception));
            
            // 处理完成，将异常上下文的内容置为空
            context.Exception = null; //Handled!
        }

        protected override int GetStatusCode(ExceptionContext context, bool wrapOnError)
        {
            if (context.Exception is AbpAuthorizationException)
            {
                return context.HttpContext.User.Identity.IsAuthenticated
                    ? (int)HttpStatusCode.Forbidden
                    : (int)HttpStatusCode.Unauthorized;
            }

            if (context.Exception is AbpValidationException)
            {
                return (int)HttpStatusCode.BadRequest;
            }

            if (context.Exception is EntityNotFoundException)
            {
                return (int)HttpStatusCode.NotFound;
            }

            return (int)HttpStatusCode.InternalServerError;
        }
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
