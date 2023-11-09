using NModbus.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace NModbus.Device
{

#if TIMER
    using System.Timers;
#endif


    /// <summary>
    /// ModbusRtuOverTcpSlave slave device.
    /// </summary>
    public class ModbusRtuOverTcpSlaveNetwork : ModbusTcpSlaveNetwork
    {
        public ModbusRtuOverTcpSlaveNetwork(TcpListener tcpListener, IModbusFactory modbusFactory, IModbusLogger logger)
            : base(tcpListener, modbusFactory, logger)
        {
            //TODO
        }

#if TIMER
        protected ModbusTcpSlave(byte unitId, TcpListener tcpListener, double timeInterval)
            : base(unitId, new EmptyTransport())
        {
            //TODO
        }
#endif

        /// <summary>
        /// 启动监听，等待连接获得客户端TcpClient对象
        /// <para>Start slave listening for requests.</para>
        /// </summary>
        public override async Task ListenAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            Logger.Information("Start ModbusRtuOverTcp Server.");
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
                        var masterConnection = new ModbusMasterRtuOverTcpConnection(client, this, ModbusFactory, Logger);
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

    }


}
