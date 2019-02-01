using System;
using System.Collections.Generic;
using System.Linq;
using HGEchoBridge;
using MIG.Interfaces.HomeAutomation.Api;
using NLog;

namespace MIG.Interfaces.HomeAutomation
{
    static class HgHelper
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        public static List<Device> GetDevicesFromHg(string hgEndpoint)
        {
            try
            {
                var api = new HgApiHelper(hgEndpoint);
                var modules = api.GetModules();
                var filteredModules = FilterModules(modules); // TODO: Add filter here with enum..

                return GenerateDevicesFromModules(filteredModules, hgEndpoint);
            }
            catch (Exception e)
            {
                Log.Error(e);
                throw;
            }
        }

        private static List<Module> FilterModules(IEnumerable<Module> modules)
        {
            var validDeviceTypes = new List<string> {"Switch", "Dimmer", "Light"};
            return modules.Where(x => x.Name.Length > 0 &&
                                      validDeviceTypes.Contains(x.DeviceType,
                                          StringComparer.InvariantCultureIgnoreCase))
                .ToList();
        }

        private static List<Device> GenerateDevicesFromModules(IEnumerable<Module> modules, string hgEndpoint)
        {
            Log.Info("Dynamically generating objects from Homegenie api");

            var deviceList = new List<Device>();
            foreach (var module in modules)
            {
                var device = CreateDevice(module, hgEndpoint);
                deviceList.Add(device);
            }
            return deviceList;
        }

        private static Device CreateDevice(Module module, string hgEndpoint)
        {
            var device = new Device
            {
                name = module.Name,
                offUrl = $"http://{hgEndpoint}/api/{module.Domain}/{module.Address}/Control.Off",
                onUrl = $"http://{hgEndpoint}/api/{module.Domain}/{module.Address}/Control.On",
                DimUrl = module.DeviceType == "Dimmer"
                    ? $"http://{hgEndpoint}/api/{module.Domain}/{module.Address}/Control.Level/{Device.INTENSITY_PERCENT}"
                    : null,
                deviceType = "switch"
            };

            var guidProperty = module.Properties.SingleOrDefault(x => x.Name == "Module.GUID");
            if (guidProperty == null)
            {
                Log.Warn(
                    "Existing GUID Property not Found, Creating new guid and adding property to the module {0}",
                    module.Name);

                var newGuid = Guid.NewGuid().ToString();

                //new property
                guidProperty = new ModuleProperties
                {
                    Name = "Module.GUID",
                    Value = newGuid,
                    NeedsUpdate = true,
                    UpdateTime = DateTime.UtcNow.ToString()
                };

                // Convert existing properties to a list so we can append it with the new GUID
                var newProperties = module.Properties.ToList();
                newProperties.Add(guidProperty);

                // Copy module and add the new properties
                var newModule = module;
                newModule.Properties = newProperties.ToArray();
                UpdateModule(newModule, hgEndpoint);
            }
            else
            {
                if (IsGuidValid(guidProperty.Value))
                {
                    Log.Info("Valid Guid found for module {0}", module.Name);
                }
                else // invalid guid
                {
                    // Copy module and properties and reset the GUID to something valid
                    var newGuid = Guid.NewGuid().ToString();
                    Log.Warn(
                        "GUID Property found with invalid data, Creating new guid {0} and updating property on module {1}",
                        newGuid, module.Name);
                    guidProperty.Value = newGuid;
                    guidProperty.NeedsUpdate = true;
                    guidProperty.UpdateTime = DateTime.UtcNow.ToString();
                    var newModule = module;
                    UpdateModule(newModule, hgEndpoint);
                }
            }

            device.id = guidProperty.Value;
            return device;
        }

        private static void UpdateModule(Module newModule, string hgEndpoint)
        {
            var apiHelper = new HgApiHelper(hgEndpoint);
            if (apiHelper.UpdateModule(newModule))
            {
                Log.Debug("Updated module {0} via Homegenie API", newModule.Name);
            }
            else
            {
                Log.Error("Failed to update module {0} via Homegenie API", newModule.Name);
            }
        }

        private static bool IsGuidValid(string myGuid)
        {
            Guid guidOutput;
            var isValid = Guid.TryParse(myGuid, out guidOutput);
            return isValid;
        }
    }
}
