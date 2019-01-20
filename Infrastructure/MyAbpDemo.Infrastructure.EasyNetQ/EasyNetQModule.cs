using Abp.Dependency;
using Abp.Modules;
using EasyNetQ.Consumer;
using EasyNetQ.DI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Castle.MicroKernel.Registration;

namespace MyAbpDemo.Infrastructure.EasyNetQ
{
    [DependsOn(typeof(InfrastructureModule))]
    public class EasyNetQModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpEasyNetQConfiguration, AbpEasyNetQConfiguration>();
            IocManager.Register<IServiceRegister, AbpServiceRegisterAdapter>();
            Configuration.ReplaceService(typeof(IConsumerErrorStrategy), () =>
            {
                Configuration.IocManager.IocContainer.Register(
                    Component.For<IConsumerErrorStrategy>()
                        .ImplementedBy<ConsumerErrorStategy>()
                        .LifestyleSingleton()
                        .IsDefault()
                    );
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
