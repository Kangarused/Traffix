using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Traffix.Common.Dtos;
using Traffix.Common.Model;
using Traffix.Web.Database.Repositories;

namespace Traffix.Web.Controllers
{
    public class TrafficMeterController : ApiController
    {
        private readonly ITrafficMeterRepository _trafficMeterRepoistory;

        public TrafficMeterController(ITrafficMeterRepository trafficMeterRepoistory)
        {
            _trafficMeterRepoistory = trafficMeterRepoistory;
        }

        [AcceptVerbs("GET")]
        public async Task<List<TrafficMeter>> GetTrafficMeters(string param)
        {
            var results = await _trafficMeterRepoistory.GetMetersForRegion(param);
            return results;
        }

        [AcceptVerbs("GET")]
        public async Task<List<LinkedTrafficMeters>> GetLinkedTrafficMeters(string param)
        {
            var results = await _trafficMeterRepoistory.GetMetersForRegion(param);
            List<LinkedTrafficMeters> list = new List<LinkedTrafficMeters>();

            foreach (var meterGroup in results.GroupBy(x => x.LinkId))
            {
                list.Add(new LinkedTrafficMeters
                {
                    LinkedMeters = meterGroup.OrderBy(x => x.Name).ToList()
                });
            }

            return list;
        }
    }
}