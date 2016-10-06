using System;
using System.Collections.Generic;

namespace EchoBridge.Devices
{
    public class DeviceState {
        public bool On {get; set;}
        public int Bri { get; set; } 
        public int Hue { get; set; }
        public int Sat { get; set; }
        public string Effect { get; set; }
        public int Ct { get; set; }
        public string Alert { get; set; }
        public string Colormode { get; set; }
        public bool Reachable { get; set; }
        public List<Double> Xy { get; set; }



        public override string ToString()
        {
            return "DeviceState{" +
                    "on=" + On +
                    ", bri=" + Bri +
                    '}';
        }
    }
}
