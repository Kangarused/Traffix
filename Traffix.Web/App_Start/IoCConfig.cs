using System.Net.Http.Formatting;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Traffix.Common.Providers;
using Traffix.Common.Providers.Logging;
using Traffix.Web.Database.OrmLiteInfrastructure;
using Traffix.Web.WebApiFilters;

namespace Traffix.Web
{
    public class IoCConfig
    {
        public static void ConfigureIoc(ContainerBuilder builder)
        {
            builder.Register(c => new ConfigurationManagerProvider("DatabaseConnectionString")).As<IConfigurationManagerProvider>().SingleInstance();

            builder.Register(c => new LoggingProvider("Traffix Private")).As<ILoggingProvider>().SingleInstance();

            builder.Register(c => new TransactionFilterAttribute(c.Resolve<IUnitOfWork>()))
            .AsWebApiActionFilterFor<ApiController>()
            .InstancePerRequest();

            builder.Register(c => new WebApiExceptionFilter())
            .AsWebApiExceptionFilterFor<ApiController>()
            .InstancePerRequest();

        }
    }
}