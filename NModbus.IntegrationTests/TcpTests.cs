using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NModbus;
using NModbus.Device;

namespace Modbus.IntegrationTests
{
    public class TcpTests : IntegrationTestBase
    {
        protected override Task<IModbusMaster> CreateMasterAsync()
        {
            var client = new TcpClient();

            client.Connect(new IPEndPoint(IPAddress.Loopback, ModbusIPPorts.Insecure));

            var master = Factory.CreateMaster(client);

            return Task.FromResult(master);
        }

        protected override Task<IModbusSlaveNetwork> CreateSlaveNetworkAsync()
        {
            var listener = new TcpListener(IPAddress.Loopback, ModbusIPPorts.Insecure);

            var slaveNetwork = new ModbusTcpSlaveNetwork(listener, Factory, Factory.Logger);

            return Task.FromResult<IModbusSlaveNetwork>(slaveNetwork);
        }
    }
}
