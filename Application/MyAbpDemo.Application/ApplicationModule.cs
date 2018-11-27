using System;
using Abp.Modules;
using Abp.Reflection.Extensions;
using MyAbpDemo.ApplicationDto;

namespace MyAbpDemo.Application
{
    [DependsOn(typeof(ApplicationDtoModule))]
    public class ApplicationModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ApplicationModule).GetAssembly());
        }
    }
}
