using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore;
using Abp.Auditing;
using Abp.Dependency;
using Abp.Hangfire;
using Abp.Hangfire.Configuration;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching.Redis;
using Microsoft.Extensions.Configuration;
using MyAbpDemo.Application;
using MyAbpDemo.Core;
using MyAbpDemo.Infrastructure.Api;

namespace MyAbpDemo.Api
{
    /// <summary>
    /// AbpAspNetCoreModule 必须依赖的
    /// </summary>
    [DependsOn(typeof(ApplicationModule),typeof(InfrastructureApiModule),
        typeof(AbpRedisCacheModule),typeof(AbpAspNetCoreModule),typeof(AbpHangfireAspNetCoreModule))]
    public class ApiModule:AbpModule
    {
        public override void PreInitialize()
        {
            var configuration = IocManager.Resolve<IConfiguration>();

            //替换ABP默认的后台工作管理器。
            Configuration.BackgroundJobs.UseHangfire();

            //配置使用Redis缓存
            //Configuration.Caching.UseRedis(options => options.ConnectionString= configuration.GetConnectionString("RedisServer"));

            //设置默认链接
            Configuration.DefaultNameOrConnectionString =
                configuration.GetConnectionString(MyAbpDemoConst.ConnectionStringName);
        }

        /// <summary>
        /// This method is used to register dependencies for this module.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ApiModule).GetAssembly());
        }
    }
}
