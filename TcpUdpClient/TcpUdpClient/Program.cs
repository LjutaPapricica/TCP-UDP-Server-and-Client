using System;
using System.Net;
using System.Text;

namespace TcpUdpClient
{
    class Program
    {
        static void Main(string[] args)
        {
            new Connections();

            System.Diagnostics.Process.GetCurrentProcess().WaitForExit();
        }
    }

    class Connections
    {
        TcpConnection TCPconnection;
        UdpConnection UDPconnection;

        public Connections()
        {
            TCPconnection = new TcpConnection("127.0.0.1", 3333);
            TCPconnection.Connected += TCPconnection_ServerConnected;
            TCPconnection.Disconnected += TCPconnection_ServerDisconnected;
            TCPconnection.Received += TCPconnection_Received;
            TCPconnection.Connect();

            UDPconnection = new UdpConnection("213.46.14.31", 23456);
            UDPconnection.Received += UDPconnection_Received;

            while (true)
            {
                string msg = Console.ReadLine();
                UDPconnection.SendData(Encoding.ASCII.GetBytes(msg));
            }

        }

        private void TCPconnection_ServerConnected()
        {
            Console.WriteLine("TCP Connected");
        }

        private void TCPconnection_ServerDisconnected()
        {
            Console.WriteLine("TCP Disconnected");
        }

        private void TCPconnection_Received(byte[] data)
        {
            Console.WriteLine("Received over TCP");
        }

        private void UDPconnection_Received(IPEndPoint sender, byte[] data)
        {
            Console.WriteLine("Received over UDP");
        }
    }
}