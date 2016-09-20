using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Traffix.Common;
using Traffix.Common.Providers;
using Traffix.Web;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ApplicationStartup))]
namespace Traffix.Web
{
    public class ApplicationStartup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            
            AreaRegistration.RegisterAllAreas();
            log4net.Config.XmlConfigurator.Configure();
            
            var webAssembly = typeof(ApplicationStartup).Assembly;
            var builder = CommonIoCConfig.InitIoc(webAssembly, new[] { typeof(CommonIoCConfig).Assembly });
            IoCConfig.ConfigureIoc(builder);
            var container = CommonIoCConfig.WireIoc(builder, config, webAssembly);

            var configManager = container.Resolve<IConfigurationManagerProvider>();
            CommonWebApiConfig.Register(config, configManager.IsDebugMode());
            
            WebApiConfig.Register(config, container);
            
            app.UseWebApi(config);

            app.MapSignalR();


            BundleConfig.RegisterBundles(BundleTable.Bundles);
            CommonRouteConfig.RegisterRoutes(RouteTable.Routes);
        }   
    }
}