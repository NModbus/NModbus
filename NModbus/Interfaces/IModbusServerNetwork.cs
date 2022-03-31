using System;
using System.Threading;
using System.Threading.Tasks;

namespace NModbus
{
    /// <summary>
    /// A network of slave devices on a single transport.
    /// </summary>
    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use IModbusServerNetwork instead.")]
    public interface IModbusSlaveNetwork : IModbusServerNetwork { }

    /// <summary>
    /// A network of server devices on a single transport.
    /// </summary>
    public interface IModbusServerNetwork : IDisposable
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
        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use AddServer instead.")]
        void AddSlave(IModbusSlave slave);
        /// <summary>
        /// Add a server to the network.
        /// </summary>
        /// <param name="server"></param>
        void AddServer(IModbusServer server);

        /// <summary>
        /// Get a slave from the network.
        /// </summary>
        /// <param name="unitId">The slave address</param>
        /// <returns>The specified slave, or null if one can't be found.</returns>
        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use GetServer instead.")]
        IModbusSlave GetSlave(byte unitId);
        /// <summary>
        /// Get a server from the network.
        /// </summary>
        /// <param name="unitId">The server address</param>
        /// <returns>The specified server, or null if one can't be found.</returns>
        IModbusServer GetServer(byte unitId);


        /// <summary>
        /// Removes a slave from the network.
        /// </summary>
        /// <param name="unitId"></param>
        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use RemoveServer instead.")]
        void RemoveSlave(byte unitId);
        /// <summary>
        /// Removes a server from the network.
        /// </summary>
        /// <param name="unitId"></param>
        void RemoveServer(byte unitId);
    }
}