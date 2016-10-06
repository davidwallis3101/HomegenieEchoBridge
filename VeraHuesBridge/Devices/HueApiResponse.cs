using System.Collections.Generic;

namespace EchoBridge.Devices
{
    public class HueApiResponse
    {
            public Dictionary<string, DeviceResponse> Lights {get; set;}
    }
}
