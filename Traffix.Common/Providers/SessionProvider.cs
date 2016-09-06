using System;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Traffix.Common.IocAttributes;

namespace Traffix.Common.Providers
{
    public interface ISessionProvider
    {
        bool IsSessionAvailable { get; }
        void Set<T>(string key, T value);
        T Get<T>(string key);
        bool ContainsKey(string key);
        bool TryGetValue<T>(string key, out T value);
    }

    [Singleton]
    public class SessionProvider : ISessionProvider
    {
        public HttpSessionState Session
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    throw new NotImplementedException("Context is null, Session only works during web requests");
                }
                if (HttpContext.Current.Session == null)
                {
                    throw new NotImplementedException("Session is null.");
                }
                return HttpContext.Current.Session;
            }
        }

        public bool IsSessionAvailable => HttpContext.Current != null && HttpContext.Current.Session != null;

        public void Set<T>(string key, T value)
        {
            Session[key] = value;
        }

        public T Get<T>(string key)
        {
            return (T)Session[key];
        }

        public bool ContainsKey(string key)
        {
            return Session.Keys.Cast<string>().Contains(key);
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            value = default(T);
            if (ContainsKey(key))
            {
                value = Get<T>(key);
                return true;
            }
            return false;
        }
    }
}
