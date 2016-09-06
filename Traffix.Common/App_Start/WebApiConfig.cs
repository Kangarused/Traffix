using System.Web.Http;
using Traffix.Common.Providers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Traffix.Common
{
    public static class CommonWebApiConfig
    {
        public static void ApplyJsonSetting(JsonSerializerSettings settings)
        {
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            settings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
            settings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            settings.Converters.Add(new StringEnumConverter());
        }

        public static void Register(HttpConfiguration config, bool isDebugMode)
        {
            // Web API configuration and services
            var formatters = config.Formatters;
            var jsonFormatter = formatters.JsonFormatter;
            var settings = jsonFormatter.SerializerSettings;
            ApplyJsonSetting(settings);
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.IncludeErrorDetailPolicy = isDebugMode ? IncludeErrorDetailPolicy.Always : IncludeErrorDetailPolicy.Never;
            
            config.Routes.MapHttpRoute(
                 name: "DefaultApi",
                 routeTemplate: "api/{controller}/{action}/{param}",
                 defaults: new { param = RouteParameter.Optional }
             );
        }
    }
}
