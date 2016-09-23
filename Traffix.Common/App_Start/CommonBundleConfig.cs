using System.Web.Hosting;
using System.Web.Optimization;
using System.Web.Routing;
using Traffix.Common;
using Traffix.Common.Utils.EmbeddedResourcesUtils;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(CommonBundleConfig), "Start")]

namespace Traffix.Common
{
   
    public class CommonBundleConfig
    {
        public const string SiteCommonScripts = "~/TraffixCommon/Embedded/Js";
        public const string SiteCommonAngularSettings = "~/TraffixCommon/Embedded/As";
        public const string SiteCommonStyles = "~/TraffixCommon/Embedded/Styles";

        public static void Start()
        {
            ConfigureRoutes();
            ConfigureBundles();
        }

        private static void ConfigureBundles()
        {
            //Debugger -- check to see which assets are being loaded in
            //var test = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();

            BundleTable.VirtualPathProvider = new EmbeddedVirtualPathProvider(HostingEnvironment.VirtualPathProvider);
            BundleTable.Bundles.Add(new ScriptBundle(SiteCommonScripts)
                .Include(
                    "~/TraffixCommon/Embedded/scripts/jquery/jquery-1.11.3.js",
                    "~/TraffixCommon/Embedded/scripts/signalr/jquery.signalR-2.2.1.js",
                    "~/TraffixCommon/Embedded/scripts/angular/angular.js",
                    "~/TraffixCommon/Embedded/scripts/angular-case/angular-case.js",
                    "~/TraffixCommon/Embedded/scripts/angular-permission/angular-permission.js",
                    "~/TraffixCommon/Embedded/scripts/angular-ui-router/angular-ui-router.js",
                    "~/TraffixCommon/Embedded/scripts/angular/angular-sanitize.js",
                    "~/TraffixCommon/Embedded/scripts/angular/angular-cookies.js",
                    "~/TraffixCommon/Embedded/scripts/angular/angular-dom-events.js",
                    "~/TraffixCommon/Embedded/scripts/linqjs/linq.js",
                    
                    "~/TraffixCommon/Embedded/scripts/bootstrap/bootstrap.js",
                    "~/TraffixCommon/Embedded/scripts/angular-bootstrap/ui-bootstrap-tpls-1.3.3.js",
                    "~/TraffixCommon/Embedded/scripts/custom-angular/show-when-loading.js",
                    "~/TraffixCommon/Embedded/scripts/ng-map/ng-map.js"
            ));
        }

        private static void ConfigureRoutes()
        {
            RouteTable.Routes.Insert(0,
                new Route("TraffixCommon/Embedded/scripts/{folder}/{file}.{extension}",
                    new RouteValueDictionary(new { }),
                    new RouteValueDictionary(new { extension = "js" }),
                    new EmbeddedResourceRouteHandler()
                ));

            RouteTable.Routes.Insert(0,
                new Route("TraffixCommon/Embedded/styles/{folder}/{file}.{extension}",
                    new RouteValueDictionary(new { }),
                    new RouteValueDictionary(new { extension = "css" }),
                    new EmbeddedResourceRouteHandler()
                ));

        }
    }
}