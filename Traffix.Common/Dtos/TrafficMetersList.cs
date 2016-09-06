using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using T4TS;

namespace Traffix.Common.Dtos
{
    [TypeScriptInterface]
    public class TrafficMetersList
    {
        public List<int> MeterIds { get; set; } 
    }
}
