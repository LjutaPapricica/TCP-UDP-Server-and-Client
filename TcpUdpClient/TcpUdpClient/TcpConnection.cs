using System;
using System.Net;
using System.Net.Sockets;

namespace TcpUdpClient
{
    class TcpConnection
    {
        private string serverIp;
        private int port;
        private Socket socket;
        private byte[] buffer;

        public TcpConnection(string _serverIp, int _port)
        {
            serverIp = _serverIp;
            port = _port;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect()
        {
            try
            {
                socket.BeginConnect(new IPEndPoint(IPAddress.Parse(serverIp), port), ConnectedCallback, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection error (Connect): " + e);
                Close();
            }
        }

        public void Disconnect()
        {
            Close();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void SendData(byte[] buffer)
        {
            try
            {
                socket.Send(buffer);
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection error (SendData): " + e);
                Close();
            }
        }

        private void Close()
        {
            if (Disconnected != null)
            {
                Disconnected();
            }

            socket.Dispose();
            socket.Close();
        }

        private void ConnectedCallback(IAsyncResult ar)
        {
            try
            {
                if (socket.Connected)
                {
                    if (Connected != null)
                    {
                        Connected();
                    }

                    socket.EndConnect(ar);
                    buffer = new byte[1024];
                    socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceivedCallback, null);
                }
                else {
                    Console.WriteLine("Connection error (ConnectedCallback 1): ");
                    Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Connection error (ConnectedCallback 2): " + e);
                Close();
            }
        }

        private void ReceivedCallback(IAsyncResult ar)
        {
            try
            {
                int bufferSize = socket.EndReceive(ar);
                byte[] packet = new byte[bufferSize];
                Array.Copy(buffer, packet, bufferSize);

                if (Received != null)
                {
                    Received(buffer);
                }

                buffer = new byte[1024];
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceivedCallback, null);

            }
            catch (Exception e)
            {
                Console.WriteLine("Connection error (ReceivedCallback): " + e);
                Close();
            }
        }

        public delegate void TCPDatagramReceivedHandler(byte[] data);
        public delegate void TCPServerConnected();
        public delegate void TCPServerDisconnected();

        public event TCPDatagramReceivedHandler Received;
        public event TCPServerConnected Connected;
        public event TCPServerDisconnected Disconnected;
    }
}