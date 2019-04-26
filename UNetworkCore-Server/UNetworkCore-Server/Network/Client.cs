using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UNetworkCore_Server.Network
{
    public class Client : INetworkExchanger
    {
        #region Private fields
        private Socket _socket;
        #endregion

        #region Public fields
        public Socket Socket { get => _socket; set => _socket = value; }

        public string IP
        {
            get
            {
                return ((IPEndPoint)this.Socket.RemoteEndPoint)?.Address?.ToString() ?? "Unknown";
            }
        }
        #endregion

        public Client(Socket socket)
        {
            Socket = socket;
        }

        public void Send(byte[] data)
        {
            
        }
    }
}
