using System;
using System.Collections;
using System.IO;
using System.Text;

namespace UNetworkCore_Core.IO
{
    public class BigEndianWriter : IDisposable, IDataWriter
    {
        private BinaryWriter m_writer;
        public Stream BaseStream
        {
            get
            {
                return this.m_writer.BaseStream;
            }
        }
        public long BytesAvailable
        {
            get
            {
                return this.m_writer.BaseStream.Length - this.m_writer.BaseStream.Position;
            }
        }
        public long Position
        {
            get
            {
                return this.m_writer.BaseStream.Position;
            }
            set
            {
                this.m_writer.BaseStream.Position = value;
            }
        }
        public byte[] Data
        {
            get
            {
                long position = this.m_writer.BaseStream.Position;
                byte[] array = new byte[this.m_writer.BaseStream.Length];
                this.m_writer.BaseStream.Position = 0L;
                this.m_writer.BaseStream.Read(array, 0, (int)this.m_writer.BaseStream.Length);
                this.m_writer.BaseStream.Position = position;
                return array;
            }
        }

        public BigEndianWriter()
        {
            this.m_writer = new BinaryWriter(new MemoryStream(), Encoding.UTF8);
        }

        public BigEndianWriter(Stream stream)
        {
            this.m_writer = new BinaryWriter(stream, Encoding.UTF8);
        }

        private void WriteBigEndianBytes(byte[] endianBytes)
        {
            for (int i = endianBytes.Length - 1; i >= 0; i--)
            {
                this.m_writer.Write(endianBytes[i]);
            }
        }

        public void WriteShort(short @short)
        {
            m_writer.Write(@short);
        }

        public void WriteInt(int @int)
        {
            m_writer.Write(@int);
        }

        public void WriteLong(long @long)
        {
            m_writer.Write(@long);
        }

        public void WriteUShort(ushort @ushort)
        {
            m_writer.Write(@ushort);
        }

        public void WriteUInt(uint @uint)
        {
            m_writer.Write(@uint);
        }

        public void WriteULong(ulong @ulong)
        {
            m_writer.Write(@ulong);
        }

        public void WriteByte(byte @byte)
        {
            this.m_writer.Write(@byte);
        }

        public void WriteSByte(sbyte @byte)
        {
            this.m_writer.Write(@byte);
        }

        public void WriteFloat(float @float)
        {
            this.m_writer.Write(@float);
        }

        public void WriteBoolean(bool @bool)
        {
            m_writer.Write(@bool);
        }

        public void WriteChar(char @char)
        {
            m_writer.Write(@char);
        }

        public void WriteDouble(double @double)
        {
            m_writer.Write(@double);
        }

        public void WriteSingle(float single)
        {
            m_writer.Write(single);
        }

        public void WriteUTF(string str)
        {
            m_writer.Write((UInt16)str.Length);

            m_writer.Write(Encoding.ASCII.GetBytes(str));
        }

        public void WriteUTFBytes(string str)
        {
            if (str.Length > 0)
                m_writer.Write(Encoding.ASCII.GetBytes(str));
        }

        public void WriteBytes(byte[] data)
        {
            this.m_writer.Write(data);
        }

        public void Seek(int offset)
        {
            this.Seek(offset, SeekOrigin.Begin);
        }

        public void Seek(int offset, SeekOrigin seekOrigin)
        {
            this.m_writer.BaseStream.Seek((long)offset, seekOrigin);
        }

        public void Clear()
        {
            this.m_writer = new BinaryWriter(new MemoryStream(), Encoding.UTF8);
        }

        public void Dispose()
        {
            this.m_writer.Flush();
            this.m_writer.Dispose();
            this.m_writer = null;
        }
    }
}