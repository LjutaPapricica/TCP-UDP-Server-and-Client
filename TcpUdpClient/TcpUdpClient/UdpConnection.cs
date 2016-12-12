using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpUdpClient
{
    class UdpConnection
    {
        private string serverIp;
        private int port;
        private UdpClient sender;
        private IPEndPoint serverEndPoint;
        private IPEndPoint clientEndPoint;

        public UdpConnection(string _serverIp, int _port)
        {
            serverIp = _serverIp;
            port = _port;

            clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), port);

            //endPoint = new IPEndPoint()
            sender = new UdpClient(clientEndPoint);
            sender.BeginReceive(ReceivedDatagram, null);
        }
        
        public void SendData(byte[] buffer)
        {
            
            try
            {
                sender.Send(buffer, buffer.Length, serverEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection error (SendData): " + e);
                Close();
            }
        }

        private void Close()
        {
            sender.Close();
        }

        private void ReceivedDatagram(IAsyncResult ar)
        {
            byte[] buffer = new byte[1024];
            buffer = sender.EndReceive(ar, ref clientEndPoint);

            if (Received != null)
            {
                Received(serverEndPoint, buffer);
            }

            sender.BeginReceive(ReceivedDatagram, null);
        }

        public delegate void UdpDatagramReceivedHandler(IPEndPoint sender, byte[] data);

        public event UdpDatagramReceivedHandler Received;
    }
}