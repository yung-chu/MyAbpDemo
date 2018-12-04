using System;
using System.Collections.Generic;
using System.Text;
using Abp.Dependency;
using Abp.Events.Bus.Exceptions;
using Abp.Events.Bus.Handlers;
using Abp.Runtime.Validation;

namespace MyAbpDemo.Infrastructure.Api
{
    public class ExceptionHandler :  IEventHandler<AbpHandledExceptionData>, ITransientDependency
    {
        public void HandleEvent(AbpHandledExceptionData eventData)
        {
            Console.WriteLine($"当前异常信息为：{eventData.Exception.Message}");

        }
    }
}
