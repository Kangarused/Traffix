using Traffix.Common.IocAttributes;
using log4net.Appender;
using log4net.Core;

namespace Traffix.Common.Providers.Logging
{
    [Singleton]
    public class ConsoleAppender : AppenderSkeleton, IAppender
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            if (loggingEvent.ExceptionObject != null)
            {
                System.Diagnostics.Debug.WriteLine("{0} - {1}, Exception:{2}", loggingEvent.Level, loggingEvent.RenderedMessage, loggingEvent.ExceptionObject.Message + ":" + loggingEvent.ExceptionObject.StackTrace);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("{0} - {1}", loggingEvent.Level, loggingEvent.RenderedMessage);
            }
        }
    }
}
