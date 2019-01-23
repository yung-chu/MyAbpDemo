using System;
using Abp.Auditing;
using Abp.Dependency;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Threading.BackgroundWorkers;
using Castle.MicroKernel.Registration;
using MyAbpDemo.ApplicationDto;
using MyAbpDemo.Infrastructure.EasyNetQ;

namespace MyAbpDemo.Application
{
    [DependsOn(typeof(ApplicationDtoModule),typeof(EasyNetQModule))]
    public class ApplicationModule : AbpModule
    {
        public override void PostInitialize()
        {
            //注册 BackgroundWorker
            var workManager = IocManager.Resolve<IBackgroundWorkerManager>();
            workManager.Add(IocManager.Resolve<MakeInactiveUsersPassiveWorker>());
        }

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
