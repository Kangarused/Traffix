using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traffix.Migrations.Profiles.MockData
{
    public class MockTrafficMeter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Congestion { get; set; }
        public DateTime DateActive { get; set; }
        public string LinkId { get; set; }
        public int SpeedLimit { get; set; }
    }
}
