using System.Collections.Generic;

namespace HGEchoBridge
{
    public class DeviceState {
        public bool on {get; set;}
        public int? bri { get; set; }
        public int hue { get; set; }
        public int sat { get; set; }
        public string effect { get; set; }
        public int ct { get; set; }
        public string alert { get; set; }
        public string colormode { get; set; }
        public bool reachable { get; set; }
        public List<double> xy { get; set; }



        public override string ToString()
        {
            return "DeviceState{" +
                    "on=" + on +
                    ", bri=" + bri +
                    '}';
        }
    }
}
