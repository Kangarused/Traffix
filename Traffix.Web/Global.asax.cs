using System.Web;
using System.Web.SessionState;

namespace Traffix.Web
{
    public class Global : HttpApplication
    {
        protected void Application_PostAuthorizeRequest()
        {
            if (HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith("~/api"))
            {
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
            }
        }
    }
}
