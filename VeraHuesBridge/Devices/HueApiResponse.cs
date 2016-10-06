using System.Collections.Generic;

namespace VeraHuesBridge.Devices
{
    public class HueApiResponse
    {
            public Dictionary<string, DeviceResponse> Lights {get; set;}
    }
}
