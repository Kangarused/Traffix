using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Traffix.Common.Dtos;
using Traffix.Common.Model;
using Traffix.Web.Database.Repositories;

namespace Traffix.Web.Controllers
{
    public class TrafficLogController : ApiController
    {
        private readonly ITrafficLogRepoistory _trafficLogRepoistory;

        public TrafficLogController(ITrafficLogRepoistory trafficLogRepoistory)
        {
            _trafficLogRepoistory = trafficLogRepoistory;
        }

        [AcceptVerbs("GET")]
        public async Task<List<TrafficLog>> GetTrafficLogsForMeter(int param)
        {
            var logs = await _trafficLogRepoistory.GetTrafficLogsForMeter(param);
            return logs;
        }

        [AcceptVerbs("POST")]
        public async Task<List<TrafficLogsRequest>> GetTrafficLogsForMeters(TrafficMetersList list)
        {
            var logs = await _trafficLogRepoistory.GetTrafficLogsForMeters(list);
            return logs;
        }
    }
}