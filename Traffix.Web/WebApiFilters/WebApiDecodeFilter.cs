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
        private readonly ICryptoProvider _cryptoProvider;

        public WebApiDecodeFilter(ICryptoProvider cryptoProvider)
        {
            _cryptoProvider = cryptoProvider;
        }

        public override async void OnActionExecuted(HttpActionExecutedContext context)
        {
            if (context.Response != null && context.Request.Headers.UserAgent.ElementAt(0).Product.Name == Constants.PrivateApiUserAgent && context.Request.Method == HttpMethod.Get)
            {
                
                var byteArray = await context.Response.Content.ReadAsByteArrayAsync();
                var message = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                var encoded = _cryptoProvider.EncodeMessage(message);
                context.Response.Content = new ByteArrayContent(encoded);
                
            }
            base.OnActionExecuted(context);
        }
    }
}