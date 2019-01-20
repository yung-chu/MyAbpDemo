using Abp.Configuration.Startup;
using Abp.Dependency;
using Castle.MicroKernel.Registration;
using EasyNetQ;
using EasyNetQ.ConnectionString;
using EasyNetQ.Consumer;
using EasyNetQ.DI;
using System;
using Abp.Configuration.Startup;

namespace MyAbpDemo.Infrastructure.EasyNetQ
{
    public static class AbpEasyNetQConfigurationExtensions
    {

        public static IAbpEasyNetQConfiguration AbpEasyNetQ(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IAbpEasyNetQConfiguration>();
        }

        public static void UseEasyNetQ(this IAbpStartupConfiguration abpConfiguration, Action<IAbpEasyNetQConfiguration> configureAction)
        {
            configureAction(abpConfiguration.Modules.AbpEasyNetQ());
            var easyNetQServiceRegister = IocManager.Instance.Resolve<IServiceRegister>();

            RabbitHutch.RegisterBus(easyNetQServiceRegister,
                c => c.Resolve<IConnectionStringParser>().Parse(abpConfiguration.Modules.AbpEasyNetQ().RabbitMqConnectionString),
                c => { });
           // abpConfiguration.IocManager.Resolve<IConventions>().ErrorQueueNamingConvention = s => $"ReTry_{s.Queue}";

        }
    }
}
