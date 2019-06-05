using System;
using System.Net;
using System.Net.Sockets;

namespace OracControl.Services
{
    public class UDPSender
    {
        public int Port { get; set; }
        public IPAddress Address { get; set; }

        IPEndPoint RemoteIpEndPoint;
        Socket sock;

        public UDPSender(IPAddress address, int port)
        {
            Port = port;
            Address = address;

            sock = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);


            RemoteIpEndPoint = new IPEndPoint(address, port);
        }

        public void Send(byte[] message)
        {
            sock.SendTo(message, RemoteIpEndPoint);
        }

        public void Close()
        {
            sock.Close();
        }
    }
}
