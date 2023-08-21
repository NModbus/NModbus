using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
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
    public class ModbusTcpSlaveNetwork : ModbusSlaveNetwork, IModbusTcpSlaveNetwork
    {
        private const int TimeWaitResponse = 1000;
        private readonly object _serverLock = new object();

        private readonly ConcurrentDictionary<string, ModbusMasterTcpConnection> _masters =
            new ConcurrentDictionary<string, ModbusMasterTcpConnection>();

        private TcpListener _server;
#if TIMER
        private Timer _timer;
#endif
        public ModbusTcpSlaveNetwork(TcpListener tcpListener, IModbusFactory modbusFactory,  IModbusLogger logger)
            : base(new EmptyTransport(modbusFactory), modbusFactory, logger)
        {
            if (tcpListener == null)
            {
                throw new ArgumentNullException(nameof(tcpListener));
            }

            _server = tcpListener;
        }

#if TIMER
        private ModbusTcpSlave(byte unitId, TcpListener tcpListener, double timeInterval)
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
        public ReadOnlyCollection<TcpClient> Masters
        {
            get
            {
                return new ReadOnlyCollection<TcpClient>(_masters.Values.Select(mc => mc.TcpClient).ToList());
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
        /////     Modbus TCP slave factory method.
        ///// </summary>
        //public static ModbusTcpSlave CreateTcp(byte unitId, TcpListener tcpListener)
        //{
        //    return new ModbusTcpSlave(unitId, tcpListener);
        //}

#if TIMER
        /// <summary>
        ///     Creates ModbusTcpSlave with timer which polls connected clients every
        ///     <paramref name="pollInterval"/> milliseconds on that they are connected.
        /// </summary>
        public static ModbusTcpSlave CreateTcp(byte unitId, TcpListener tcpListener, double pollInterval)
        {
            return new ModbusTcpSlave(unitId, tcpListener, pollInterval);
        }
#endif

        /// <summary>
        ///     Start slave listening for requests.
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
                        var masterConnection = new ModbusMasterTcpConnection(client, this, ModbusFactory, Logger);
                        masterConnection.ModbusMasterTcpConnectionClosed += OnMasterConnectionClosedHandler;
                        _masters.TryAdd(client.Client.RemoteEndPoint.ToString(), masterConnection);
                    }
                }
                catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested)
                { 
                    //Swallow this
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

                            foreach (var key in _masters.Keys)
                            {
                                ModbusMasterTcpConnection connection;

                                if (_masters.TryRemove(key, out connection))
                                {
                                    connection.ModbusMasterTcpConnectionClosed -= OnMasterConnectionClosedHandler;
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
            foreach (var master in _masters.ToList())
            {
                if (IsSocketConnected(master.Value.TcpClient.Client) == false)
                {
                    master.Value.Dispose();
                }
            }
        }
#endif
        private void OnMasterConnectionClosedHandler(object sender, TcpConnectionEventArgs e)
        {
            ModbusMasterTcpConnection connection;

            if (!_masters.TryRemove(e.EndPoint, out connection))
            {
                string msg = $"EndPoint {e.EndPoint} cannot be removed, it does not exist.";
                throw new ArgumentException(msg);
            }

            connection.Dispose();
            Logger.Information($"Removed Master {e.EndPoint}");
        }
    }
}
