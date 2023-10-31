using System.Diagnostics;
using System.IO;
using NModbus.Logging;

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
    }
}
