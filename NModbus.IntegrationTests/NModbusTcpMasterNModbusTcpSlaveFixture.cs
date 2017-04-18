using System.Net.Sockets;
using Modbus.Device;

namespace Modbus.IntegrationTests
{
    internal class NModbusTcpMasterNModbusTcpSlaveFixture : ModbusMasterFixture
    {
        public NModbusTcpMasterNModbusTcpSlaveFixture()
        {
            SlaveTcp = new TcpListener(TcpHost, Port);
            SlaveTcp.Start();
            Slave = ModbusTcpSlave.CreateTcp(SlaveAddress, SlaveTcp);
            StartSlave();

            MasterTcp = new TcpClient(TcpHost.ToString(), Port);
            Master = ModbusIpMaster.CreateIp(MasterTcp);
        }
    }
}
