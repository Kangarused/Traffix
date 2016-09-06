using System;
using System.Web.Http;
using Autofac;
using Traffix.Common.Providers;
using Traffix.Web.WebApiFilters;

namespace Traffix.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config, IContainer container)
        {
            Func<ICryptoProvider> cryptoServiceFactory = container.Resolve<ICryptoProvider>;
            config.Formatters.Add(new CustomBinaryFormatter(cryptoServiceFactory));
            config.Filters.Add(new WebApiExceptionFilter());
        }
    }
}
