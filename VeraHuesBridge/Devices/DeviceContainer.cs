using System.Collections.Generic;

namespace VeraHuesBridge.Devices
{
    public class DeviceContainer
    {
        public List<Device> Content { get; set; }
        public List<string> Facets { get; set; }
        public int TotalElements { get; set; }
        public int TotalPages { get; set; }
        public int Size { get; set; }
        public int Number { get; set; }
        public bool Last { get; set; }
        public int NumberOfElements { get; set; }
        public string Sort { get; set; }
        public bool First { get; set; }

        public DeviceContainer()
        {
            TotalPages = 1;
            Last = true;
            First = true;
            Number = 0;
            TotalElements = 0;
            Size = 0;
            NumberOfElements = 0;
            Facets = new List<string>();
        }

        public DeviceContainer(List<Device> devices)
        {
            Content = devices;
            TotalElements = devices.Count;
            Size = devices.Count;
            NumberOfElements = devices.Count;

            TotalPages = 1;
            Last = true;
            First = true;
            Number = 0;
            Facets = new List<string>();
        }

    }
}
