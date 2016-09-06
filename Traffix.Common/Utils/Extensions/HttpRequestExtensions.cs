using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Traffix.Common.Utils.Extensions
{
    public static class HttpRequestExtensions
    {
        public static string GetIpAddress(this HttpRequest request)
        {
            if (request==null)
                throw new ArgumentException("Requets is null", nameof(request));

            string forwardedFor = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            string ipStr = string.IsNullOrWhiteSpace(forwardedFor)
                               ? request.ServerVariables["REMOTE_ADDR"]
                               : forwardedFor.Split(',').Select(s => s.Trim()).First();
            return ipStr;
        }
    }
}
