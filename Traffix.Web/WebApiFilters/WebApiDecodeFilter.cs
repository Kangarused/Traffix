using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using Traffix.Common.Model;
using Traffix.Common.Providers;

namespace Traffix.Web.WebApiFilters
{
    public class WebApiDecodeFilter : ActionFilterAttribute, IAutofacActionFilter
    {
        public WebApiDecodeFilter()
        {
            
        }

        public override async void OnActionExecuted(HttpActionExecutedContext context)
        {
            if (context.Response != null && context.Request.Headers.UserAgent.ElementAt(0).Product.Name == Constants.PrivateApiUserAgent && context.Request.Method == HttpMethod.Get)
            {
                var byteArray = await context.Response.Content.ReadAsByteArrayAsync();
                context.Response.Content = new ByteArrayContent(byteArray);
            }
            base.OnActionExecuted(context);
        }
    }
}