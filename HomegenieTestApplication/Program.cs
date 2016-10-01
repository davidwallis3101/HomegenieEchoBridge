using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Test
{
    class Program
    {

        static void Main(string[] args)
        {
            VeraHuesBridge.SSDPService svcSSDP = null;
            VeraHuesBridge.WebServer ws;
            bool respondToSSDP = true;

            Console.WriteLine(LocalIPAddress().ToString());

            if (respondToSSDP)
            {
                svcSSDP = new VeraHuesBridge.SSDPService("239.255.255.250",
                                                        1900,
                                                        LocalIPAddress().ToString(),
                                                        8080,
                                                        "aef85303-330a-4eab-b28d-038ac90416ab");  //init on localIP

                svcSSDP.Start();
            }

            string deviceConfigFile = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DeviceConfig.txt");
            ws = new VeraHuesBridge.WebServer(LocalIPAddress().ToString(),
                                             8081,
                                            "aef85303-330a-4eab-b28d-038ac90416ab",
                                            200,
                                            deviceConfigFile);
            ws.Start();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

        }

        static private IPAddress LocalIPAddress()
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
            {
                return null;
            }

            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            return host
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
        }
    }
}
