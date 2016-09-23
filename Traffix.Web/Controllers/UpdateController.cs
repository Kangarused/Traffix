using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using ServiceStack;
using Traffix.Common.Dtos;
using Traffix.Common.IocAttributes;
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
        private readonly IAuthRepository _authRepository;

        public UpdateController(
            ITrafficMeterRepository trafficMeterRepository,
            ITrafficLogRepoistory trafficLogRepoistory, 
            ITraffixUpdateProvider traffixUpdateProvider,
            IAuthRepository authRepository)
        {
            _trafficMeterRepoistory = trafficMeterRepository;
            _trafficLogRepoistory = trafficLogRepoistory;
            _traffixUpdateProvider = traffixUpdateProvider;
            _authRepository = authRepository;
        }

    
        [HttpPost]
        [RequireHttps]
        public async Task<string> InsertLog(InsertLogRequest data)
        {
            var auth = await _authRepository.GetByIdAsync(1);
            if (auth.Secret == data.Key)
            {
                var meter = await _trafficMeterRepoistory.GetByIdAsync(data.MeterId);
                if (meter != null)
                {
                    TrafficLog newLog = new TrafficLog
                    {
                        MeterId = meter.Id,
                        Time = data.Timestamp,
                        Speed = (int)data.Speed
                    };

                    await _trafficLogRepoistory.InsertAsync(newLog);

                    _traffixUpdateProvider.MeterUpdated(meter.Region, meter, newLog);
                    return "LOG ADDED";
                }
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            throw new HttpResponseException(HttpStatusCode.Unauthorized);
        }

        [HttpGet]
        [RequireHttps]
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

                        TrafficLog newLog = new TrafficLog
                        {
                            MeterId = meter.Id,
                            Time = DateTime.Now,
                            Speed = rand.Next(40, 130)
                        };

                        await _trafficLogRepoistory.InsertAsync(newLog);

                        _traffixUpdateProvider.MeterUpdated(meter.Region, meter, newLog);
                    }
                }
                return "LOGS ADDED";
            }
            throw new HttpResponseException(HttpStatusCode.BadRequest);
        }

        [HttpGet]
        [RequireHttps]
        public async Task<string> InsertLogForMeter(int param)
        {
            var meter = await _trafficMeterRepoistory.GetByIdAsync(param);

            if (meter != null)
            {
                Random rand = new Random();

                TrafficLog newLog = new TrafficLog
                {
                    MeterId = meter.Id,
                    Time = DateTime.Now,
                    Speed = rand.Next(40, 130)
                };

                await _trafficLogRepoistory.InsertAsync(newLog);

                _traffixUpdateProvider.MeterUpdated(meter.Region, meter, newLog);

                return "LOG ADDED";
            }
            throw new HttpResponseException(HttpStatusCode.BadRequest);
        }
    }
}