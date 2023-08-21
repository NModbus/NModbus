using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbus.IO;
using NModbus.Logging;
using NModbus.Unme.Common;

namespace NModbus.Device
{
    using Extensions;

    /// <summary>
    ///     Modbus UDP slave device.
    /// </summary>
    public class ModbusUdpSlaveNetwork : ModbusSlaveNetwork
    {
        private readonly UdpClient _udpClient;

        public ModbusUdpSlaveNetwork(UdpClient udpClient, IModbusFactory modbusFactory, IModbusLogger logger)
            : base(new ModbusIpTransport(new UdpClientAdapter(udpClient), modbusFactory, logger), modbusFactory, logger)
        {
            _udpClient = udpClient;
        }

        /// <summary>
        ///     Start slave listening for requests.
        /// </summary>
        public override async Task ListenAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            using (cancellationToken.Register(() =>
            {
                _udpClient.Dispose();
            }))
            {
                Logger.Information("Start Modbus Udp Server.");

                try
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        UdpReceiveResult receiveResult = await _udpClient.ReceiveAsync().ConfigureAwait(false);
                        IPEndPoint masterEndPoint = receiveResult.RemoteEndPoint;
                        byte[] frame = receiveResult.Buffer;

                        Debug.WriteLine($"Read Frame completed {frame.Length} bytes");

                        Logger.LogFrameRx(frame);

                        IModbusMessage request = ModbusFactory.CreateModbusRequest(frame.Slice(6, frame.Length - 6).ToArray());
                        request.TransactionId = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 0));

                        // perform action and build response
                        IModbusMessage response = ApplyRequest(request);

                        if (response != null)
                        {
                            response.TransactionId = request.TransactionId;

                            // write response
                            byte[] responseFrame = Transport.BuildMessageFrame(response);

                            Logger.LogFrameTx(frame);

                            await _udpClient.SendAsync(responseFrame, responseFrame.Length, masterEndPoint)
                                .ConfigureAwait(false);
                        }
                    }
                }
                catch( ObjectDisposedException) when (cancellationToken.IsCancellationRequested)
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
