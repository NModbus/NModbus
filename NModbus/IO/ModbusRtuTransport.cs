using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NModbus.Extensions;
using NModbus.Logging;
using NModbus.Utility;

namespace NModbus.IO
{
    /// <summary>
    ///     Refined Abstraction - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    internal class ModbusRtuTransport : ModbusSerialTransport, IModbusRtuTransport
    {
        public const int RequestFrameStartLength = 7;

        public const int ResponseFrameStartLength = 4;

        internal ModbusRtuTransport(IStreamResource streamResource, IModbusFactory modbusFactory, IModbusLogger logger)
            : base(streamResource, modbusFactory, logger)
        {
            if (modbusFactory == null) throw new ArgumentNullException(nameof(modbusFactory));
            Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
        }

        internal int RequestBytesToRead(byte[] frameStart)
        {
            byte functionCode = frameStart[1];

            IModbusFunctionService service = ModbusFactory.GetFunctionServiceOrThrow(functionCode);
                
            return service.GetRtuRequestBytesToRead(frameStart);
        }

        internal int ResponseBytesToRead(byte[] frameStart)
        {
            byte functionCode = frameStart[1];

            if (functionCode > Modbus.ExceptionOffset)
            {
                return 1;
            }

            IModbusFunctionService service = ModbusFactory.GetFunctionServiceOrThrow(functionCode);

            return service.GetRtuResponseBytesToRead(frameStart);
        }

        public virtual byte[] Read(int count)
        {
            byte[] frameBytes = new byte[count];
            int numBytesReadTotal = 0;

            while (numBytesReadTotal != count)
            {
                int numBytesRead = StreamResource.Read(frameBytes, numBytesReadTotal, count - numBytesReadTotal);
                
                if (numBytesRead == 0)
                {
                    throw new IOException("Read resulted in 0 bytes returned.");
                }
                
                numBytesReadTotal += numBytesRead;
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

        public override bool ChecksumsMatch(IModbusMessage message, byte[] messageFrame)
        {
            ushort messageCrc = BitConverter.ToUInt16(messageFrame, messageFrame.Length - 2);
            ushort calculatedCrc = BitConverter.ToUInt16(ModbusUtility.CalculateCrc(message.MessageFrame), 0);

            return messageCrc == calculatedCrc;
        }

        public override IModbusMessage ReadResponse<T>(IModbusMessage request)
        {
            byte[] frame = ReadResponse(request);

            Logger.LogFrameRx(frame);

            return CreateResponse<T>(frame);
        }

        private byte[] ReadResponse(IModbusMessage request)
        {
            byte[] frameStart = request != null ? 
                ReadStartFrameResponse(request) : 
                Read(ResponseFrameStartLength);
            byte[] frameEnd = Read(ResponseBytesToRead(frameStart));
            byte[] frame = frameStart.Concat(frameEnd).ToArray();

            return frame;
        }

        public override void IgnoreResponse()
        {
            byte[] frame = ReadResponse(null);

            Logger.LogFrameIgnoreRx(frame);
        }

        public override byte[] ReadRequest()
        {
            byte[] frameStart = Read(RequestFrameStartLength);
            byte[] frameEnd = Read(RequestBytesToRead(frameStart));
            byte[] frame = frameStart.Concat(frameEnd).ToArray();

            Logger.LogFrameRx(frame);

            return frame;
        }

        private byte[] ReadStartFrameResponse(IModbusMessage request)
        {
            const int HeaderLength = 2;
            int frameHeadLength = ResponseFrameStartLength;
            byte functionCode = request.FunctionCode;
            byte slaveId = request.SlaveAddress;
            int maxGarbageBytes = 1024;

            byte[] window = new byte[frameHeadLength];
            int count = 0;
            int garbageCount = 0;

            while (true)
            {
                // Fill the sliding window buffer
                while (count < frameHeadLength)
                {
                    byte[] b = Read(1);
                    window[count++] = b[0];
                }

                int headIdx = -1;
                if (slaveId == 0)
                {
                    // Broadcast request: look for [1-247, functionCode] as valid header
                    for (int i = 0; i <= frameHeadLength - HeaderLength; i++)
                    {
                        if (window[i] >= 1 && window[i] <= 247 && window[i + 1] == functionCode)
                        {
                            headIdx = i;
                            break;
                        }
                    }
                }
                else
                {
                    // Exact match: look for [slaveId, functionCode]
                    if (window[0] == slaveId && window[1] == functionCode)
                        headIdx = 0;
                }

                if (headIdx >= 0)
                {
                    // Extract the frame header starting from headIdx
                    byte[] frameHead = new byte[frameHeadLength];
                    Array.Copy(window, headIdx, frameHead, 0, frameHeadLength - headIdx);

                    // If there was garbage before the header, read extra bytes to complete the header
                    int bytesNeeded = headIdx;
                    if (bytesNeeded > 0)
                    {
                        byte[] rest = Read(bytesNeeded);
                        Array.Copy(rest, 0, frameHead, frameHeadLength - bytesNeeded, bytesNeeded);
                    }
                    return frameHead;
                }

                // Slide the window: shift left by one byte, decrease count accordingly
                Array.Copy(window, 1, window, 0, frameHeadLength - 1);
                count = frameHeadLength - 1;
                garbageCount++;
                if (garbageCount > maxGarbageBytes)
                    throw new IOException("Too many garbage bytes, failed to find Modbus RTU frame header.");
            }
        }
    }
}
