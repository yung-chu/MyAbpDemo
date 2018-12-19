using System;
using Abp.Auditing;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Castle.MicroKernel.Registration;
using MyAbpDemo.ApplicationDto;

namespace MyAbpDemo.Application
{
    [DependsOn(typeof(ApplicationDtoModule))]
    public class ApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            //Configuration.ReplaceService(typeof(IAuditingStore), () =>
            //{
            //    IocManager.Register<IAuditingStore, MyAuditingStore>(DependencyLifeStyle.Transient);
            //});
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ApplicationModule).GetAssembly());
        }
    }
}
