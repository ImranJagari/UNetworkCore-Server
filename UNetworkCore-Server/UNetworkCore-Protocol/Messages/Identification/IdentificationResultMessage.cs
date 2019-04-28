using System;
using System.Collections.Generic;
using System.Text;
using UNetworkCore_Core.IO;
using UNetworkCore_Protocol.Enums;

namespace UNetworkCore_Protocol.Messages.Identification
{
    public class IdentificationResultMessage : NetworkMessage
    {
        public const MessageIdEnum Id = MessageIdEnum.IDENTIFICATION_RESULT;

        public sbyte result;

        public IdentificationResultMessage()
        {
        }

        public IdentificationResultMessage(sbyte result)
        {
            this.result = result;
        }

        public override MessageIdEnum MessageId => Id;

        public override void Deserialize(IDataReader reader)
        {
            result = reader.ReadSByte();
        }

        public override void Serialize(IDataWriter writer)
        {
            writer.WriteSByte(result);
        }
    }
}
