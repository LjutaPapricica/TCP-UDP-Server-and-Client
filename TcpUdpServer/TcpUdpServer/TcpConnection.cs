using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpUdpServer
{
    class SocketAcceptedEventHandler : EventArgs
    {
        public Socket acceptedSocket {
            get;
            private set;
        }

        public SocketAcceptedEventHandler(Socket socket)
        {
            acceptedSocket = socket;
        }
    }

    class TcpConnection
    {
        public event EventHandler<SocketAcceptedEventHandler> SocketAccepted;

        private Socket serverSocket;
        private int port;
        private bool listening;

        public TcpConnection(int _port)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            port = _port;
        }

        public void Start()
        {
            if (listening)
                return;

            serverSocket.Bind(new IPEndPoint(0, port));
            serverSocket.Listen(0);
            serverSocket.BeginAccept(AcceptCallback, null);
            listening = true;
        }

        public void Stop()
        {
            if (!listening)
                return;

            serverSocket.Close();
            serverSocket.Dispose();
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listening = false;
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket socket = serverSocket.EndAccept(ar);

                if (SocketAccepted != null)
                {
                    SocketAccepted(this, new SocketAcceptedEventHandler(socket));
                }

                serverSocket.BeginAccept(AcceptCallback, null);
            }
            catch
            {

            }
        }
    }
}
