﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpUdpServer
{
    class Client
    {
        private Socket socket;

        public Client(Socket _socket)
        {
            socket = _socket;
            socket.BeginReceive(new byte[] { 0 }, 0, 0, 0, ReceivedCallback, null);
        }

        public void SendData(byte[] buffer)
        {
            try
            {
                socket.Send(buffer);
            }
            catch (Exception e)
            {
                Console.WriteLine("Client connection error (SendData): " + e);
                Close();
            }
        }

        private void Close()
        {
            socket.Dispose();
            socket.Close();
        }

        private void ReceivedCallback(IAsyncResult ar)
        {
            try
            {
                socket.EndReceive(ar);

                byte[] buffer = new byte[1024];
                int receive = socket.Receive(buffer, buffer.Length, 0);

                if (receive < buffer.Length)
                {
                    Array.Resize(ref buffer, receive);
                }

                if (Received != null)
                {
                    Received(this, buffer);
                }

                socket.BeginReceive(new byte[] { 0 }, 0, 0, 0, ReceivedCallback, null);
            }
            catch (Exception e)
            {
                Console.WriteLine("Client connection error: " + e.Message);
                Close();

                if (Disconnected != null)
                {
                    Disconnected(this);
                }
            }
        }

        public delegate void ClientReceivedHandler(Client sender, byte[] data);
        public delegate void ClientDisconnectedHandler(Client sender);

        public event ClientReceivedHandler Received;
        public event ClientDisconnectedHandler Disconnected;
    }
}
