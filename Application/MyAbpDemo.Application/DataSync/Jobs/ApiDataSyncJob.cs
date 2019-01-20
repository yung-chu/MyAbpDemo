using System;
using System.Collections.Generic;
using System.Text;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Net.Mail.Smtp;

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
            Console.WriteLine("5465656");
        }
    }
}
