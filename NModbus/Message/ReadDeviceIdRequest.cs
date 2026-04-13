using System;
using System.Linq;
using NModbus.Data;

namespace NModbus.Message
{
    /// <summary>
    ///     Read Device Identification request (function code 0x2B, MEI type 0x0E).
    ///     See Modbus Application Protocol V1.1b3, section 6.21.
    /// </summary>
    public class ReadDeviceIdRequest : IModbusMessage
    {
        /// <summary>Gets or sets the slave address.</summary>
        public byte SlaveAddress { get; set; }

        /// <summary>Gets the MEI type (always 0x0E for Device Identification).</summary>
        public byte MeiType => 0x0E;

        /// <summary>Gets or sets the device ID category to read.</summary>
        public DeviceIdCategory ReadDeviceIdCode { get; set; }

        /// <summary>Gets or sets the first object ID to read.</summary>
        public byte ObjectId { get; set; }

        /// <summary>Gets the function code (always 0x2B).</summary>
        public byte FunctionCode
        {
            get => ModbusFunctionCodes.ReadDeviceIdentification;
            set { /* Fixed function code */ }
        }

        /// <summary>Not used for Device ID requests.</summary>
        public ushort TransactionId { get; set; }

        /// <summary>Parameterless constructor for deserialization.</summary>
        public ReadDeviceIdRequest() { }

        /// <summary>
        ///     Creates a new Read Device Identification request.
        /// </summary>
        /// <param name="slaveAddress">The slave address (1-247).</param>
        /// <param name="category">The device ID category to read.</param>
        /// <param name="objectId">The first object ID to read.</param>
        public ReadDeviceIdRequest(byte slaveAddress, DeviceIdCategory category, byte objectId)
        {
            SlaveAddress = slaveAddress;
            ReadDeviceIdCode = category;
            ObjectId = objectId;
        }

        /// <summary>Gets the complete message frame including slave address.</summary>
        public byte[] MessageFrame => new byte[]
        {
            SlaveAddress,
            FunctionCode,
            MeiType,
            (byte)ReadDeviceIdCode,
            ObjectId
        };

        /// <summary>Gets the protocol data unit (frame without slave address).</summary>
        public byte[] ProtocolDataUnit => MessageFrame.Skip(1).ToArray();

        /// <summary>Initializes the request from a raw frame.</summary>
        /// <param name="frame">The raw frame (minimum 5 bytes).</param>
        /// <exception cref="FormatException">Thrown when the frame is too short or has an invalid function code.</exception>
        public void Initialize(byte[] frame)
        {
            if (frame == null || frame.Length < 5)
                throw new FormatException("Device ID request frame too short (minimum 5 bytes).");

            SlaveAddress = frame[0];

            if (frame[1] != ModbusFunctionCodes.ReadDeviceIdentification)
                throw new FormatException($"Invalid function code. Expected 0x{ModbusFunctionCodes.ReadDeviceIdentification:X2}, got 0x{frame[1]:X2}.");

            ReadDeviceIdCode = (DeviceIdCategory)frame[3];
            ObjectId = frame[4];
        }
    }
}
