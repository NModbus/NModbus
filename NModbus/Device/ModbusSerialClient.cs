using System;
using System.Diagnostics.CodeAnalysis;
using NModbus.Data;
using NModbus.Message;

namespace NModbus.Device
{

    /// <summary>
    ///     Modbus serial master device.
    /// </summary>
    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ModbusSerialClient instead.")]
    internal class ModbusSerialMaster : ModbusSerialClient, IModbusSerialMaster
    {
        internal ModbusSerialMaster(IModbusSerialTransport transport) : base(transport) { }
    }

    /// <summary>
    ///     Modbus serial client device.
    /// </summary>
    internal class ModbusSerialClient : ModbusClient, IModbusSerialClient
    {
        internal ModbusSerialClient(IModbusSerialTransport transport)
            : base(transport)
        {
        }

        /// <summary>
        ///     Gets the Modbus Transport.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        IModbusSerialTransport IModbusSerialClient.Transport => (IModbusSerialTransport)Transport;

        /// <summary>
        ///     Serial Line only.
        ///     Diagnostic function which loops back the original data.
        ///     NModbus only supports looping back one ushort value, this is a limitation of the "Best Effort" implementation of
        ///     the RTU protocol.
        /// </summary>
        /// <param name="serverAddress">Address of device to test.</param>
        /// <param name="data">Data to return.</param>
        /// <returns>Return true if server device echoed data.</returns>
        public bool ReturnQueryData(byte serverAddress, ushort data)
        {
            DiagnosticsRequestResponse request = new DiagnosticsRequestResponse(
                ModbusFunctionCodes.DiagnosticsReturnQueryData,
                serverAddress,
                new RegisterCollection(data));

            DiagnosticsRequestResponse response = Transport.UnicastMessage<DiagnosticsRequestResponse>(request);

            return response.Data[0] == data;
        }
    }
}
