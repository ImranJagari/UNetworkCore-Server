using System;
using System.Collections.Generic;
using System.Text;
using UNetworkCore_Core.IO;
using UNetworkCore_Protocol.Enums;

namespace UNetworkCore_Protocol.Messages.Identification
{
    public class IdentificationRequestMessage : NetworkMessage
    {
        public const MessageIdEnum Id = MessageIdEnum.IDENTIFICATION_REQUEST;

        public string username;
        public string passwordHash;

        public IdentificationRequestMessage()
        {
        }

        public IdentificationRequestMessage(string username, string passwordHash)
        {
            this.username = username ?? throw new ArgumentNullException(nameof(username));
            this.passwordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
        }

        public override MessageIdEnum MessageId => Id;

        public override void Deserialize(IDataReader reader)
        {
            username = reader.ReadUTF();
            passwordHash = reader.ReadUTF();
        }

        public override void Serialize(IDataWriter writer)
        {
            writer.WriteUTF(username);
            writer.WriteUTF(passwordHash);
        }
    }
}
