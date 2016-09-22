using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Traffix.Common.Dtos;
using Traffix.Common.Model;
using Traffix.Web.Database.Repositories;
using Traffix.Web.Hubs;
using Traffix.Web.Providers;

namespace Traffix.Web.Controllers
{
    public class UpdateController : ApiController
    {
        private readonly ITrafficMeterRepository _trafficMeterRepoistory;
        private readonly ITrafficLogRepoistory _trafficLogRepoistory;
        private readonly ITraffixUpdateProvider _traffixUpdateProvider;

        public UpdateController(
            ITrafficMeterRepository trafficMeterRepository,
            ITrafficLogRepoistory trafficLogRepoistory, 
            ITraffixUpdateProvider traffixUpdateProvider)
        {
            _trafficMeterRepoistory = trafficMeterRepository;
            _trafficLogRepoistory = trafficLogRepoistory;
            _traffixUpdateProvider = traffixUpdateProvider;
        }

        [AcceptVerbs("GET")]
        public async Task<string> InsertLogForAllMeters()
        {
            var meters = await _trafficMeterRepoistory.GetAllAsync();

            if (meters != null)
            {
                foreach (var meter in meters)
                {
                    if (meter != null)
                    {
                        Random rand = new Random();

                        TrafficLog newLog = new TrafficLog();
                        newLog.MeterId = meter.Id;
                        newLog.Time = DateTime.Now;
                        newLog.Speed = rand.Next(40, 130);

                        await _trafficLogRepoistory.InsertAsync(newLog);

                        _traffixUpdateProvider.MeterUpdated(meter.Region, meter, newLog);
                    }
                }
                return "LOGS ADDED";
            }
                        
            return "METER DOES NOT EXIST";
        }

        [AcceptVerbs("GET")]
        public async Task<string> InsertLogForMeter(int param)
        {
            var meter = await _trafficMeterRepoistory.GetByIdAsync(param);

            if (meter != null)
            {
                Random rand = new Random();

                TrafficLog newLog = new TrafficLog();
                newLog.MeterId = meter.Id;
                newLog.Time = DateTime.Now;
                newLog.Speed = rand.Next(40, 130);

                await _trafficLogRepoistory.InsertAsync(newLog);

                _traffixUpdateProvider.MeterUpdated(meter.Region, meter, newLog);

                return "LOG ADDED";
            }
            return "METER DOES NOT EXIST";
        }
    }
}