using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T4TS;
using Traffix.Common.Model;

namespace Traffix.Common.Dtos
{
    [TypeScriptInterface]
    public class LinkedTrafficMeters
    {
        public List<TrafficMeter> LinkedMeters { get; set; } 
    }
}
