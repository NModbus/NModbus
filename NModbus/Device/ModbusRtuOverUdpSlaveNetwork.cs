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
    public class ModbusRtuOverUdpSlaveNetwork : ModbusUdpSlaveNetwork
    {
        public ModbusRtuOverUdpSlaveNetwork(UdpClient udpClient, IModbusFactory modbusFactory, IModbusLogger logger)
            : base(udpClient, modbusFactory, logger, new ModbusRtuTransport(new UdpClientAdapter(udpClient), modbusFactory, logger))
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
                Logger.Information("Start ModbusRtuOverUdp Server.");

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        UdpReceiveResult receiveResult = await _udpClient.ReceiveAsync().ConfigureAwait(false);
                        IPEndPoint masterEndPoint = receiveResult.RemoteEndPoint;
                        //获取报文
                        byte[] frame = receiveResult.Buffer;
                        Debug.WriteLine($"Read Frame completed {frame.Length} bytes");

                        Logger.Debug($"Begin reading request from Master at IP: {masterEndPoint}");
                        var Transport = this.Transport as ModbusRtuTransport;

                        Logger.LogFrameRx(frame);
                        Logger.Information($"RX from Master at {masterEndPoint}: {string.Join(", ", frame)}");
                        //报文转请求对象
                        var request = ModbusFactory.CreateModbusRequest(frame);

                        #region CRC校验在UDP中无必要
                        //if (Transport.CheckFrame && !Transport.ChecksumsMatch(request, frame))
                        //{
                        //    string msg = $"Checksums failed to match {string.Join(", ", request.MessageFrame)} != {string.Join(", ", frame)}.";
                        //    Logger.Warning(msg);
                        //    throw new IOException(msg);
                        //} 
                        #endregion

                        //获得响应
                        IModbusMessage response = ApplyRequest(request);
                        if (response != null)
                        {
                            byte[] responseFrame = Transport.BuildMessageFrame(response);
                            Logger.Information($"TX to Master at {masterEndPoint}: {string.Join(", ", responseFrame)}");
                            Logger.LogFrameTx(frame);

                            await _udpClient.SendAsync(responseFrame, responseFrame.Length, masterEndPoint)
                                .ConfigureAwait(false);
                        }
                    }
                }
                catch (ObjectDisposedException) when (cancellationToken.IsCancellationRequested)
                {
                    // Swallow this error.
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
