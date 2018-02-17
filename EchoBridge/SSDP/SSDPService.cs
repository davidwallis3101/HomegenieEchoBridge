using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using NLog;

namespace HGEchoBridge
{
    public class SSDPService
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private static bool _running;
        private static string _multicastIp;
        private static string _multicastLocalIp;
        private static int _multicastPort;

        public static string UUID;
        public static int WebServerPort;
        
        private static UdpClient _multicastClient;
        private static byte[] _byteDiscovery;
        public static string DiscoveryResponse;

        //{0}=IPAddress {1}=Port {2}=RandomUUID
        private const string DiscoveryTemplate = "HTTP/1.1 200 OK\r\n" +
            "CACHE-CONTROL: max-age=86400\r\n" +
            "EXT:\r\n" +
            "LOCATION: http://{0}:{1}/api/setup.xml\r\n" +
            "OPT: \"http://schemas.upnp.org/upnp/1/0/\"; ns=01\r\n" +
            "01-NLS: {2}\r\n" +
            "ST: urn:schemas-upnp-org:device:basic:1\r\n" +
            "USN: uuid:Socket-1_0-221438K0100073::urn:Belkin:device:**\r\n\r\n";


        //239.255.255.250   port 1900  10.10.26
        public SSDPService(string multicastIp, int multicastPort, string localIp, int webPort, string uuid)
        {
            logger.Info("New SSDP Service initiated on IP [{0}], port [{1}]", multicastIp, multicastPort);
            _multicastIp = multicastIp;
            _multicastPort = multicastPort;
            _multicastLocalIp = localIp;
            WebServerPort =webPort;
            UUID = uuid;
            DiscoveryResponse = string.Format(DiscoveryTemplate, _multicastLocalIp, WebServerPort, UUID);
            _byteDiscovery = Encoding.ASCII.GetBytes(DiscoveryResponse);
            _running = false;
        }

        public bool Start()
        {
            try
            {
                logger.Info("Starting SSDP Service on IP [{0}], port [{1}]...", _multicastIp, _multicastPort);
                _multicastClient = new UdpClient(_multicastPort);
                var ssdpIp = IPAddress.Parse(_multicastIp);

                logger.Info("Joining multicast group on IP [{0}]...", _multicastLocalIp);
                _multicastClient.JoinMulticastGroup(ssdpIp, IPAddress.Parse(_multicastLocalIp));

                _running = true;

                var udpListener = new UdpStateInfo(_multicastClient, new IPEndPoint(ssdpIp, _multicastPort));

                logger.Info("Starting Multicast Receiver...");
                _multicastClient.BeginReceive(MulticastReceiveCallback, udpListener);
                logger.Info("SSDP Service started.");
            }
            catch (Exception ex)
            {
                logger.Warn(ex, "Error occured starting SSDP service.");
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
            logger.Info("Stopping SSDP Service...");
            _running = false;
            _multicastClient.Client.Shutdown(SocketShutdown.Both);
            _multicastClient.Close();
            logger.Info("SSDP Service stopped.");
            
        }

        private static void MulticastReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var udpListener = (UdpStateInfo)ar.AsyncState;
                var client = udpListener.client;
                var endpoint = udpListener.endpoint;

                if (client != null)
                {
                    //logger.Info("Received a UDP multicast from IP [{0}], on port [{1}].", endpoint.Address.ToString(), endpoint.Port);
                    byte[] receiveBytes = client.EndReceive(ar, ref endpoint);
                    string receiveString = Encoding.ASCII.GetString(receiveBytes);

                    //todo dw
  
                    //discovery has occured, send our response
                    if (IsSsdpDiscoveryPacket(receiveString))
                    {
                        logger.Debug("Sending SSDP setup information to {0}",endpoint); 

                        //MulticastClient.Send(byteDiscovery, byteDiscovery.Length, endpoint);
                        using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                        {
                            socket.Connect(endpoint);
                            socket.Send(_byteDiscovery);
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Close();
                        }
                    }
                }

                if (_running)
                {
                    //logger.Info("Restarted Multicast Receiver.");
                    _multicastClient.BeginReceive(MulticastReceiveCallback, udpListener);
                }
                    
            }
            catch (Exception ex)
            {
                if (_running)
                {
                    logger.Warn(ex, "Error occured in MulticastReceiveCallBack.");
                }
                else
                {
                    logger.Debug(ex, "Ignoring an Error occured in MulticastReceiveCallBack as SSDP service is not running.");
                }
            }
        }

        private static bool IsSsdpDiscoveryPacket(string message)
        {
            //logger.Info("Testing if message is SSDP Discovery Packet...");
            //logger.Info("Examing message [{0}]", message);
            if (message != null && message.StartsWith("M-SEARCH * HTTP/1.1") && message.Contains("MAN: \"ssdp:discover\""))
            {
                logger.Info("SSDP Discovery Packet detected.");
                //logger.Debug(message);
                return true;
            }
            //logger.Info("SSDP Discovery Packet not detected.");
            return false;
        }
    }
}
