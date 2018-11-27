using System;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace MyAbpDemo.Core
{

    public class CoreModule: AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CoreModule).GetAssembly());
        }
    }
}
