using System;
using System.Collections.Generic;
using System.Text;
using Abp.Modules;
using Abp.Reflection.Extensions;
using MyAbpDemo.EFCore;

namespace MyAbpDemo.Infrastructure.Api
{
    [DependsOn(typeof(EfCoreModule))]
    public class InfrastructureApiModule: AbpModule
    {
        /// <summary>
        /// This method is used to register dependencies for this module.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(InfrastructureApiModule).GetAssembly());
        }
    }
}
