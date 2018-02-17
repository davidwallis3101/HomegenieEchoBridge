using System.Net;
using System.Net.Sockets;

namespace HGEchoBridge
{
    public class UdpStateInfo
    {
        public UdpStateInfo(UdpClient c, IPEndPoint ep )
        {
            client = c;
            endpoint = ep;
        }

        public UdpClient client;
        public IPEndPoint endpoint;
    }
}
