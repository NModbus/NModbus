using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbus.IO;
using NModbus.Logging;

namespace NModbus.Device
{
#if TIMER
    using System.Timers;
#endif

    /// <summary>
    ///     Modbus TCP slave device.
    /// </summary>
    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ModbusTcpServerNetwork instead.")]
    internal class ModbusTcpSlaveNetwork : ModbusTcpServerNetwork
    {
        internal ModbusTcpSlaveNetwork(TcpListener tcpListener, IModbusFactory modbusFactory, IModbusLogger logger) : base(tcpListener, modbusFactory, logger) { }
    }

    /// <summary>
    ///     Modbus TCP server device.
    /// </summary>
    internal class ModbusTcpServerNetwork : ModbusServerNetwork
    {
        private const int TimeWaitResponse = 1000;
        private readonly object _serverLock = new object();

        private readonly ConcurrentDictionary<string, ModbusClientTcpConnection> _clients =
            new ConcurrentDictionary<string, ModbusClientTcpConnection>();

        private TcpListener _server;
#if TIMER
        private Timer _timer;
#endif
        internal ModbusTcpServerNetwork(TcpListener tcpListener, IModbusFactory modbusFactory,  IModbusLogger logger)
            : base(new EmptyTransport(modbusFactory), modbusFactory, logger)
        {
            if (tcpListener == null)
            {
                throw new ArgumentNullException(nameof(tcpListener));
            }

            _server = tcpListener;
        }

#if TIMER
        private ModbusTcpServer(byte unitId, TcpListener tcpListener, double timeInterval)
            : base(unitId, new EmptyTransport())
        {
            if (tcpListener == null)
            {
                throw new ArgumentNullException(nameof(tcpListener));
            }

            _server = tcpListener;
            _timer = new Timer(timeInterval);
            _timer.Elapsed += OnTimer;
            _timer.Enabled = true;
        }
#endif

        /// <summary>
        ///     Gets the Modbus TCP Masters connected to this Modbus TCP Slave.
        /// </summary>
        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use Clients instead.")]
        public ReadOnlyCollection<TcpClient> Masters => Clients;

        /// <summary>
        ///     Gets the Modbus TCP Clients connected to this Modbus TCP Server.
        /// </summary>
        public ReadOnlyCollection<TcpClient> Clients
        {
            get
            {
                return new ReadOnlyCollection<TcpClient>(_clients.Values.Select(mc => mc.TcpClient).ToList());
            }
        }

        /// <summary>
        ///     Gets the server.
        /// </summary>
        /// <value>The server.</value>
        /// <remarks>
        ///     This property is not thread safe, it should only be consumed within a lock.
        /// </remarks>
        private TcpListener Server
        {
            get
            {
                if (_server == null)
                {
                    throw new ObjectDisposedException("Server");
                }

                return _server;
            }
        }

        ///// <summary>
        /////     Modbus TCP server factory method.
        ///// </summary>
        //public static ModbusTcpServer CreateTcp(byte unitId, TcpListener tcpListener)
        //{
        //    return new ModbusTcpServer(unitId, tcpListener);
        //}

#if TIMER

/// <summary>
        ///     Creates ModbusTcpSlave with timer which polls connected clients every
        ///     <paramref name="pollInterval"/> milliseconds on that they are connected.
        /// </summary>
        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ModbusTcpServer instead.")]
        public static ModbusTcpSlave CreateTcp(byte unitId, TcpListener tcpListener, double pollInterval) { return ModbusTcpServer(unitId, tcpListener, pollInterval); }

        /// <summary>
        ///     Creates ModbusTcpServer with timer which polls connected clients every
        ///     <paramref name="pollInterval"/> milliseconds on that they are connected.
        /// </summary>
        public static ModbusTcpServer CreateTcp(byte unitId, TcpListener tcpListener, double pollInterval)
        {
            return new ModbusTcpServer(unitId, tcpListener, pollInterval);
        }
#endif

        /// <summary>
        ///     Start server listening for requests.
        /// </summary>
        public override async Task ListenAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            Logger.Information("Start Modbus Tcp Server.");
            // TODO: add state {stopped, listening} and check it before starting
            Server.Start();
            // Cancellation code based on https://stackoverflow.com/a/47049129/11066760
            using (cancellationToken.Register(() => Server.Stop()))
            {
                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        TcpClient client = await Server.AcceptTcpClientAsync().ConfigureAwait(false);
                        var clientConnection = new ModbusClientTcpConnection(client, this, ModbusFactory, Logger);
                        clientConnection.ModbusClientTcpConnectionClosed += OnClientConnectionClosedHandler;
                        _clients.TryAdd(client.Client.RemoteEndPoint.ToString(), clientConnection);
                    }
                }
                catch (InvalidOperationException)
                {
                    // Either Server.Start wasn't called (a bug!)
                    // or the CancellationToken was cancelled before
                    // we started accepting (giving an InvalidOperationException),
                    // or the CancellationToken was cancelled after
                    // we started accepting (giving an ObjectDisposedException).
                    //
                    // In the latter two cases we should surface the cancellation
                    // exception, or otherwise rethrow the original exception.
                    cancellationToken.ThrowIfCancellationRequested();
                    throw;
                }
            }
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        /// <remarks>Dispose is thread-safe.</remarks>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // double-check locking
                if (_server != null)
                {
                    lock (_serverLock)
                    {
                        if (_server != null)
                        {
                            _server.Stop();
                            _server = null;

#if TIMER
                            if (_timer != null)
                            {
                                _timer.Dispose();
                                _timer = null;
                            }
#endif

                            foreach (var key in _clients.Keys)
                            {
                                ModbusClientTcpConnection connection;

                                if (_clients.TryRemove(key, out connection))
                                {
                                    connection.ModbusClientTcpConnectionClosed -= OnClientConnectionClosedHandler;
                                    connection.Dispose();
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool IsSocketConnected(Socket socket)
        {
            bool poll = socket.Poll(TimeWaitResponse, SelectMode.SelectRead);
            bool available = (socket.Available == 0);
            return poll && available;
        }

#if TIMER
        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            foreach (var client in _clients.ToList())
            {
                if (IsSocketConnected(client.Value.TcpClient.Client) == false)
                {
                    client.Value.Dispose();
                }
            }
        }
#endif
        private void OnClientConnectionClosedHandler(object sender, TcpConnectionEventArgs e)
        {
            ModbusClientTcpConnection connection;

            if (!_clients.TryRemove(e.EndPoint, out connection))
            {
                string msg = $"EndPoint {e.EndPoint} cannot be removed, it does not exist.";
                throw new ArgumentException(msg);
            }

            connection.Dispose();
            Logger.Information($"Removed Client {e.EndPoint}");
        }
    }
}
