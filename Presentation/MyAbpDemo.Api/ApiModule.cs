using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore;
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
    /// AbpAspNetCoreModule
    /// </summary>
    [DependsOn(typeof(ApplicationModule),typeof(InfrastructureApiModule),typeof(AbpRedisCacheModule),typeof(AbpAspNetCoreModule))]
    public class ApiModule:AbpModule
    {
        public override void PreInitialize()
        {
            var configuration = IocManager.Resolve<IConfiguration>();

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
