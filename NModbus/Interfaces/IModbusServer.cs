using System;

namespace NModbus
{
    /// <summary>
    /// A modbus slave.
    /// </summary>
    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use IModbusServer instead.")]
    public interface IModbusSlave : IModbusServer { }

    /// <summary>
    /// A modbus server.
    /// </summary>
    public interface IModbusServer
    {
        /// <summary>
        /// Gets the unit id of this server.
        /// </summary>
        byte UnitId { get; }

        /// <summary>
        /// Gets the data store for this server.
        /// </summary>
        IServerDataStore DataStore { get; }


        /// <summary>
        /// Applies the request.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        IModbusMessage ApplyRequest(IModbusMessage request);
    }
}