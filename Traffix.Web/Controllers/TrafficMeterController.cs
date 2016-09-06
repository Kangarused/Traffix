using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
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
        public async Task<List<TrafficMeter>> GetTrafficMeters()
        {
            var results = await _trafficMeterRepoistory.GetAllAsync();
            return results;
        }

        //[AcceptVerbs("GET")]
        //public Task<List<TrafficMeter>> GetTrafficMetersWithinRadius(string location, int radius)
        //{
        //    GeoCoordinate cords = GetGeoCoordianteFromString(location);
        //    //var distance = 
        //}

        private GeoCoordinate GetGeoCoordianteFromString(string location)
        {
            string[] values = location.Split(',');
            double latitude = Double.Parse(values[0]);
            double longitude = Double.Parse(values[1]);

            return new GeoCoordinate(latitude, longitude);
        } 
    }
}