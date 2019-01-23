using System;
using System.Collections.Generic;
using System.Text;
using Abp.Dependency;
using Castle.Core.Logging;
using Hangfire.RecurringJobExtensions;
using Hangfire.Server;

namespace MyAbpDemo.Application
{
    public class MyJob2 : IRecurringJob, ISingletonDependency
    {
        public ILogger Logger { get; set; }
        public MyJob2(StudentAppService studentAppService)
        {
            Logger = NullLogger.Instance;
        }

        public void Execute(PerformContext context)
        {
            //取值
            var intVal = context.GetJobData<int>("IntVal");
            var stringVal = context.GetJobData<string>("StringVal");
            var booleanVal = context.GetJobData<bool>("BooleanVal");
            var simpleObject = context.GetJobData<SimpleObject>("SimpleObject");
        }
    }

    class SimpleObject
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
}
