using System;
using System.Net;
using System.Text;

namespace TcpUdpServer
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
        UdpConnection UDPconnection;
        TcpConnection TCPconnection;

        public Connections()
        {
            TCPconnection = new TcpConnection(3333);
            TCPconnection.SocketAccepted += TCPconnection_SocketAccepted;
            TCPconnection.Start();

            UDPconnection = new UdpConnection(23456);
            UDPconnection.Received += UDPconnection_Received;
        }

        private void TCPconnection_SocketAccepted(object sender, SocketAcceptedEventHandler e)
        {
            Console.WriteLine("TCP Accepted");
            Client client = new Client(e.acceptedSocket);
            client.Received += new Client.ClientReceivedHandler(TCPconnection_ClientReceived);
            client.Disconnected += new Client.ClientDisconnectedHandler(TCPconnection_ClientDisconnected);
        }

        private void TCPconnection_ClientDisconnected(Client sender)
        {
            Console.WriteLine("TCP Disconnected");
        }

        private void TCPconnection_ClientReceived(Client sender, byte[] data)
        {
            Console.WriteLine("Received over TCP");
        }

        private void UDPconnection_Received(IPEndPoint sender, byte[] data)
        {
            Console.WriteLine("Received over UDP");
            Console.WriteLine(BitConverter.ToString(data));
        }

    }
}
