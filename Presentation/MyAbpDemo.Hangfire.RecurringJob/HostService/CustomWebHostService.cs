using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace MyAbpDemo.Hangfire.RecurringJob.HostService
{
    public class CustomWebHostService :WebHostService
    {
        private static readonly NLog.Logger Logger = NLogBuilder.ConfigureNLog("NLog.config").GetCurrentClassLogger();

        public CustomWebHostService(IWebHost host) : base(host)
        {
        }

        protected override void OnStarting(string[] args)
        {
            Logger.Info("OnStarting method called.");
            base.OnStarting(args);
        }

        protected override void OnStarted()
        {
            Logger.Info("OnStarted method called.");
            base.OnStarted();
        }

        protected override void OnStopping()
        {
            Logger.Info("OnStopping method called.");
            base.OnStopping();
        }
    }
}
