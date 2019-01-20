using Abp.Dependency;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EasyNetQ.DI;
using System;

namespace MyAbpDemo.Infrastructure.EasyNetQ
{
    internal class AbpServiceRegisterAdapter : IServiceRegister, ISingletonDependency
    {
        public Lazy<IWindsorContainer> Container => new Lazy<IWindsorContainer>(() => Abp.Dependency.IocManager.Instance.IocContainer);
        private class AbpServiceResolverResolver : IServiceResolver
        {
            private readonly IKernel kernel;

            public AbpServiceResolverResolver(IKernel kernel)
            {
                this.kernel = kernel;
            }

            public TService Resolve<TService>() where TService : class
            {
                return kernel.Resolve<TService>();
            }

            public IServiceResolverScope CreateScope()
            {
                return new ServiceResolverScope(this);
            }
        }

        public AbpServiceRegisterAdapter()
        {
            this.Container.Value.Register(Component.For<IServiceResolver>()
              .UsingFactoryMethod(c => new AbpServiceResolverResolver(c))
              .LifestyleTransient());
        }

        public IServiceRegister Register<TService, TImplementation>(Lifetime lifetime = Lifetime.Singleton) where TService : class where TImplementation : class, TService
        {
            var registration = Component.For<TService>()
                                        .Named(Guid.NewGuid().ToString())
                                        .ImplementedBy<TImplementation>()
                                        .LifeStyle.Is(GetLifestyleType(lifetime))
                                        .IsDefault();
            this.Container.Value.Register(registration);
            return this;
        }

        public IServiceRegister Register<TService>(TService instance) where TService : class
        {
            var registration = Component.For<TService>()
                                        .Named(Guid.NewGuid().ToString())
                                        .Instance(instance)
                                        .LifestyleSingleton()
                                        .IsDefault();
            this.Container.Value.Register(registration);
            return this;
        }

        public IServiceRegister Register<TService>(Func<IServiceResolver, TService> factory, Lifetime lifetime = Lifetime.Singleton) where TService : class
        {
            var registration = Component.For<TService>()
                                        .Named(Guid.NewGuid().ToString())
                                        .UsingFactoryMethod(x => factory(x.Resolve<IServiceResolver>()))
                                        .LifeStyle.Is(GetLifestyleType(lifetime))
                                        .IsDefault();
            this.Container.Value.Register(registration);
            return this;
        }

        private LifestyleType GetLifestyleType(Lifetime lifetime)
        {
            switch (lifetime)
            {
                case Lifetime.Transient:
                    return LifestyleType.Transient;
                case Lifetime.Singleton:
                    return LifestyleType.Singleton;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, null);
            }
        }
    }
}
