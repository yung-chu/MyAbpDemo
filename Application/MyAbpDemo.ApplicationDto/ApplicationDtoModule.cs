using System;
using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using MyAbpDemo.Core;
using MyAbpDemo.EFCore;

namespace MyAbpDemo.ApplicationDto
{
    [DependsOn(typeof(CoreModule),typeof(EfCoreModule), typeof(AbpAutoMapperModule))]
    public class ApplicationDtoModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CoreModule).GetAssembly());
        }
    }
}
