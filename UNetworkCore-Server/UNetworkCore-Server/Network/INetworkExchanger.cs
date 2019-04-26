using System;
using System.Collections.Generic;
using System.Text;

namespace UNetworkCore_Server.Network
{
    public interface INetworkExchanger
    {
        void Send(byte[] data);
    }
}
