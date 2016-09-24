using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Traffix.Common.Utils;

namespace Traffix.Common.Model
{
    [TypescriptEnum]
    public enum CongestionTypes
    {
        [Description("Minimal Congestion")]
        Low = 0,
        [Description("Medium Congestion")]
        Medium = 1,
        [Description("High Congestion")]
        High = 2
    }
}
