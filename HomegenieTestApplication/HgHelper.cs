using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HomegenieTestApplication.Api;

namespace HomegenieTestApplication
{

    static class HgHelper
    {

        public static List<VeraHuesBridge.Device> GetDevicesFromHG(string ipAddress)
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
            return modules.Where(module => (module.DeviceType == "Switch") || (module.DeviceType == "Light")).Where(module => module.Name.Length > 0).ToList();
        }

        public static List<VeraHuesBridge.Device> GenerateOutput(List<Api.Module> modules, string ipAddress)
        {
            List<VeraHuesBridge.Device> deviceList = new List<VeraHuesBridge.Device>();

            foreach (var module in modules)
            {
                // create new module object
                var newModule = new Api.Module();

                var device = new VeraHuesBridge.Device
                {
                    name = module.Name,
                    offUrl = $"http://{ipAddress}/api/{module.Domain}/{module.Address}/Off",
                    onUrl = $"http://{ipAddress}/api/{module.Domain}/{module.Address}/On",
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
                        Console.WriteLine("Valid Guid found for module {0}, property Name: {1}",module.Name, property.Name);
                        // set the gateway id to that of the module
                        device.id = property.Value;
                    }
                    else // invalid guid
                    {
                        // Copy module and properties and reset the GUID to something valid
                        newModule = module;
                        foreach (var moduleProperty in newModule.Properties.Where(moduleProperty => moduleProperty.Name == "Module.GUID"))
                        {
                            var newGuid = Guid.NewGuid().ToString();
                            Console.WriteLine("GUID Property found with invalid data, Creating new guid {0} and updating property on module {1}", newGuid, module.Name);
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
                    Console.WriteLine(
                        "Existing GUID Property not Found, Creating new guid and adding property to the module {0}",
                        module.Name);

                    var newGuid = Guid.NewGuid().ToString();

                    // Set the guid for the device
                    device.id = newGuid;

                    //new property
                    var moduleGuidProperty = new Api.ModuleProperties
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
                        Console.WriteLine("Updated module {0}",module.Name);
                    }
                    else
                    {
                        Console.WriteLine("Failed to update module {0}", module.Name);
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