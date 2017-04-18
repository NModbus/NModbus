using System.Net.Sockets;
using Modbus.Device;

namespace Modbus.IntegrationTests
{
    internal class NModbusTcpMasterJamodTcpSlaveFixture : ModbusMasterFixture
    {
        public NModbusTcpMasterJamodTcpSlaveFixture()
        {
            string program = $"TcpSlave {Port}";
            StartJamodSlave(program);

            MasterTcp = new TcpClient(TcpHost.ToString(), Port);
            Master = ModbusIpMaster.CreateIp(MasterTcp);
        }

        /// <summary>
        /// Not supported by the Jamod Slave
        /// </summary>
        public override void ReadWriteMultipleRegisters()
        {
        }
    }
}
