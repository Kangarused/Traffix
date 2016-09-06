using System.Reflection;
using System.Web.Optimization;
using Traffix.Common.Utils;

namespace Traffix.Web
{
    public class BundleConfig
    {
        private static readonly string Version = "_" + Assembly.GetCallingAssembly().GetName().Version;
        public static string SiteScripts = "~/bundles/scripts" + Version;
        public static string SiteStyles = "~/bundles/styles" + Version;
        public static string SitePartials = "~/bundles/partials" + Version;

        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle(SiteStyles)
                .Include("~/Content/styles/traffix-custom.css", new CssRewriteUrlTransform())
           );
            
            bundles.Add(new ScriptBundle(SiteScripts)
                .IncludeDirectory("~/Scripts", "*.js")
                //.IncludeDirectory("~/App/models", "*.js")
                .IncludeDirectory("~/App/controllers", "*.js")
                .Include("~/App/services/baseDataService.js")
                .IncludeDirectory("~/App/services", "*.js")
                //.IncludeDirectory("~/App/directives", "*.js")
                .Include("~/App/app.js")
                .Include("~/App/appValidationRules.js")
                .Include("~/App/appValidationSchemas.js")
            );

            bundles.Add(new TemplateBundle("traffix", SitePartials)
                .IncludeDirectory("~/App/views", "*.html"));
        }
    }
}