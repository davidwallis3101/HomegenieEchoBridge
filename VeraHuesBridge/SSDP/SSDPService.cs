using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NLog;

namespace VeraHuesBridge.SSDP
{
    public class UdpStateInfo
    {
        public UdpStateInfo(UdpClient c, IPEndPoint ep )
        {
            Client = c;
            Endpoint = ep;
        }
        public UdpClient Client;
        public IPEndPoint Endpoint;
    }
    
    public class SsdpService
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static bool _running;
        private static string _multicastIp;
        private static string _multicastLocalIp;
        private static int _multicastPort;
        public static string Uuid;
        public static int WebServerPort;
        
        private static UdpClient _multicastClient;

        private static byte[] _byteDiscovery;
        public static string DiscoveryResponse;
        //{0}=IPAddress {1}=Port {2}=RandomUUID
        private static readonly string DiscoveryTemplate = "HTTP/1.1 200 OK\r\n" +
            "CACHE-CONTROL: max-age=86400\r\n" +
            "EXT:\r\n" +
            "LOCATION: http://{0}:{1}/api/setup.xml\r\n" +
            "OPT: \"http://schemas.upnp.org/upnp/1/0/\"; ns=01\r\n" +
            "01-NLS: {2}\r\n" +
            "ST: urn:schemas-upnp-org:device:basic:1\r\n" +
            "USN: uuid:Socket-1_0-221438K0100073::urn:Belkin:device:**\r\n\r\n";


        //239.255.255.250   port 1900  10.10.26
        public SsdpService(string multicastIp, int multicastPort, string localIp, int webPort, string uuid)
        {
            Logger.Info("New SSDP Service initiated on IP [{0}], port [{1}]", multicastIp, multicastPort);
            _multicastIp = multicastIp;
            _multicastPort = multicastPort;
            _multicastLocalIp = localIp;
            WebServerPort =webPort;
            Uuid = uuid;
            DiscoveryResponse = string.Format(DiscoveryTemplate, _multicastLocalIp, WebServerPort, Uuid);
            _byteDiscovery = Encoding.ASCII.GetBytes(DiscoveryResponse);
            _running = false;
        }
        public bool Start()
        {
            try
            {
                Logger.Info("Starting SSDP Service on IP [{0}], port [{1}]", _multicastIp, _multicastPort);
                _multicastClient = new UdpClient(_multicastPort);
                var ipSsdp = IPAddress.Parse(_multicastIp);

                Logger.Info("Joining multicast group on IP [{0}]", _multicastLocalIp);
                _multicastClient.JoinMulticastGroup(ipSsdp, IPAddress.Parse(_multicastLocalIp));

                _running = true;

                var udpListener = new UdpStateInfo(_multicastClient, new IPEndPoint(ipSsdp, _multicastPort));

                Logger.Info("Starting Multicast Receiver");
                _multicastClient.BeginReceive(new AsyncCallback(MulticastReceiveCallback), udpListener);
                Logger.Info("SSDP Service started.");
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Error occured starting SSDP service.");
                throw;
            }

            return true;
        }

        public bool IsRunning()
        {
            return _running;
        }

        public void Stop()
        {
            Logger.Info("Stopping SSDP Service...");
            _running = false;
            _multicastClient.Client.Shutdown(SocketShutdown.Both);
            _multicastClient.Close();
            Logger.Info("SSDP Service stopped.");
            
        }
        public static void MulticastReceiveCallback(IAsyncResult ar)
        {
            try
            {
                
                var udpListener = (UdpStateInfo)(ar.AsyncState);
                var client = udpListener.Client;
                var endpoint = udpListener.Endpoint;

                if (client != null)
                {
                    // logger.Info("Received a UDP multicast from IP [{0}], on port [{1}].", endpoint.Address.ToString(), endpoint.Port);
                    var receiveBytes = client.EndReceive(ar, ref endpoint);
                    var receiveString = Encoding.ASCII.GetString(receiveBytes);

                    //todo dw
                    //if (endpoint.Address.ToString() == "192.168.0.193") { logger.Debug("Multicast From: {0}\r\nData:\r\n{1}", endpoint.ToString(), receiveString); }
        
                    //discovery has occured, send our response
                    if (IsSsdpDiscoveryPacket(receiveString))
                    {
                        if (endpoint.Address.ToString() == "192.168.0.193") { Logger.Info("Sending SSDP setup information..."); }

                        //MulticastClient.Send(byteDiscovery, byteDiscovery.Length, endpoint);
                        var winSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


                        winSocket.Connect(endpoint);
                        winSocket.Send(_byteDiscovery);
                        winSocket.Shutdown(SocketShutdown.Both);
                        winSocket.Close();
                        if (endpoint.ToString() == "192.168.0.193") { Logger.Debug(string.Format("Sent Response To: {0}\r\nData:\r\n{1}", endpoint, DiscoveryResponse)); }
                    }
                    else
                    {
                        if (endpoint.ToString() == "192.168.0.193") { Logger.Debug("Not SSDP Packet"); }
                    }

                }
                if (_running)
                {
                    //logger.Info("Restarted Multicast Receiver.");
                    _multicastClient.BeginReceive(new AsyncCallback(MulticastReceiveCallback), udpListener);
                }
                    
            }
            catch (Exception ex)
            {
                
                if (_running)
                {
                    Logger.Warn(ex, "Error occured in MulticastReceiveCallBack.");
                }
                else
                {
                    Logger.Debug(ex, "Ignoring an Error occured in MulticastReceiveCallBack as SSDP service is not running.");
                }
                    
            }
        }

        private static bool IsSsdpDiscoveryPacket(string message)
        {
            //logger.Info("Testing if message is SSDP Discovery Packet...");
            //logger.Info("Examing message [{0}]", message);
            if (message != null && message.StartsWith("M-SEARCH * HTTP/1.1") && message.Contains("MAN: \"ssdp:discover\""))
            {
                // logger.Info("SSDP Discovery Packet detected.");
                return true;
            }
            //logger.Info("SSDP Discovery Packet not detected.");
            return false;

        }

    }


}
