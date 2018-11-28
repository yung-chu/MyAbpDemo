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
        /// <summary>
        ///automapper Configuration
        /// http://docs.automapper.org/en/latest/Configuration.html?highlight=AddProfiles
        /// </summary>
        public override void PreInitialize()
        {
            //添加AutoMapper Profile
            Configuration.Modules.AbpAutoMapper().Configurators.Add(
                cfg => cfg.AddProfiles(typeof(ApplicationDtoModule).GetAssembly())
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CoreModule).GetAssembly());
        }
    }
}
