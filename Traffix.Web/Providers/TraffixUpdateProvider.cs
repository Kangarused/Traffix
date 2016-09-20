using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Traffix.Common.IocAttributes;
using Traffix.Common.Model;
using Traffix.Web.Hubs;

namespace Traffix.Web.Providers
{
    public interface ITraffixUpdateProvider
    {
        void MeterUpdated(string region, TrafficMeter meter, TrafficLog log);
    }

    [Singleton]
    public class TraffixUpdateProvider : ITraffixUpdateProvider
    {
        private IHubConnectionContext<dynamic> Clients
        {
            get;
            set;
        }

        public TraffixUpdateProvider()
        {
            Clients = GlobalHost.ConnectionManager.GetHubContext<TrafficHub>().Clients;
        }

        public void MeterUpdated(string region, TrafficMeter meter, TrafficLog log)
        {
            Clients.All.meterUpdated(region, meter, log);
        }
    }
}