using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore;
using Abp.Castle.Logging.NLog;
using Castle.Facilities.Logging;
using Hangfire;
using Hangfire.RecurringJobExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAbpDemo.Infrastructure.EFCore;

namespace MyAbpDemo.Hangfire.RecurringJob
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //注入数据库上下文
            services.AddDbContext<MyAbpDemoDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("Default"));
            });

            //HangFire配置
            services.AddHangfire(x =>
            {
                //使用SQL Server 作为存储
                x.UseSqlServerStorage(Configuration.GetConnectionString("Default"));
                //https://github.com/icsharp/Hangfire.RecurringJobExtensions
                x.UseRecurringJob("recurringjob.json");
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //ABP集成到ASP.NET Core和依赖注入，在最后调用
            return services.AddAbp<RecurringJobModule>(
                options => options.IocManager.IocContainer.AddFacility<LoggingFacility>(f => f.UseAbpNLog().WithConfig("NLog.config"))
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {  
            //初始化ABP框架和所有其他模块，这个应该首先被调用
            app.UseAbp();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //配置Hangfire
            app.UseHangfireServer(new BackgroundJobServerOptions
            {
                Queues = new[] { "default", "apis", "jobs" },
                WorkerCount = Math.Max(Environment.ProcessorCount, Configuration.GetValue<int>("hangfire.consumer.threadCount")),
                ShutdownTimeout = TimeSpan.FromMinutes(30),
                ServerName = "MyAbpDemoSchedule"
            });
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                //Authorization = new[] { new AbpHangfireAuthorizationFilter() }
            });

            app.UseMvc();
        }
    }
}
