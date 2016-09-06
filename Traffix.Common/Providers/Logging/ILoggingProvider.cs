
using log4net;

namespace Traffix.Common.Providers.Logging
{
    public interface ILoggingProvider
    {
        ILog Logger
        {
            get;
        }
    }

    
    public class LoggingProvider : ILoggingProvider
    {
        public LoggingProvider(string loggerName)
        {
            _logger = LogManager.GetLogger(loggerName);
        }

        private readonly ILog _logger;

        public ILog Logger
        {
            get
            {
                return _logger;
            }
        }
    }
}
