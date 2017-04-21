using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NModbus.Interfaces;
using NModbus.Message;
using NModbus.Utility;

namespace NModbus.IO
{
    /// <summary>
    ///     Refined Abstraction - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    internal class ModbusRtuTransport : ModbusSerialTransport, IModbusRtuTransport
    {
        private readonly IModbusFactory _modbusFactory;
        public const int RequestFrameStartLength = 7;

        public const int ResponseFrameStartLength = 4;

        internal ModbusRtuTransport(IStreamResource streamResource, IModbusFactory modbusFactory)
            : base(streamResource)
        {
            if (modbusFactory == null) throw new ArgumentNullException(nameof(modbusFactory));
            _modbusFactory = modbusFactory;
            Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
        }

        public static int RequestBytesToRead(byte[] frameStart)
        {
            byte functionCode = frameStart[1];
            int numBytes;

            switch (functionCode)
            {
                case ModbusFunctionCodes.ReadCoils:
                case ModbusFunctionCodes.ReadInputs:
                case ModbusFunctionCodes.ReadHoldingRegisters:
                case ModbusFunctionCodes.ReadInputRegisters:
                case ModbusFunctionCodes.WriteSingleCoil:
                case ModbusFunctionCodes.WriteSingleRegister:
                case ModbusFunctionCodes.Diagnostics:
                    numBytes = 1;
                    break;
                case ModbusFunctionCodes.WriteMultipleCoils:
                case ModbusFunctionCodes.WriteMultipleRegisters:
                    byte byteCount = frameStart[6];
                    numBytes = byteCount + 2;
                    break;
                default:
                    string msg = $"Function code {functionCode} not supported.";
                    Debug.WriteLine(msg);
                    throw new NotImplementedException(msg);
            }

            return numBytes;
        }

        public static int ResponseBytesToRead(byte[] frameStart)
        {
            byte functionCode = frameStart[1];

            // exception response
            if (functionCode > Modbus.ExceptionOffset)
            {
                return 1;
            }

            int numBytes;
            switch (functionCode)
            {
                case ModbusFunctionCodes.ReadCoils:
                case ModbusFunctionCodes.ReadInputs:
                case ModbusFunctionCodes.ReadHoldingRegisters:
                case ModbusFunctionCodes.ReadInputRegisters:
                    numBytes = frameStart[2] + 1;
                    break;
                case ModbusFunctionCodes.WriteSingleCoil:
                case ModbusFunctionCodes.WriteSingleRegister:
                case ModbusFunctionCodes.WriteMultipleCoils:
                case ModbusFunctionCodes.WriteMultipleRegisters:
                case ModbusFunctionCodes.Diagnostics:
                    numBytes = 4;
                    break;
                default:
                    string msg = $"Function code {functionCode} not supported.";
                    Debug.WriteLine(msg);
                    throw new NotImplementedException(msg);
            }

            return numBytes;
        }

        public virtual byte[] Read(int count)
        {
            byte[] frameBytes = new byte[count];
            int numBytesRead = 0;

            while (numBytesRead != count)
            {
                numBytesRead += StreamResource.Read(frameBytes, numBytesRead, count - numBytesRead);
            }

            return frameBytes;
        }

        public override byte[] BuildMessageFrame(IModbusMessage message)
        {
            var messageFrame = message.MessageFrame;
            var crc = ModbusUtility.CalculateCrc(messageFrame);
            var messageBody = new MemoryStream(messageFrame.Length + crc.Length);

            messageBody.Write(messageFrame, 0, messageFrame.Length);
            messageBody.Write(crc, 0, crc.Length);

            return messageBody.ToArray();
        }

        internal override bool ChecksumsMatch(IModbusMessage message, byte[] messageFrame)
        {
            return BitConverter.ToUInt16(messageFrame, messageFrame.Length - 2) ==
                BitConverter.ToUInt16(ModbusUtility.CalculateCrc(message.MessageFrame), 0);
        }

        public override IModbusMessage ReadResponse<T>()
        {
            byte[] frameStart = Read(ResponseFrameStartLength);
            byte[] frameEnd = Read(ResponseBytesToRead(frameStart));
            byte[] frame = frameStart.Concat(frameEnd).ToArray();
            Debug.WriteLine($"RX: {string.Join(", ", frame)}");

            return CreateResponse<T>(frame);
        }

        public override byte[] ReadRequest()
        {
            byte[] frameStart = Read(RequestFrameStartLength);
            byte[] frameEnd = Read(RequestBytesToRead(frameStart));
            byte[] frame = frameStart.Concat(frameEnd).ToArray();
            Debug.WriteLine($"RX: {string.Join(", ", frame)}");

            return frame;
        }
    }
}
