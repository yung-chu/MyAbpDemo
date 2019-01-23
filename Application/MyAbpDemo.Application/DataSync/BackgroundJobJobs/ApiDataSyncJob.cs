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
    /// <summary>
    /// 后台作业以队列和持久化的方式来排队执行某些任务。
    /// </summary>
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
