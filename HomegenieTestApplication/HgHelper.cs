using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomegenieTestApplication
{

    static class HgHelper
    {

        public static List<VeraHuesBridge.Device> GetDevicesFromHG(string fileName, string ipAddress)
        {
            var api = new Api.ApiHelper();

            // Get available modules from HG
            var modules = api.GetModules(ipAddress);
            
            // Filter modules to ones we are interested in
            var filteredList = FilterModules(modules); // TODO: Add filter here with enum..

            return GenerateOutput(filteredList, ipAddress);

        }

        private static List<Api.Module> FilterModules(List<Api.Module> modules)
        {
            List<Api.Module> filteredList = new List<Api.Module>();

            foreach (var module in modules)
            {
                if ((module.DeviceType == "Switch") || (module.DeviceType== "Light"))
                {
                    if (module.Name.Length > 0) { filteredList.Add(module); }
                }

            }

            return filteredList;
        }

        public static List<VeraHuesBridge.Device> GenerateOutput(List<Api.Module> modules, string ipAddress)
        {
            List<VeraHuesBridge.Device> deviceList = new List<VeraHuesBridge.Device>();

            foreach (var module in modules)
            {
                var device = new VeraHuesBridge.Device();
                device.name = module.Name;
                device.id = Guid.NewGuid().ToString();
                device.offUrl = $"http://{ipAddress}/api/{module.Domain}/{module.Address}/Off";
                device.onUrl = $"http://{ipAddress}/api/{module.Domain}/{module.Address}/On";
                device.deviceType = "switch";
                deviceList.Add(device);      
            }

            return deviceList;
        }
    }
}
