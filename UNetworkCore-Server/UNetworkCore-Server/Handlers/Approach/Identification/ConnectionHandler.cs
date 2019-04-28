using System;
using System.Collections.Generic;
using System.Text;
using UNetworkCore_Protocol.Enums;
using UNetworkCore_Protocol.Messages.Identification;
using UNetworkCore_Server.Network;

namespace UNetworkCore_Server.Handlers.Approach.Identification
{
    public class ConnectionHandler
    {
        [MessageId(IdentificationRequestMessage.Id)]
        public static void HandleIdentificationRequestMessage(Client client, IdentificationRequestMessage message)
        {
            client.Send(new IdentificationResultMessage((sbyte)IdentificationResultEnum.OK));
        }
    }
}
