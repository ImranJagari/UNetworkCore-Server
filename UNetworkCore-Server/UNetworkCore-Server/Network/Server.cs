using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UNetworkCore_Server.Network
{
    /// <summary>
    /// Server class it permit to start a connection listening on a port and IP
    /// There is events subscribable for : Initialize begin, Initialize end, Server connection start, Client connection received 
    /// </summary>
    public class Server
    {

        #region Private fields
        private Socket _socket;
        private IPAddress _ip;
        private short _port;
        private int _maxPendingConnections;
        #endregion

        #region Public fields
        /// <summary>
        /// Server socket data
        /// </summary>
        public Socket Socket { get => _socket; set => _socket = value; }
        /// <summary>
        /// Server IP connection
        /// </summary>
        public IPAddress IP { get => _ip; set => _ip = value; }
        /// <summary>
        /// Server port connection
        /// </summary>
        public short Port { get => _port; set => _port = value; }
        /// <summary>
        /// Max connections are authorized to be in the queue connection stack
        /// </summary>
        public int MaxPendingConnections { get => _maxPendingConnections; set => _maxPendingConnections = value; }
        #endregion

        #region Events
        public delegate void ServerStartEvent(Socket serverSocket);
        public delegate void InitializeBeginEvent();
        public delegate void InitializeEndedEvent();
        public delegate void ConnectionAcceptedEvent(Client client);

        public event InitializeBeginEvent InitializeBegin;
        public event InitializeEndedEvent InitializeEnded;
        public event ServerStartEvent ServerStarted;
        public event ConnectionAcceptedEvent ConnectionAccepted;

        public event AsyncCallback ConnectionReceived;
        #endregion

        /// <summary>
        /// Constructor with <see cref="IPAddress"/>
        /// </summary>
        /// <param name="ipAddress">IPAdress for server connection</param>
        /// <param name="port">Port for server connection</param>
        public Server(IPAddress ipAddress, short port)
        {
            Initialize();

            IP = ipAddress;
            Port = port;
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            ConnectionReceived += ConnectionReceive;
        }

        /// <summary>
        /// Constructor with string ip
        /// </summary>
        /// <param name="ipAddress">IPAdress for server connection as string</param>
        /// <param name="port">Port for server connection</param>
        public Server(string ip, short port) : this(IPAddress.Parse(ip), port)
        {
        }

        /// <summary>
        /// Empty constructor it initialize by default the IP to local address and port to 5555
        /// </summary>
        public Server() : this(IPAddress.Parse("127.0.0.1"), 5555)
        {

        }

        /// <summary>
        /// Initialize the necessary data for the server
        /// <para></para>
        /// Raise the <see cref="InitializeBegin"/> event and <see cref="InitializeEnded"/> event
        ///</summary>
        public void Initialize()
        {
            InitializeBegin?.Invoke();

            InitializeEnded?.Invoke();
        }

        /// <summary>
        /// Start the connection to listen on specified port and ip and with a max pending connection
        /// <para></para>
        /// Raise the <see cref="ServerStarted"/> event
        /// </summary>
        public void Start()
        {
            Socket.Bind(new IPEndPoint(IP, Port));
            Socket.Listen(MaxPendingConnections);

            Socket.BeginAccept(ConnectionReceived, Socket);

            ServerStarted?.Invoke(Socket);
        }

        /// <summary>
        /// Callback for client are trying to connect to the server
        /// <para></para>
        /// Raise <see cref="ConnectionAccepted"/> event if the connection is accepted
        /// </summary>
        /// <param name="result">Asynchronous data</param>
        public void ConnectionReceive(IAsyncResult result)
        {
            if (Socket.Connected)
            {
                Socket clientSocket = result.AsyncState as Socket;

                if (clientSocket != null)
                {
                    clientSocket.EndAccept(result);

                    Client client = new Client(clientSocket);

                    Console.WriteLine($"Client <{client.IP}> connected !");

                    ConnectionAccepted?.Invoke(client);
                }
            }

            Socket.BeginAccept(ConnectionReceived, Socket);
        }
    }
}
