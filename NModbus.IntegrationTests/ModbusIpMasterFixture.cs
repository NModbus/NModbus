using System.Net.Sockets;
using System.Threading;
using Modbus.Device;
using Xunit;

namespace Modbus.IntegrationTests
{
    internal class ModbusIpMasterFixture
    {
        [Fact]
        public void OverrideTimeoutOnTcpClient()
        {
            var listener = new TcpListener(ModbusMasterFixture.TcpHost, ModbusMasterFixture.Port);
            using (var slave = ModbusTcpSlave.CreateTcp(ModbusMasterFixture.SlaveAddress, listener))
            {
                var slaveThread = new Thread(slave.Listen);
                slaveThread.Start();

                var client = new TcpClient(ModbusMasterFixture.TcpHost.ToString(), ModbusMasterFixture.Port);
                client.ReceiveTimeout = 1500;
                client.SendTimeout = 3000;
                using (var master = ModbusIpMaster.CreateIp(client))
                {
                    Assert.Equal(1500, client.GetStream().ReadTimeout);
                    Assert.Equal(3000, client.GetStream().WriteTimeout);
                }
            }
        }

        [Fact]
        public void OverrideTimeoutOnNetworkStream()
        {
            var listener = new TcpListener(ModbusMasterFixture.TcpHost, ModbusMasterFixture.Port);
            using (var slave = ModbusTcpSlave.CreateTcp(ModbusMasterFixture.SlaveAddress, listener))
            {
                var slaveThread = new Thread(slave.Listen);
                slaveThread.Start();

                var client = new TcpClient(ModbusMasterFixture.TcpHost.ToString(), ModbusMasterFixture.Port);
                client.GetStream().ReadTimeout = 1500;
                client.GetStream().WriteTimeout = 3000;
                using (var master = ModbusIpMaster.CreateIp(client))
                {
                    Assert.Equal(1500, client.GetStream().ReadTimeout);
                    Assert.Equal(3000, client.GetStream().WriteTimeout);
                }
            }
        }
    }
}
