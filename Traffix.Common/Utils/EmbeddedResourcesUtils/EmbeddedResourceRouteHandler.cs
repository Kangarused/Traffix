using System.Web;
using System.Web.Routing;

namespace Traffix.Common.Utils.EmbeddedResourcesUtils
{
    public class EmbeddedResourceRouteHandler : IRouteHandler
    {
        IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext)
        {
            return new EmbeddedResourceHttpHandler(requestContext.RouteData);
        }
    }
}
