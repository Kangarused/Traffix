using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Traffix.Web.Hubs
{
    public interface ITrafficHubClient
    {
        [HubMethodName("updateMap")]
        void UpdateMap(string region);
    }

    public class TrafficHub : Hub<ITrafficHubClient>
    {
        public TrafficHub()
        {
            
        }

        public void NotifyClients(string region)
        {
            Clients.All.UpdateMap(region);
        }
    }
}