/*
 *     Author: David Wallis david@wallis2000.co.uk
 *     Version 1.0 - 19/04/2016 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using HGEchoBridge;
using MIG.Config;
using NLog;
using NLog.Fluent;

namespace MIG.Interfaces.HomeAutomation
{

    public class EchoBridge : MigInterface
    {

        List<InterfaceModule> interfaceModules;
        public static Logger Log = LogManager.GetCurrentClassLogger();
        public bool IsEnabled { get; set; }
        public bool IsConnected { get; private set; }
        public List<Option> Options { get; set; }
        
        public event InterfaceModulesChangedEventHandler InterfaceModulesChanged;
        public event InterfacePropertyChangedEventHandler InterfacePropertyChanged;

        public EchoBridge()
        {
            //var debugEvent = this.GetDomain() + "\t" + "Source" + "\t" + "Description" + "\t" + "Property" + "\t" + "Value";
        }
        
        public void OnSetOption(Option option)
        {
            if (IsEnabled)
                Connect();
        }

        public List<InterfaceModule> GetModules()
        {
            return interfaceModules;
        }
      
        public bool IsDevicePresent()
        {
            return true;
        }

        public bool Connect()
        {
            if (!IsConnected)
            {
                
                Log.Info("Starting HGEchoBrige Version {0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());

                // create a list to hold the modules 
                interfaceModules = new List<InterfaceModule>();

                var ipAddress = LocalIpAddress().ToString();
                
                //todo allow non standard port
                //var hgIpAddress = "192.168.0.161";
                //var hgIpAddress = ipAddress;

                MigService.Log.Info("Mig: Connecting to Homegenie API [{0}] to discover valid devices", ipAddress);
                Log.Info("Log: Connecting to Homegenie API [{0}] to discover valid devices", ipAddress);
                List<Device> devices = HgHelper.GetDevicesFromHg(ipAddress);
                              
                MigService.Log.Info("Starting SSDP service");
                var svcSsdp = new SSDPService("239.255.255.250",

                        1900,
                        ipAddress,
                        8080,
                        "aef85303-330a-4eab-b28d-038ac90416ab");

                svcSsdp.Start();

                MigService.Log.Info("Starting Web Server");
                var ws = new WebServer(ipAddress,
                    8080,
                    "aef85303-330a-4eab-b28d-038ac90416ab",
                    200,
                    devices);

                ws.Start();
             
                IsConnected = true;
            }
            OnInterfaceModulesChanged(this.GetDomain());
            return true;
        }


        public void Disconnect()
        {
            if (IsConnected)
            {
                MigService.Log.Debug("Echo Bridge Disconnecting");
                // TODO: ...
                IsConnected = false;
            }
        }

        public void Dispose()
        {
            Disconnect();
        }

        public object InterfaceControl(MigInterfaceCommand request)
        {
            var response = new ResponseText("OK"); //default success value

            //Commands command;
            //Enum.TryParse<Commands>(request.Command.Replace(".", "_"), out command);

            //var module = interfaceModules.Find (m => m.Address.Equals (request.Address));

            //if (module != null) {
            //    switch (command) {
            //    case Commands.Control_On:
            //    // TODO: ...
            //        OnInterfacePropertyChanged (this.GetDomain (), request.Address, "Test Interface", "Status.Level", 1);
            //        break;
            //    case Commands.Control_Off:
            //        OnInterfacePropertyChanged (this.GetDomain (), request.Address, "Test Interface", "Status.Level", 0);
            //    // TODO: ...
            //        break;
            //    case Commands.Temperature_Get:
            //        OnInterfacePropertyChanged (this.GetDomain (), request.Address, "Test Interface", "Sensor.Temperature", 19.75);
            //    // TODO: ...
            //        break;
            //    case Commands.Greet_Hello:
            //    // TODO: ...
            //        OnInterfacePropertyChanged (this.GetDomain (), request.Address, "Test Interface", "Sensor.Message", string.Format ("Hello {0}", request.GetOption (0)));
            //        response = new ResponseText ("Hello World!");
            //        break;
            //    }
            //}
            //else 
            //{
            //    response = new ResponseText ("ERROR: invalid module address");
            //}

            return response;
        }

        protected virtual void OnInterfaceModulesChanged(string domain)
        {
            if (InterfaceModulesChanged == null) return;
            var args = new InterfaceModulesChangedEventArgs(domain);
            InterfaceModulesChanged(this, args);
        }

        protected virtual void OnInterfacePropertyChanged(string domain, string source, string description, string propertyPath, object propertyValue)
        {
            if (InterfacePropertyChanged == null) return;
            var args = new InterfacePropertyChangedEventArgs(domain, source, description, propertyPath, propertyValue);
            InterfacePropertyChanged(this, args);
        }
   
        private static IPAddress LocalIpAddress()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            var host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => (ip.AddressFamily == AddressFamily.InterNetwork) && ip.ToString().StartsWith("169.254") == false);
        }


    }


}

