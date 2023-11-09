using NModbus.Extensions;
using NModbus.IO;
using NModbus.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NModbus.Device
{
    public class ModbusAsciiOverUdpSlaveNetwork : ModbusUdpSlaveNetwork
    {
        public ModbusAsciiOverUdpSlaveNetwork(UdpClient udpClient, IModbusFactory modbusFactory, IModbusLogger logger)
            : base(udpClient, modbusFactory, logger, new ModbusAsciiTransport(new UdpClientAdapter(udpClient), modbusFactory, logger))
        {
            //TODO
        }

        /// <summary>
        /// Start slave listening for requests.
        /// </summary>
        public override async Task ListenAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (cancellationToken.Register(() =>
            {
#if NET45
                _udpClient.Close();
#else
                _udpClient.Dispose();
#endif
            }))
            {
                Logger.Information("Start ModbusAsciiOverUdp Server.");

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        Logger.Debug($"Begin reading request from Master at IP: {this._udpClient.Client.LocalEndPoint.ToString()}");
                        var Transport = this.Transport as ModbusAsciiTransport;
                        byte[] frame = Transport.ReadRequest();
                        Logger.Information($"RX from Master at {this.Transport.StreamResource.Name}: {string.Join(", ", frame)}");
                        //报文转请求对象
                        var request = ModbusFactory.CreateModbusRequest(frame);

                        #region LRC校验在UDP中无必要
                        //if (Transport.CheckFrame && !Transport.ChecksumsMatch(request, frame))
                        //{
                        //    string msg = $"Checksums failed to match {string.Join(", ", request.MessageFrame)} != {string.Join(", ", frame)}.";
                        //    Logger.Warning(msg);
                        //    throw new IOException(msg);
                        //} 
                        #endregion

                        //获得从站对象
                        IModbusMessage response = ApplyRequest(request);
                        Logger.Information($"TX to Master at {this.Transport.StreamResource.Name}: {string.Join(", ", Transport.BuildMessageFrame(response))}");
                        if (response is null)
                        {
                            Transport.IgnoreResponse();
                        }
                        else
                        {
                            Transport.Write(response);
                        }
                    }
                }
                catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested)
                {
                    // Swallow this error.
                    await Task.Delay(0);
                }
                catch (SocketException se)
                {
                    // this hapens when slave stops
                    if (se.SocketErrorCode != SocketError.Interrupted)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
