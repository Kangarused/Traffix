using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Traffix.Common.IocAttributes;
using Traffix.Common.Providers;

namespace Traffix.Web.Providers
{
    [Singleton]
    public class DatabaseVersionReader : IDatabaseVersionReader
    {
        //todo:implement this
        public Task<long> GetDatabaseVersionAsync()
        {
            return Task.Run(() => Convert.ToInt64(0));
        }
    }
}