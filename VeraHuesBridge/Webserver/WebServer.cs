using System;
using System.Collections.Generic;
using Microsoft.Owin.Hosting;
using NLog;
using VeraHuesBridge.Devices;

namespace VeraHuesBridge.Webserver
{
    public  class WebServer
    {

       private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        
        private IDisposable _webApplication;

         public WebServer(){
             Logger.Info("New webserver initiated");
         }

        public WebServer(string ipAddress, int port, string uuid, int defaultIntensity, List<Device>deviceObj)
        {
            Globals.IpAddress = ipAddress;
            Globals.Port = port;
            Globals.BaseAddress = "http://" + Globals.IpAddress + ":" + Globals.Port + "/";
            Globals.Uuid = uuid;
            Globals.DefaultIntensity = defaultIntensity;

            Globals.DeviceList = new Devices.Devices(deviceObj);
            Logger.Info("Webserver created. Config holds [{0}] device(s)", Globals.DeviceList.Count());

        }


        public void Start()
        {
            Logger.Info("Webserver starting up, listening on {0}", Globals.BaseAddress);
            _webApplication = WebApp.Start<WebServerStartup>(url: Globals.BaseAddress);
            Logger.Info("Webserver started");
        }

        public void Stop()
        {
            Logger.Info("Webserver stopping");
            _webApplication.Dispose();
            Logger.Info("Webserver stopped");
        }
  }
}
