using System.Diagnostics;
using System.IO;
using NModbus.Logging;
using NModbus.Message;

namespace NModbus.IO
{
    /// <summary>
    ///     Transport for Serial protocols.
    ///     Refined Abstraction - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    public abstract class ModbusSerialTransport : ModbusTransport, IModbusSerialTransport
    {
        private bool _checkFrame = true;

        internal ModbusSerialTransport(IStreamResource streamResource, IModbusFactory modbusFactory, IModbusLogger logger)
            : base(streamResource, modbusFactory, logger)
        {
            Debug.Assert(streamResource != null, "Argument streamResource cannot be null.");
        }

        /// <summary>
        /// Gets or sets a value indicating whether LRC/CRC frame checking is performed on messages.
        /// </summary>
        public bool CheckFrame
        {
            get => _checkFrame;
            set => _checkFrame = value;
        }

        public void DiscardInBuffer()
        {
            StreamResource.DiscardInBuffer();
        }

        public override void Write(IModbusMessage message)
        {
            DiscardInBuffer();

            byte[] frame = BuildMessageFrame(message);

            Logger.LogFrameTx(frame);
            
            StreamResource.Write(frame, 0, frame.Length);
        }

        public override IModbusMessage CreateResponse<T>(byte[] frame)
        {
            IModbusMessage response = base.CreateResponse<T>(frame);

            // compare checksum
            if (CheckFrame && !ChecksumsMatch(response, frame))
            {
                string msg = $"Checksums failed to match {string.Join(", ", response.MessageFrame)} != {string.Join(", ", frame)}";
                Logger.Warning(msg);
                throw new IOException(msg);
            }

            return response;
        }

        public abstract void IgnoreResponse();

        public abstract bool ChecksumsMatch(IModbusMessage message, byte[] messageFrame);

        internal override void OnValidateResponse(IModbusMessage request, IModbusMessage response)
        {
            // no-op
        }

        public override bool ShouldRetryResponse(IModbusMessage request, IModbusMessage response)
        {
            if (request.FunctionCode != response.FunctionCode)
            {
                return true;
            }

            if (request.SlaveAddress != 0)
            {
                if (response.SlaveAddress != request.SlaveAddress)
                {
                    return true;
                }
            }
            else
            {
                if (response.SlaveAddress < 1 || response.SlaveAddress > 247)
                {
                    return true;
                }
            }
            return false;
        }

        public override void ValidateResponse(IModbusMessage request, IModbusMessage response)
        {
            // always check the function code and slave address, regardless of transport protocol
            if (request.FunctionCode != response.FunctionCode)
            {
                string msg = $"Received response with unexpected Function Code. Expected {request.FunctionCode}, received {response.FunctionCode}.";
                throw new IOException(msg);
            }

            // Validate slave address
            if (request.SlaveAddress != 0)
            {
                // Normal (non-broadcast) request: response address must match and be valid
                if (response.SlaveAddress != request.SlaveAddress)
                {
                    string msg = $"Response slave address does not match request. Expected {request.SlaveAddress}, received {response.SlaveAddress}.";
                    throw new IOException(msg);
                }
            }
            else
            {
                // Broadcast request: only check that the response slave address is in valid range
                if (response.SlaveAddress < 1 || response.SlaveAddress > 247)
                {
                    string msg = $"Response slave address {response.SlaveAddress} is out of valid range (1~247) for a broadcast request.";
                    throw new IOException(msg);
                }
            }

            // message specific validation
            var req = request as IModbusRequest;

            if (req != null)
            {
                req.ValidateResponse(response);
            }

            OnValidateResponse(request, response);
        }
    }
}
