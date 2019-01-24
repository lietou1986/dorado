using Autofac;
using Autofac.Integration.Mvc;
using Dorado.Core;
using Dorado.Core.Cache;
using Dorado.Core.Cache.StorageStrategys;
using Dorado.Core.SuperCache;
using Dorado.Platform.Events;
using Dorado.Platform.FileSystems.AppData;
using Dorado.Platform.FileSystems.LockFile;
using Dorado.Platform.FileSystems.VirtualPath;
using Dorado.Platform.FileSystems.WebSite;
using Dorado.Platform.Infrastructure.DependencyManagement;
using Dorado.Platform.Services;
using Dorado.Services;
using Dorado.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Dorado.Platform.Infrastructure
{
    public class DoradoContext : LazySingleton<DoradoContext>
    {
        public ContainerManager ContainerManager { get; private set; }

        private ITypeFinder CreateTypeFinder()
        {
            return new WebAppTypeFinder();
        }

        private ContainerManager CreateHostContainer(Action<ContainerBuilder> registrations)
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();
            var typeFinder = CreateTypeFinder();

            // core dependencies
            builder = new ContainerBuilder();

            builder.RegisterInstance(typeFinder).As<ITypeFinder>();
            builder.RegisterModule(new CacheModule());
            builder.RegisterModule(new EventsModule());

            builder.RegisterType<Signals>().As<ISignals>();
            builder.RegisterType<JsonNetSerializer>().As<IJsonSerializer>().SingleInstance();

            RegisterVolatileProvider<WebSiteFolder, IWebSiteFolder>(builder);
            RegisterVolatileProvider<AppDataFolder, IAppDataFolder>(builder);
            RegisterVolatileProvider<DefaultLockFileManager, ILockFileManager>(builder);
            RegisterVolatileProvider<Clock, IClock>(builder);
            RegisterVolatileProvider<DefaultVirtualPathMonitor, IVirtualPathMonitor>(builder);
            RegisterVolatileProvider<DefaultVirtualPathProvider, IVirtualPathProvider>(builder);

            registrations(builder);

            // Autofac
            var lifetimeScopeAccessor = new DefaultLifetimeScopeAccessor(container);
            var lifetimeScopeProvider = new DefaultLifetimeScopeProvider(lifetimeScopeAccessor);
            builder.RegisterInstance(lifetimeScopeAccessor).As<ILifetimeScopeAccessor>();
            builder.RegisterInstance(lifetimeScopeProvider).As<ILifetimeScopeProvider>();

            var dependencyResolver = new AutofacDependencyResolver(container, lifetimeScopeProvider);
            builder.RegisterInstance(dependencyResolver);
            DependencyResolver.SetResolver(dependencyResolver);

            builder.Update(container);

            builder = new ContainerBuilder();
            var registrarTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            var registrarInstances =
                registrarTypes.Select(type => (IDependencyRegistrar)Activator.CreateInstance(type)).ToList();
            // sort
            registrarInstances = registrarInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            foreach (var registrar in registrarInstances)
            {
                registrar.Register(builder, typeFinder);
            }

            builder.Update(container);

            //
            // Register Virtual Path Providers
            //
            if (!HostingEnvironment.IsHosted) return new ContainerManager(container);

            foreach (var vpp in container.Resolve<IEnumerable<ICustomVirtualPathProvider>>())
            {
                HostingEnvironment.RegisterVirtualPathProvider(vpp.Instance);
            }

            return new ContainerManager(container);
        }

        public IDoradoHost CreateHost(Action<ContainerBuilder> registrations)
        {
            ContainerManager = CreateHostContainer(registrations);
            return ContainerManager.Resolve<IDoradoHost>();
        }

        private void RegisterVolatileProvider<TRegister, TService>(ContainerBuilder builder) where TService : IVolatileProvider
        {
            builder.RegisterType<TRegister>()
                .As<TService>()
                .As<IVolatileProvider>()
                .SingleInstance();
        }
    }

    /// <summary>
    /// 缓存模块
    /// </summary>
    internal class CacheModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DefaultCacheManager>().As<ICacheManager>().InstancePerDependency();
            builder.RegisterType<DefaultCacheHolder>().As<ICacheHolder>().SingleInstance();
            builder.RegisterType<DefaultCacheContextAccessor>().As<ICacheContextAccessor>();
            builder.RegisterType<MemoryStorageStrategy<string, object>>().As<IStorageStrategyEx<string, object>>();
            builder.Register(c => new Core.Cache<string, object>(c.Resolve<IStorageStrategyEx<string, object>>())).SingleInstance();
        }

        protected override void AttachToComponentRegistration(Autofac.Core.IComponentRegistry componentRegistry, Autofac.Core.IComponentRegistration registration)
        {
            var needsCacheManager = registration.Activator.LimitType
                .GetConstructors()
                .Any(x => x.GetParameters()
                    .Any(xx => xx.ParameterType == typeof(ICacheManager)));

            if (needsCacheManager)
            {
                registration.Preparing += (sender, e) =>
                {
                    var parameter = new TypedParameter(
                        typeof(ICacheManager),
                        e.Context.Resolve<ICacheManager>(new TypedParameter(typeof(Type), registration.Activator.LimitType)));
                    e.Parameters = e.Parameters.Concat(new[] { parameter });
                };
            }
        }
    }

    internal class EventsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterSource(new EventsRegistrationSource());
            base.Load(builder);
        }
    }
}