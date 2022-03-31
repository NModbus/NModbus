using System;

namespace NModbus
{

    /// <summary>
    ///     Modbus Serial Master device.
    /// </summary>
    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use IModbusSerialClient instead.")]
    public interface IModbusSerialMaster : IModbusSerialClient, IModbusMaster { }

    /// <summary>
    ///     Modbus Serial Client device.
    /// </summary>
    public interface IModbusSerialClient : IModbusClient
    {
        /// <summary>
        ///     Transport for used by this client.
        /// </summary>
        new IModbusSerialTransport Transport { get; }

        /// <summary>
        ///     Serial Line only.
        ///     Diagnostic function which loops back the original data.
        ///     NModbus only supports looping back one ushort value, this is a
        ///     limitation of the "Best Effort" implementation of the RTU protocol.
        /// </summary>
        /// <param name="serverAddress">Address of device to test.</param>
        /// <param name="data">Data to return.</param>
        /// <returns>Return true if server device echoed data.</returns>
        bool ReturnQueryData(byte serverAddress, ushort data);
    }
}
