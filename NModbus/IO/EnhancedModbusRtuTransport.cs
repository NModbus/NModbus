using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NModbus.Logging;
using NModbus.Message;
using NModbus.Utility;

namespace NModbus.IO
{
    /// <summary>
    ///     Extended RTU transport that handles variable-length responses such as
    ///     Read Device Identification (function code 0x2B). Use
    ///     <see cref="ModbusFactoryExtensions.CreateRtuMasterWithDeviceId"/> to
    ///     create a master that uses this transport.
    /// </summary>
    internal class EnhancedModbusRtuTransport : ModbusRtuTransport
    {
        private const byte DeviceIdFunctionCode = ModbusFunctionCodes.ReadDeviceIdentification;
        private const byte MeiType = 0x0E;

        public EnhancedModbusRtuTransport(
            IStreamResource streamResource,
            IModbusFactory modbusFactory,
            IModbusLogger logger)
            : base(streamResource, modbusFactory, logger)
        {
        }

        /// <summary>
        ///     Overrides response reading to detect Device ID responses and
        ///     read their variable-length frames correctly.
        /// </summary>
        public override IModbusMessage ReadResponse<T>()
        {
            byte[] frameStart = Read(2); // [SlaveAddress][FunctionCode]

            if (frameStart[1] == DeviceIdFunctionCode)
            {
                byte[] fullFrame = ReadDeviceIdResponseFrame(frameStart);
                Logger.LogFrameRx(fullFrame);
                return CreateResponse<T>(fullFrame);
            }
            else if ((frameStart[1] & 0x80) == 0x80)
            {
                // Error response: [SlaveAddr][FC|0x80][ExceptionCode][CRC][CRC]
                byte exceptionCode = Read(1)[0];
                byte[] crc = Read(2);

                byte[] fullFrame = new byte[5];
                fullFrame[0] = frameStart[0];
                fullFrame[1] = frameStart[1];
                fullFrame[2] = exceptionCode;
                fullFrame[3] = crc[0];
                fullFrame[4] = crc[1];

                Logger.LogFrameRx(fullFrame);
                return CreateResponse<T>(fullFrame);
            }
            else
            {
                byte[] fullFrame = ReadStandardModbusResponse(frameStart);
                Logger.LogFrameRx(fullFrame);
                return CreateResponse<T>(fullFrame);
            }
        }

        /// <summary>
        ///     Reads a standard Modbus response after the first 2 bytes have
        ///     already been consumed.
        /// </summary>
        private byte[] ReadStandardModbusResponse(byte[] frameStart)
        {
            var frame = new List<byte>(frameStart);
            byte functionCode = frameStart[1];

            switch (functionCode)
            {
                case 0x01: // Read Coils
                case 0x02: // Read Discrete Inputs
                case 0x03: // Read Holding Registers
                case 0x04: // Read Input Registers
                case 0x11: // Report Slave ID
                case 0x17: // Read/Write Multiple Registers
                {
                    byte byteCount = Read(1)[0];
                    frame.Add(byteCount);
                    frame.AddRange(Read(byteCount));
                    frame.AddRange(Read(2)); // CRC
                    break;
                }

                case 0x05: // Write Single Coil
                case 0x06: // Write Single Register
                case 0x0F: // Write Multiple Coils
                case 0x10: // Write Multiple Registers
                {
                    frame.AddRange(Read(4)); // Address + Value/Quantity
                    frame.AddRange(Read(2)); // CRC
                    break;
                }

                case 0x16: // Mask Write Register
                {
                    frame.AddRange(Read(6)); // Address + AND mask + OR mask
                    frame.AddRange(Read(2)); // CRC
                    break;
                }

                case 0x18: // Read FIFO Queue
                {
                    byte[] counts = Read(4);
                    frame.AddRange(counts);
                    ushort byteCount = (ushort)((counts[0] << 8) | counts[1]);
                    if (byteCount > 2)
                        frame.AddRange(Read(byteCount - 2));
                    frame.AddRange(Read(2)); // CRC
                    break;
                }

                default:
                    throw new IOException(
                        $"EnhancedModbusRtuTransport: unsupported function code 0x{functionCode:X2}");
            }

            return frame.ToArray();
        }

        /// <summary>
        ///     Reads a complete Device ID response frame including CRC verification.
        /// </summary>
        private byte[] ReadDeviceIdResponseFrame(byte[] frameStart)
        {
            var frame = new List<byte>(frameStart);

            // Error response check
            if ((frameStart[1] & 0x80) == 0x80)
            {
                frame.Add(Read(1)[0]); // Exception code
                frame.AddRange(Read(2)); // CRC
                return frame.ToArray();
            }

            // Header: MEI type + Device ID code + Conformity + MoreFollows + NextObjId + NumObjects
            byte meiType = Read(1)[0];
            frame.Add(meiType);

            if (meiType != MeiType)
                throw new IOException($"Unexpected MEI type 0x{meiType:X2} in Device ID response.");

            byte[] header = Read(5); // DeviceIdCode, Conformity, MoreFollows, NextObjId, NumObjects
            frame.AddRange(header);

            byte numberOfObjects = header[4];

            // Read each object: [ObjectId][Length][Value...]
            for (int i = 0; i < numberOfObjects; i++)
            {
                byte objectId = Read(1)[0];
                frame.Add(objectId);

                byte objectLength = Read(1)[0];
                frame.Add(objectLength);

                if (objectLength > 0)
                    frame.AddRange(Read(objectLength));
            }

            // Read and verify CRC
            byte[] crcBytes = Read(2);
            byte[] dataFrame = frame.ToArray();
            ushort received = (ushort)(crcBytes[0] | (crcBytes[1] << 8));
            ushort calculated = BitConverter.ToUInt16(ModbusUtility.CalculateCrc(dataFrame), 0);

            if (received != calculated)
                throw new IOException($"Device ID response CRC mismatch. Received: 0x{received:X4}, calculated: 0x{calculated:X4}");

            return dataFrame;
        }
    }
}
