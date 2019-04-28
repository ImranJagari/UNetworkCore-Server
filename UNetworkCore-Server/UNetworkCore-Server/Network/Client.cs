using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UNetworkCore_Core.IO;
using UNetworkCore_Core.Reflection;
using UNetworkCore_Protocol;
using UNetworkCore_Protocol.Enums;
using UNetworkCore_Protocol.Messages;
using UNetworkCore_Server.Handlers;

namespace UNetworkCore_Server.Network
{
    /// <summary>
    /// Client abstraction
    /// </summary>
    public class Client : INetworkExchanger, IDisposable
    {
        #region Private fields
        private Socket _socket;
        private byte[] _buffer;
        private short _maxBufferLength = 8096;
        private MessagePart _currentMessage;
        private BigEndianReader _reader;
        #endregion

        #region Public fields
        public Socket Socket { get => _socket; set => _socket = value; }
        public string IP => Socket.GetIPString();

        public byte[] Buffer { get => _buffer; set => _buffer = value; }
        public MessagePart CurrentMessage { get => _currentMessage; set => _currentMessage = value; }
        public BigEndianReader Reader { get => _reader; set => _reader = value; }
        #endregion

        #region Events
        private delegate void DataProcessedEvent(MessagePart messagePart);

        public delegate void ClientDisconnectedEvent(Client client);

        private event DataProcessedEvent OnDataProcessed;
        public event ClientDisconnectedEvent OnClientDisconnected;

        private event AsyncCallback OnDataReceived;
        private event AsyncCallback OnDisconnectionProcessed;
        #endregion

        public Client(Socket socket)
        {
            Buffer = new byte[_maxBufferLength];
            OnDataReceived += Client_ProcessDataReceived;
            OnDisconnectionProcessed += Client_ProcessDisconnection;
            OnDataProcessed += Client_ProcessMessagePart;

            Socket = socket;
            Socket.BeginReceive(Buffer, 0, _maxBufferLength, SocketFlags.None, OnDataReceived, Socket);
        }

        public void Send(NetworkMessage message)
        {
            using (BigEndianWriter writer = new BigEndianWriter())
            {
                message.Pack(writer);
                Send(writer.Data);
            }
        }
        /// <inheritdoc/>
        private void Send(byte[] data)
        {
            Socket.Send(data);
        }

        public void Disconnect()
        {
            Socket.BeginDisconnect(false, OnDisconnectionProcessed, Socket);
        }

        private void Client_ProcessDataReceived(IAsyncResult ar)
        {
            Socket socket = ar.AsyncState as Socket;
            if (socket != null && socket.Connected)
            {
                int bytesRead = 0;
                try
                {
                    bytesRead = Socket.EndReceive(ar);


                    if (bytesRead == 0)
                    {
                        this.Disconnect();
                        return;
                    }

                    byte[] data = new byte[bytesRead];
                    Array.Copy(Buffer, data, bytesRead);
                    Reader.Add(data, 0, data.Length);

                    ThreatBuffer();

                    Socket.BeginReceive(Buffer, 0, _maxBufferLength, SocketFlags.None, OnDataReceived, Socket);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine($"--------------- Error at : {DateTime.Now}  ---------------");
                    Console.WriteLine($"{ex.GetType().Name}");
                    Console.WriteLine($"Message : {ex.Message}");
                    Console.WriteLine($"Stacktrace : {ex.StackTrace}");
                    Console.WriteLine($"--------------- End error ---------------");

                    this.Disconnect();
                }
            }
            else
            {
                Disconnect();
            }
        }

        public void ThreatBuffer()
        {
            if (this.CurrentMessage == null)
                this.CurrentMessage = new MessagePart(true);

            long position = this.Reader.Position;

            if (this.CurrentMessage.Build(this.Reader))
            {
                this.OnDataProcessed(this.CurrentMessage);
                this.CurrentMessage = (MessagePart)null;
                this.ThreatBuffer();
            }
            else
            {
                if (this.Reader.Position == this.Reader.BaseStream.Length)
                    this.Reader = new BigEndianReader();
                else
                {
                    this.Reader.Seek((int)position, SeekOrigin.Begin);
                }
            }
        }

        private void Client_ProcessMessagePart(MessagePart messagePart)
        {
            BigEndianReader customReader = new BigEndianReader(messagePart.Data);
            NetworkMessage message = MessageReceiver.BuildMessage(messagePart.MessageId.HasValue ? messagePart.MessageId.Value : MessageIdEnum.UKNOWN, customReader);

            Console.WriteLine($"[RCV] {this.IP} -> {message}");

            MessageManager.ParseHandler(this, message);
        }

        private void Client_ProcessDisconnection(IAsyncResult ar)
        {
            Socket.Disconnect(false);
            OnClientDisconnected?.Invoke(this);

            Dispose();
        }

        public void Dispose()
        {
            Socket.Dispose();
            UnBindEvent();
        }

        public void UnBindEvent()
        {
            var delegates = OnDataProcessed.GetInvocationList();
            foreach (var @delegate in delegates)
            {
                OnDataProcessed -= @delegate as DataProcessedEvent;
            }
            delegates = OnClientDisconnected.GetInvocationList();
            foreach (var @delegate in delegates)
            {
                OnClientDisconnected -= @delegate as ClientDisconnectedEvent;
            }
            delegates = OnDataReceived.GetInvocationList();
            foreach (var @delegate in delegates)
            {
                OnDataReceived -= @delegate as AsyncCallback;
            }
            delegates = OnDisconnectionProcessed.GetInvocationList();
            foreach (var @delegate in delegates)
            {
                OnDisconnectionProcessed -= @delegate as AsyncCallback;
            }
        }
    }
}
