using System.Collections.ObjectModel;
using System.Net.Sockets;

namespace NModbus
{
    /// <summary>
    ///     Modbus TCP slave device.
    /// </summary>
    public interface IModbusTcpSlaveNetwork : IModbusSlaveNetwork
    {
        /// <summary>
        ///     Gets the Modbus TCP Masters connected to this Modbus TCP Slave.
        /// </summary>
        ReadOnlyCollection<TcpClient> Masters { get; }
    }
}
