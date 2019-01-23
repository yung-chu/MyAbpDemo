using System;
using System.Collections.Generic;
using System.Text;
using Abp.Dependency;
using Abp.Threading;
using Castle.Core.Logging;
using Hangfire.RecurringJobExtensions;
using Hangfire.Server;
using MyAbpDemo.Core;

namespace MyAbpDemo.Application
{
    public class MyJob1: IRecurringJob, ISingletonDependency
    {
        public ILogger Logger { get; set; }
        public readonly IStudentAppService _studentAppService;

        public MyJob1( StudentAppService studentAppService)
        {
            Logger = NullLogger.Instance;
            _studentAppService = studentAppService;
        }

        public void Execute(PerformContext context)
        {
            context.SetJobData("NewIntVal", 99);
            var newIntVal = context.GetJobData<int>("NewIntVal");
            var students = AsyncHelper.RunSync(()=> _studentAppService.GetStudentListAsync()).Data;
            Logger.Info($"{DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")} MyJob1 Running Content: {newIntVal} ..."); ;
        }
    }
}
