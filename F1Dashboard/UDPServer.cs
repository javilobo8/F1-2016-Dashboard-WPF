using System;
using System.Net;
using System.Net.Sockets;

namespace F1Dashboard
{
    class UDPServer
    {
        public static int PORT = 20776;
        public static IPAddress HOST = IPAddress.Parse("127.0.0.1");

        private byte[] data;
        private IPEndPoint ipep;
        private UdpClient newsock;
        private ConvertMethod OnPacketReceived;

        public UDPServer(ConvertMethod OnPacketReceived)
        {
            data = new byte[1024];
            this.OnPacketReceived = OnPacketReceived;
            ipep = new IPEndPoint(HOST, PORT);
            newsock = new UdpClient(ipep);
            Console.WriteLine("Init UDPServer");
        }

        public void listen()
        {
            Console.WriteLine("Waiting for a connection");
            while (true) OnPacketReceived(newsock.Receive(ref ipep));
        }
    }
}
