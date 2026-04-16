using System;
using System.Collections.Generic;
using System.Linq;
using NModbus.Data;

namespace NModbus.Message
{
    /// <summary>
    ///     Response to a Read Device Identification request (function code 0x2B, MEI type 0x0E).
    ///     Parses variable-length object data from the response frame.
    /// </summary>
    public class ReadDeviceIdResponse : IModbusMessage
    {
        private const byte MaxObjectCount = 128;

        /// <summary>Gets or sets the slave address.</summary>
        public byte SlaveAddress { get; set; }

        /// <summary>Gets the function code (always 0x2B).</summary>
        public byte FunctionCode
        {
            get => ModbusFunctionCodes.ReadDeviceIdentification;
            set { /* Fixed function code */ }
        }

        /// <summary>Gets the MEI type (always 0x0E for Device Identification).</summary>
        public byte MeiType => 0x0E;

        /// <summary>Gets the device ID category that was requested.</summary>
        public DeviceIdCategory ReadDeviceIdCode { get; set; }

        /// <summary>Gets the conformity level of the device.</summary>
        public byte ConformityLevel { get; set; }

        /// <summary>Gets whether more objects are available in a subsequent request.</summary>
        public bool MoreFollows { get; set; }

        /// <summary>Gets the object ID to use in the next request when <see cref="MoreFollows"/> is true.</summary>
        public byte NextObjectId { get; set; }

        /// <summary>Gets the parsed device identification objects (key = object ID, value = string).</summary>
        public Dictionary<byte, string> Objects { get; set; } = new Dictionary<byte, string>();

        /// <summary>
        ///     The complete response frame. Stored because Device ID responses are variable-length
        ///     and cannot be reconstructed from properties alone.
        /// </summary>
        public byte[] MessageFrame { get; private set; }

        /// <summary>Gets the protocol data unit (frame without slave address).</summary>
        public byte[] ProtocolDataUnit => MessageFrame?.Skip(1).ToArray();

        // Standard object convenience properties
        /// <summary>Gets the vendor name (object 0x00), or null if not present.</summary>
        public string VendorName => Objects.ContainsKey(0x00) ? Objects[0x00] : null;

        /// <summary>Gets the product code (object 0x01), or null if not present.</summary>
        public string ProductCode => Objects.ContainsKey(0x01) ? Objects[0x01] : null;

        /// <summary>Gets the major/minor revision (object 0x02), or null if not present.</summary>
        public string MajorMinorRevision => Objects.ContainsKey(0x02) ? Objects[0x02] : null;

        /// <summary>Gets the vendor URL (object 0x03), or null if not present.</summary>
        public string VendorUrl => Objects.ContainsKey(0x03) ? Objects[0x03] : null;

        /// <summary>Gets the product name (object 0x04), or null if not present.</summary>
        public string ProductName => Objects.ContainsKey(0x04) ? Objects[0x04] : null;

        /// <summary>Gets the model name (object 0x05), or null if not present.</summary>
        public string ModelName => Objects.ContainsKey(0x05) ? Objects[0x05] : null;

        /// <summary>Gets the user application name (object 0x06), or null if not present.</summary>
        public string UserApplicationName => Objects.ContainsKey(0x06) ? Objects[0x06] : null;

        /// <summary>Not used for Device ID responses.</summary>
        public ushort TransactionId { get; set; }

        /// <summary>
        ///     Initializes the response from a raw Modbus frame.
        /// </summary>
        /// <param name="frame">
        ///     The complete response frame including slave address.
        ///     Minimum 8 bytes: [SlaveAddr][0x2B][MEI][Category][Conformity][MoreFollows][NextObjId][NumObjects]
        /// </param>
        /// <exception cref="FormatException">Thrown when the frame is malformed.</exception>
        public void Initialize(byte[] frame)
        {
            if (frame == null)
                throw new ArgumentNullException(nameof(frame));

            MessageFrame = frame;

            if (frame.Length < 8)
                throw new FormatException($"Device ID response too short ({frame.Length} bytes, minimum 8).");

            SlaveAddress = frame[0];

            // Error response: MSB of function code is set (0x2B becomes 0xAB)
            if ((frame[1] & 0x80) != 0)
            {
                byte exceptionCode = frame.Length > 2 ? frame[2] : (byte)0;
                throw new SlaveException(
                    $"Device ID exception from slave {SlaveAddress}: exception code 0x{exceptionCode:X2}");
            }

            if (frame[1] != FunctionCode)
                throw new FormatException($"Invalid function code. Expected 0x{FunctionCode:X2}, got 0x{frame[1]:X2}.");

            if (frame[2] != MeiType)
                throw new FormatException($"Invalid MEI type. Expected 0x{MeiType:X2}, got 0x{frame[2]:X2}.");

            ReadDeviceIdCode = (DeviceIdCategory)frame[3];
            ConformityLevel = frame[4];
            MoreFollows = frame[5] != 0x00;
            NextObjectId = frame[6];

            byte numberOfObjects = frame[7];
            if (numberOfObjects > MaxObjectCount)
                throw new FormatException($"Object count {numberOfObjects} exceeds maximum ({MaxObjectCount}).");

            int index = 8;
            Objects.Clear();

            for (int i = 0; i < numberOfObjects; i++)
            {
                if (index + 2 > frame.Length)
                    throw new FormatException("Unexpected end of frame while parsing object header.");

                byte objectId = frame[index++];
                byte objectLength = frame[index++];

                if (index + objectLength > frame.Length)
                    throw new FormatException($"Object 0x{objectId:X2} data ({objectLength} bytes) exceeds frame length.");

                string objectValue = System.Text.Encoding.ASCII.GetString(frame, index, objectLength);
                index += objectLength;

                Objects[objectId] = objectValue;
            }
        }
    }
}
