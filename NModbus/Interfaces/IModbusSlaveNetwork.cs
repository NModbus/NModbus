using System;
using System.Threading;
using System.Threading.Tasks;

namespace NModbus
{
    /// <summary>
    /// A network of slave devices on a single transport.
    /// </summary>
    public interface IModbusSlaveNetwork : IDisposable
    {
        /// <summary>
        /// Listen for incoming requests.
        /// </summary>
        /// <returns></returns>
        Task ListenAsync(CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Add a slave to the network.
        /// </summary>
        /// <param name="slave"></param>
        void AddSlave(IModbusSlave slave);

        /// <summary>
        /// Get a slave from the network.
        /// </summary>
        /// <param name="unitId">The slave address</param>
        /// <returns>The specified slave, or null if one can't be found.</returns>
        IModbusSlave GetSlave(byte unitId);

        /// <summary>
        /// Removes a slave from the network.
        /// </summary>
        /// <param name="unitId"></param>
        void RemoveSlave(byte unitId);
    }
}