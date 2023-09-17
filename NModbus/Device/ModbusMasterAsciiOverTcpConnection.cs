using NModbus.Extensions;
using NModbus.IO;
using NModbus.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NModbus.Device
{

    /// <summary>
    /// 代表来自Modbus主机的传入连接,包含从机处理连接的逻辑。
    /// <para>Represents an incoming connection from a Modbus master. Contains the slave's logic to process the connection.</para>
    /// </summary>
    internal class ModbusMasterAsciiOverTcpConnection : ModbusMasterTcpConnection, IDisposable
    {

        public ModbusMasterAsciiOverTcpConnection(TcpClient client, IModbusSlaveNetwork slaveNetwork, IModbusFactory modbusFactory, IModbusLogger logger)
            : base(client, slaveNetwork, modbusFactory, logger, new ModbusAsciiTransport(new TcpClientAdapter(client), modbusFactory, logger))
        {
            //TODO
        }

        public new event EventHandler<TcpConnectionEventArgs> ModbusMasterTcpConnectionClosed;

        protected override async Task HandleRequestAsync()
        {
            try
            {
                while (true)
                {
                    Logger.Debug($"Begin reading request from Master at IP: {EndPoint}");
                    var Transport = this.Transport as ModbusAsciiTransport;
                    byte[] frame = Transport.ReadRequest();
                    Logger.Information($"RX from Master at {EndPoint}: {string.Join(", ", frame)}");
                    //报文转请求对象
                    var request = _modbusFactory.CreateModbusRequest(frame);

                    #region LRC校验在TCP中无必要
                    //if (Transport.CheckFrame && !Transport.ChecksumsMatch(request, frame))
                    //{
                    //    string msg = $"Checksums failed to match {string.Join(", ", request.MessageFrame)} != {string.Join(", ", frame)}.";
                    //    Logger.Warning(msg);
                    //    throw new IOException(msg);
                    //} 
                    #endregion

                    //获得从站对象
                    IModbusMessage response = (_slaveNetwork as ModbusAsciiOverTcpSlaveNetwork).ApplyRequest(request);
                    Logger.Information($"TX to Master at {EndPoint}: {string.Join(", ", Transport.BuildMessageFrame(response))}");
                    if (response == null)
                    {
                        Transport.IgnoreResponse();
                    }
                    else
                    {
                        Transport.Write(response);
                    }
                }
            }
            // If an exception occurs (such as IOException in case of disconnect, or other failures), handle it as if the connection was gracefully closed
            catch (Exception e)
            {
                Logger.Warning($"{e.GetType().Name} occured with Master at {EndPoint}. Closing connection.");
                ModbusMasterTcpConnectionClosed?.Invoke(this, new TcpConnectionEventArgs(EndPoint));
                //直接返回Task.CompletedTask
                await Task.Delay(0);
            }
        }
    }
}
