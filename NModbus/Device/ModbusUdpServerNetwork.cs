using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbus.IO;
using NModbus.Logging;
using NModbus.Message;
using NModbus.Unme.Common;

namespace NModbus.Device
{
    using Extensions;

    /// <summary>
    ///     Modbus UDP slave device.
    /// </summary>
    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ModbusUdpServerNetwork instead.")]
    internal class ModbusUdpSlaveNetwork : ModbusUdpServerNetwork
    {
        public ModbusUdpSlaveNetwork(UdpClient udpClient, IModbusFactory modbusFactory, IModbusLogger logger) : base(udpClient, modbusFactory, logger) { }
    }

    /// <summary>
    ///     Modbus UDP server device.
    /// </summary>
    internal class ModbusUdpServerNetwork : ModbusServerNetwork
    {
        private readonly UdpClient _udpClient;

        public ModbusUdpServerNetwork(UdpClient udpClient, IModbusFactory modbusFactory, IModbusLogger logger)
            : base(new ModbusIpTransport(new UdpClientAdapter(udpClient), modbusFactory, logger), modbusFactory, logger)
        {
            _udpClient = udpClient;
        }

        /// <summary>
        ///     Start server listening for requests.
        /// </summary>
        public override async Task ListenAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            Logger.Information("Start Modbus Udp Server.");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    UdpReceiveResult receiveResult = await _udpClient.ReceiveAsync().ConfigureAwait(false);
                    IPEndPoint clientEndPoint = receiveResult.RemoteEndPoint;
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

                        await _udpClient.SendAsync(responseFrame, responseFrame.Length, clientEndPoint)
                            .ConfigureAwait(false);
                    }
                }
            }
            catch (SocketException se)
            {
                // this hapens when server stops
                if (se.SocketErrorCode != SocketError.Interrupted)
                {
                    throw;
                }
            }
        }
    }
}
