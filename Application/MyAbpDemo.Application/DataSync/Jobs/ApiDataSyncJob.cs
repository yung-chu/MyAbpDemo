using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Net.Mail.Smtp;
using MyAbpDemo.Core;

namespace MyAbpDemo.Application
{
    public class ApiDataSyncJob : BackgroundJob<ApiDataSyncJobArgs>, ITransientDependency
    {
        private readonly ISmtpEmailSenderConfiguration _smtpEmialSenderConfig;
        public ApiDataSyncJob(ISmtpEmailSenderConfiguration smtpEmailSenderConfiguration)
        {
            _smtpEmialSenderConfig = smtpEmailSenderConfiguration;
        }

        public override void Execute(ApiDataSyncJobArgs args)
        {
            //模拟延迟效果200s
            Thread.Sleep(1000*200);
            Logger.Info("后台工作执行了"+System.DateTime.Now.ToLocalTime());
        }
    }
}
