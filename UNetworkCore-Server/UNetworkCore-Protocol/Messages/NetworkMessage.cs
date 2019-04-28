using System;
using System.Collections.Generic;
using System.Text;
using UNetworkCore_Core.IO;
using UNetworkCore_Protocol.Enums;

namespace UNetworkCore_Protocol.Messages
{
    public abstract class NetworkMessage : IDisposable
    {
        private const byte BIT_RIGHT_SHIFT_LEN_PACKET_ID = 2;
        private const byte BIT_MASK = 3;

        public abstract MessageIdEnum MessageId { get; }

        public void Dispose()
        {
        }

        public void Unpack(IDataReader reader)
        {
            Deserialize(reader);
        }

        public void Pack(IDataWriter writer)
        {
            byte typeLen = 3;
            var header = (short)SubComputeStaticHeader(MessageId, typeLen);

            writer.WriteShort(header);

            for (int i = typeLen - 1; i >= 0; i--)
            {
                writer.WriteByte(0);
            }

            Serialize(writer);
            var len = writer.Position - 5;
            writer.Seek(2);

            for (int i = typeLen - 1; i >= 0; i--)
            {
                writer.WriteByte((byte)(len >> 8 * i & 255));
            }

            writer.Seek((int)len + 5);
        }

        public abstract void Serialize(IDataWriter writer);
        public abstract void Deserialize(IDataReader reader);

        private static byte ComputeTypeLen(int param1)
        {
            byte result;
            if (param1 > 65535)
            {
                result = 3;
            }
            else
            {
                if (param1 > 255)
                {
                    result = 2;
                }
                else
                {
                    if (param1 > 0)
                    {
                        result = 1;
                    }
                    else
                    {
                        result = 0;
                    }
                }
            }
            return result;
        }

        private static uint SubComputeStaticHeader(MessageIdEnum id, byte typeLen)
        {
            return (uint)id << BIT_RIGHT_SHIFT_LEN_PACKET_ID | typeLen;
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}