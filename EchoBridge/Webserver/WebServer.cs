using System;
using System.Collections.Generic;
using Microsoft.Owin.Hosting;
using NLog;

namespace HGEchoBridge
{
    public class WebServer
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IDisposable webApplication;

        public WebServer(string ipAddress, int port, string uuid, int defaultIntensity, List<Device> deviceObj)
        {
            logger.Info("New webserver initiated.");
            Globals.IPAddress = ipAddress;
            Globals.Port = port;
            Globals.BaseAddress = "http://" + Globals.IPAddress + ":" + Globals.Port + "/";
            Globals.UUID = uuid;
            Globals.DefaultIntensity = defaultIntensity;

            Globals.DeviceList = new Devices(deviceObj);
            logger.Info("Webserver created.  DeviceConfig holds [{0}] device(s)", Globals.DeviceList.Count());
        }

        public void Start()
        {
            logger.Info("Webserver starting up, listening on {0}", Globals.BaseAddress);
            webApplication = WebApp.Start<WebServerStartup>(Globals.BaseAddress);
            logger.Info("Webserver started.");
        }

        public void Stop()
        {
            logger.Info("Webserver stopping...");
            webApplication.Dispose();
            logger.Info("Webserver stopped.");
        }
    }
}
