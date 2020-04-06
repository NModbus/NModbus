using NModbus.Data;
using NModbus.Unme.Common;
using System;
using System.Linq;
using System.Net;

namespace NModbus.Message
{
    class WriteFileRecordResponse : AbstractModbusMessageWithData<FileRecordCollection>, IModbusMessage
    {
        public WriteFileRecordResponse()
        {
        }

        public WriteFileRecordResponse(byte slaveAddress)
            : base(slaveAddress, ModbusFunctionCodes.WriteFileRecord)
        {
        }

        public WriteFileRecordResponse(byte slaveAddress, FileRecordCollection data)
            : base(slaveAddress, ModbusFunctionCodes.WriteFileRecord)
        {
            Data = data;
        }

        public override int MinimumFrameSize => 10;

        public byte ByteCount
        {
            get => MessageImpl.ByteCount.Value;
            set => MessageImpl.ByteCount = value;
        }

        protected override void InitializeUnique(byte[] frame)
        {
            if (frame.Length < frame[2])
            {
                throw new FormatException("Message frame does not contain enough bytes.");
            }

            ByteCount = frame[2];
            Data = new FileRecordCollection(frame);
        }

        public override string ToString()
        {
            string msg = $"Wrote {Data.DataBytes.Count} bytes for file {Data.FileNumber} starting at address {Data.StartingAddress}.";
            return msg;
        }
    }
}
