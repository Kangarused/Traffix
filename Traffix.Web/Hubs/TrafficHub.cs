using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Traffix.Common.Model;

namespace Traffix.Web.Hubs
{
    [HubName("trafficHub")]
    public class TrafficHub : Hub
    {
        public void MeterUpdated(string region, TrafficMeter meter, TrafficLog log)
        {
            Clients.All.meterUpdated(region, meter, log);
        }
    }
}