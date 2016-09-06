using System.Web;
using System.Web.Routing;

namespace Traffix.Common.Utils.EmbeddedResourcesUtils
{
    public class EmbeddedResourceHttpHandler : IHttpHandler
    {
        private readonly RouteData _routeData;
        public EmbeddedResourceHttpHandler(RouteData routeData)
        {
            _routeData = routeData;
        }

        public bool IsReusable => false;

        public void ProcessRequest(HttpContext context)
        {
            var routeDataValues = _routeData.Values;
            var fileName = routeDataValues["file"].ToString();
            var fileExtension = routeDataValues["extension"].ToString();
            var contentType = fileExtension == "js" ? "scripts" : "styles";

            var folder = routeDataValues["folder"].ToString().Replace("-","_");
            string nameSpace = typeof(EmbeddedResourceHttpHandler)
                                .Assembly
                                .GetName()
                                .Name;// Mostly the default namespace and assembly name are same
            string manifestResourceName = $"{nameSpace}.Content.{contentType}.{folder}.{fileName}.{fileExtension}";
            var stream = typeof(EmbeddedResourceHttpHandler).Assembly.GetManifestResourceStream(manifestResourceName);
            if (stream == null)
            {
                throw new EmbeddedResourceException("Could not find embedded resource:" + manifestResourceName + ", check if the resource has the build action set to 'Embedded Resource'");
            }

            context.Response.Clear();
            context.Response.ContentType = "text/css";// default
            if (fileExtension == "js")
                context.Response.ContentType = "text/javascript";
            if (stream != null) stream.CopyTo(context.Response.OutputStream);
        }
    }
}
