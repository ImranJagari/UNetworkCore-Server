using System;
using System.Collections.Generic;
using System.Text;

namespace UNetworkCore_Server.Network
{
    public interface INetworkExchanger
    {
        /// <summary>
        /// Send data to the client connected to the server
        /// </summary>
        /// <param name="data">data must be sended</param>
        void Send(byte[] data);
    }
}
