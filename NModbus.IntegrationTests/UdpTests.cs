using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NModbus;
using NModbus.Device;

namespace Modbus.IntegrationTests
{
    public class UdpTests : IntegrationTestBase
    {
        protected override Task<IModbusMaster> CreateMasterAsync()
        {
            var client = new UdpClient();

            client.Connect(new IPEndPoint(IPAddress.Loopback, ModbusIPPorts.Insecure));

            var master = Factory.CreateMaster(client);

            return Task.FromResult(master);
        }

        protected override Task<IModbusSlaveNetwork> CreateSlaveNetworkAsync()
        {
            var client = new UdpClient(ModbusIPPorts.Insecure);

            var slaveNetwork = new ModbusUdpSlaveNetwork(client, Factory, Factory.Logger);

            return Task.FromResult<IModbusSlaveNetwork>(slaveNetwork);
        }
    }
}
