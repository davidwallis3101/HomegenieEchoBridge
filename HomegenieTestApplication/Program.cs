using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace Test
{
    class Program
    {
        private static Logger Log = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            HGEchoBridge.SSDPService svcSSDP = null;
            HGEchoBridge.WebServer ws;


            Console.WriteLine("You may need to disable the SSDP service and your local firewall if you have issues accepting connections\r\n");
            Console.WriteLine("Enter the ip address of your homegenie instance IE: 192.168.0.1 - This app assumes you havent changed the port that HG runs on from the default of 80.");
            //string apiIpaddress = Console.ReadLine();
            string apiIpaddress = "192.168.0.161";
            Log.Info("Connecting to Homegenie API [{0}] to discover valid devices", apiIpaddress);
            var devices = HomegenieTestApplication.HgHelper.GetDevicesFromHG(apiIpaddress);

            Log.Info("Starting SSDP service");
            try
            {
                svcSSDP = new HGEchoBridge.SSDPService("239.255.255.250", //IP V4 Multicast Address
                    1900,
                    LocalIPAddress().ToString(),
                    8080,
                    "aef85303-330a-4eab-b28d-038ac90416ab");
            }
            catch (Exception)
            {
                Log.Error("Unable to start ssdp server, is UPNP interface or SSDP Service already running?");
                throw;
            }
            

            svcSSDP.Start();

            
            Log.Info("Starting Web Server");
            try
            {
                ws = new HGEchoBridge.WebServer(LocalIPAddress().ToString(),
                    8080,
                    "aef85303-330a-4eab-b28d-038ac90416ab",
                    200,
                    devices);

                ws.Start();
            }
            catch (Exception)
            {
                Log.Error("Unable to start web server, Check permissions or whether port in use.");  
                throw;
            }


            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

        }

        private static IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            var host = Dns.GetHostEntry(Dns.GetHostName());


            //return only Ipv4 Addresses and None APIPA addresses.
            return host
                .AddressList
                .FirstOrDefault(ip => (ip.AddressFamily == AddressFamily.InterNetwork) && ip.ToString().StartsWith("169.254") == false);

        }
    }
}
