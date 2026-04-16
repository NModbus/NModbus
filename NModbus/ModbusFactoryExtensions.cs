using NModbus.IO;

namespace NModbus
{
    /// <summary>
    ///     Extension methods for <see cref="IModbusFactory"/> that provide enhanced transport support.
    /// </summary>
    public static class ModbusFactoryExtensions
    {
        /// <summary>
        ///     Creates an RTU master that supports Read Device Identification (FC 0x2B)
        ///     and other variable-length Modbus responses.
        /// </summary>
        /// <remarks>
        ///     Uses <see cref="EnhancedModbusRtuTransport"/> which extends the standard
        ///     RTU transport with dynamic frame length detection for Device Identification
        ///     responses that cannot be determined from the response header alone.
        /// </remarks>
        /// <param name="factory">The Modbus factory.</param>
        /// <param name="streamResource">The underlying serial stream resource.</param>
        /// <returns>A serial master capable of Device ID operations.</returns>
        public static IModbusSerialMaster CreateRtuMasterWithDeviceId(
            this IModbusFactory factory,
            IStreamResource streamResource)
        {
            var transport = new EnhancedModbusRtuTransport(streamResource, factory, factory.Logger);
            return factory.CreateMaster(transport);
        }
    }
}
