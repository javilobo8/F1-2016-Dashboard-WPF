using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace F1Dashboard
{
    class UDPServer
    {
        public static int PORT = 20776;
        public static string HOST = "127.0.0.1";

        private byte[] data;
        private IPEndPoint ipep;
        private UdpClient newsock;
        private ConvertMethod OnPacketReceived;
        private Thread listenThread;

        public UDPServer(ConvertMethod OnPacketReceived)
        {
            data = new byte[1024];
            this.OnPacketReceived = OnPacketReceived;
            ipep = new IPEndPoint(IPAddress.Parse(HOST), PORT);
            newsock = new UdpClient(ipep);
            Console.WriteLine("Init UDPServer");
        }

        public void Listen()
        {
            listenThread = new Thread(() => ListenLoop());
            listenThread.Start();
        }

        public void ListenLoop()
        {
            while (true)
            {
                OnPacketReceived(newsock.Receive(ref ipep));
            }
        }
    }
}
