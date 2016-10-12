using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HGEchoBridge;
using MIG.Interfaces.HomeAutomation.Api;
using NLog;


namespace MIG.Interfaces.HomeAutomation
{

    static class HgHelper
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();

        public static List<Device> GetDevicesFromHg(string ipAddress)
        {
            var api = new ApiHelper();

            // Get available modules from HG
            var modules = api.GetModules(ipAddress);

            // Filter modules to ones we are interested in
            var filteredList = FilterModules(modules); // TODO: Add filter here with enum..

            return GenerateOutput(filteredList, ipAddress);

        }

        private static List<Module> FilterModules(List<Module> modules)
        {
            return modules.Where(module => (module.DeviceType == "Switch") || (module.DeviceType == "Light")).Where(module => module.Name.Length > 0).ToList();
        }

        public static List<Device> GenerateOutput(List<Module> modules, string ipAddress)
        {
            Log.Info("Dynamically generating objects from Homegenie api");

            var deviceList = new List<Device>();

            foreach (var module in modules)
            {
                // create new module object
                var newModule = new Module();

                var device = new Device
                {
                    name = module.Name,
                    offUrl = $"http://{ipAddress}/api/{module.Domain}/{module.Address}/Control.Off",
                    onUrl = $"http://{ipAddress}/api/{module.Domain}/{module.Address}/Control.On",
                    deviceType = "switch"
                };


                var found = false;
                var updateNeeded = false;

                foreach (var property in module.Properties)
                {
                    // Skip the wrong types of properties
                    if (property.Name != "Module.GUID") continue;

                    found = true; //Found existing property

                    // Check guid is valid
                    if (IsGuidValid(property.Value))
                    {
                        Log.Info("Valid Guid found for module {0}", module.Name);
                        // set the gateways device id to that of the module
                        device.id = property.Value;
                    }
                    else // invalid guid
                    {
                        // Copy module and properties and reset the GUID to something valid
                        newModule = module;
                        foreach (var moduleProperty in newModule.Properties.Where(moduleProperty => moduleProperty.Name == "Module.GUID"))
                        {
                            var newGuid = Guid.NewGuid().ToString();
                            Log.Warn("GUID Property found with invalid data, Creating new guid {0} and updating property on module {1}", newGuid, module.Name);
                            moduleProperty.Value = newGuid;
                            moduleProperty.NeedsUpdate = true;
                            moduleProperty.UpdateTime = DateTime.UtcNow.ToString();

                            device.id = property.Value;
                            updateNeeded = true;

                        }
                    }

                } // End of properties

                if (!found) // didnt find an existing property
                {
                    Log.Warn(
                        "Existing GUID Property not Found, Creating new guid and adding property to the module {0}",
                        module.Name);

                    var newGuid = Guid.NewGuid().ToString();

                    // Set the guid for the device
                    device.id = newGuid;

                    //new property
                    var moduleGuidProperty = new ModuleProperties
                    {
                        Name = "Module.GUID",
                        Value = newGuid,
                        NeedsUpdate = true,
                        UpdateTime = DateTime.UtcNow.ToString()
                    };

                    // Convert existing properties to a list so we can append it with the new GUID
                    var newProperties = module.Properties.ToList();
                    newProperties.Add(moduleGuidProperty);

                    // Copy module and add the new properties
                    newModule = module;
                    newModule.Properties = newProperties.ToArray();

                    updateNeeded = true;

                }

                if (updateNeeded) // Send data back to api?
                {
                    var apiHelper = new ApiHelper();

                    if (apiHelper.UpdateModule(ipAddress, newModule))
                    {
                        Log.Debug("Updated module {0} via Homegenie API", module.Name);
                    }
                    else
                    {
                        Log.Error("Failed to update module {0} via Homegenie API", module.Name);
                    }

                }

                deviceList.Add(device);
            }

            return deviceList;
        }

        private static bool IsGuidValid(string myGuid)
            {
            Guid guidOutput;
            var isValid = Guid.TryParse(myGuid, out guidOutput);
            return isValid;
        }

    }
}