using System;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace MyAbpDemo.Infrastructure
{
    public class InfrastructureModule:AbpModule
    {
        /// <summary>
        /// This method is used to register dependencies for this module.
        /// </summary>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(InfrastructureModule).GetAssembly());
        }
    }
}
