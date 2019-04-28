using System;
using System.Collections.Generic;
using System.Text;
using UNetworkCore_Protocol.Enums;

namespace UNetworkCore_Server.Handlers
{
    public class MessageIdAttribute : Attribute
    {
        public MessageIdEnum MessageId
        {
            get;
            set;
        }
        public bool AuthenticationRequired
        {
            get;
            set;
        } = true;
        public MessageIdAttribute(MessageIdEnum messageId)
        {
            MessageId = messageId;
        }
    }
}
