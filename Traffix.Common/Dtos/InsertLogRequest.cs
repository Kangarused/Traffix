using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Traffix.Common.Dtos
{
    public class InsertLogRequest
    {
        public string Key { get; set; }
        public int MeterId { get; set; }
        public DateTime Timestamp { get; set; }
        public double Speed { get; set; }
    }
}
