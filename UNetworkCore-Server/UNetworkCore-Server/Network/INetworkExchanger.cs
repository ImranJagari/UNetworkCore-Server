using System;
using System.Collections.Generic;
using System.Text;
using UNetworkCore_Protocol.Messages;

namespace UNetworkCore_Server.Network
{
    public interface INetworkExchanger
    {
        /// <summary>
        /// Send data to the client connected to the server
        /// </summary>
        /// <param name="message">data must be sended</param>
        void Send(NetworkMessage message);
    }
}
