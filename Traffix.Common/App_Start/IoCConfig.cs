using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Traffix.Common.IocAttributes;
using Traffix.Common.Providers;

namespace Traffix.Common
{
    public class CommonIoCConfig
    {
        public static ContainerBuilder InitIoc(Assembly webAssembly, IEnumerable<Assembly> otherAssemblies)
        {
            var builder = new ContainerBuilder();
            var list = otherAssemblies.ToList();
            list.Add(webAssembly);

            builder.Register(c => new WebAssemblyResolver(webAssembly)).As<IWebAssemblyResolver>().SingleInstance();

            builder.RegisterAssemblyTypes(list.ToArray())
                .Where(t => t.GetCustomAttributes(typeof(SingletonAttribute), false).Any())
               .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
               .AsImplementedInterfaces().SingleInstance();

            builder.RegisterAssemblyTypes(list.ToArray())
                .Where(t => t.GetCustomAttributes(typeof(PerDependencyAttribute), false).Any())
                .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(list.ToArray())
                .Where(t => t.GetCustomAttributes(typeof(PerRequestAttribute), false).Any())
               .PropertiesAutowired(PropertyWiringOptions.AllowCircularDependencies)
               .AsImplementedInterfaces().InstancePerLifetimeScope();

            return builder;
        }


        public static IContainer WireIoc(ContainerBuilder builder, HttpConfiguration config, Assembly webAssembly)
        {
            
            builder.RegisterApiControllers(webAssembly);
            builder.RegisterWebApiFilterProvider(config);
            builder.RegisterControllers(webAssembly);

            var container = builder.Build();

            var resolver = new AutofacWebApiDependencyResolver(container);
            config.DependencyResolver = resolver;

            var mvcResolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(mvcResolver);

            return container;
        }
    }
}