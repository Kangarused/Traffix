using T4TS;

namespace Traffix.Common.Dtos
{
    [TypeScriptInterface]
    public class AnimatedTrafficMeter
    {
        public string Animation { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Congestion { get; set; }
        public string DateActive { get; set; }
        public string LinkId { get; set; }
    }
}
