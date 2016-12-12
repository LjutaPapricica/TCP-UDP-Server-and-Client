using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpUdpServer
{
    class UdpConnection
    {
        private UdpClient listener;
        private IPEndPoint serverEndPoint;

        public UdpConnection(int port)
        {
            serverEndPoint = new IPEndPoint(IPAddress.Any, port);
            listener = new UdpClient(serverEndPoint);
            listener.BeginReceive(ReceivedDatagram, null);
        }

        public void SendData(byte[] buffer, IPEndPoint clientEndPoint)
        {
            try
            {
                listener.Send(buffer, buffer.Length, clientEndPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection error (SendData): " + e);
            }
        }

        private void ReceivedDatagram(IAsyncResult ar)
        {
            byte[] buffer = new byte[1024];
            buffer = listener.EndReceive(ar, ref serverEndPoint);

            if (Received != null)
            {
                Received(serverEndPoint, buffer);
            }

            listener.BeginReceive(ReceivedDatagram, null);
        }

        public delegate void UdpDatagramReceivedHandler(IPEndPoint sender, byte[] data);

        public event UdpDatagramReceivedHandler Received;
    }
}