using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Filters;
using Autofac.Integration.WebApi;
using Traffix.Common.Model;

namespace Traffix.Web.WebApiFilters
{
    public class WebApiExceptionFilter : ExceptionFilterAttribute, IAutofacExceptionFilter
    {
        public override Task OnExceptionAsync(HttpActionExecutedContext context, CancellationToken cancellationToken)
        {
            if (context.Request.Headers.UserAgent.ElementAt(0).Product.Name == Constants.PrivateApiUserAgent)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                context.Response.Headers.Add(Constants.OriginalExceptionHeader, Regex.Replace(context.Exception.Message, @"\r\n?|\n", ";"));
                context.Response.Headers.Add(Constants.OriginalExceptionStackHeader, Regex.Replace(context.Exception.StackTrace, @"\r\n?|\n", ";"));
            }
            return base.OnExceptionAsync(context, cancellationToken);
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Request.Headers.UserAgent.ElementAt(0).Product.Name == Constants.PrivateApiUserAgent)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                context.Response.Headers.Add(Constants.OriginalExceptionHeader, Regex.Replace(context.Exception.Message, @"\r\n?|\n", ";"));
                context.Response.Headers.Add(Constants.OriginalExceptionStackHeader, Regex.Replace(context.Exception.StackTrace, @"\r\n?|\n", ";"));
            }
        }
    }
}