using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;

namespace Traffix.Common.IocAttributes
{
    public class RequireHttpsAttribute : AuthorizationFilterAttribute
    {
        public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var request = actionContext.Request;

            // If request is not https then return error
            if (request.RequestUri.Scheme != Uri.UriSchemeHttps)
            {
                var html = "<p>Https is required</p>";

                actionContext.Response = request.CreateResponse(HttpStatusCode.NotFound);
                actionContext.Response.Content = new StringContent(html, Encoding.UTF8, "text/html");
            }
        }
    }
}
