using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using NLog;
using EchoBridge.SSDP;
using EchoBridge.Webserver;

namespace HomegenieTestApplication
{
    internal class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        private static void Main(string[] args)
        {

            var apiIpaddress = "192.168.0.161";

            _logger.Info("Connecting to Homegenie API [{0}]to discover valid devices");

            var devices = HgHelper.GetDevicesFromHg(apiIpaddress);

            _logger.Info("Starting SSDP service");

            var svcSsdp = new SsdpService("239.255.255.250",
                    1900,
                    LocalIpAddress().ToString(),
                    8080,
                    "aef85303-330a-4eab-b28d-038ac90416ab");

                svcSsdp.Start();


            _logger.Info("Starting Web Server");
            var ws = new WebServer(LocalIpAddress().ToString(),
                8080,
                "aef85303-330a-4eab-b28d-038ac90416ab",
                200,
                devices);

            ws.Start();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
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
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}