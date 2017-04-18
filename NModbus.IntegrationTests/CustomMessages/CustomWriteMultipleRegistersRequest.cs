using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Modbus.Data;
using Modbus.Message;

namespace Modbus.IntegrationTests.CustomMessages
{
    public class CustomWriteMultipleRegistersRequest : IModbusMessage
    {
        private byte _functionCode;
        private byte _slaveAddress;
        private byte _byteCount;
        private ushort _startAddress;
        private ushort _numberOfPoints;
        private ushort _transactionId;
        private RegisterCollection _data;

        public CustomWriteMultipleRegistersRequest(byte functionCode, byte slaveAddress, ushort startAddress, RegisterCollection data)
        {
            _functionCode = functionCode;
            _slaveAddress = slaveAddress;
            _startAddress = startAddress;
            _numberOfPoints = (ushort)data.Count;
            _byteCount = data.ByteCount;
            _data = data;
        }

        public byte[] MessageFrame
        {
            get
            {
                List<byte> frame = new List<byte>();
                frame.Add(SlaveAddress);
                frame.AddRange(ProtocolDataUnit);

                return frame.ToArray();
            }
        }

        public byte[] ProtocolDataUnit
        {
            get
            {
                List<byte> pdu = new List<byte>();

                pdu.Add(FunctionCode);
                pdu.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)StartAddress)));
                pdu.AddRange(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)NumberOfPoints)));
                pdu.Add(ByteCount);
                pdu.AddRange(Data.NetworkBytes);

                return pdu.ToArray();
            }
        }

        public ushort TransactionId
        {
            get { return _transactionId; }
            set { _transactionId = value; }
        }

        public byte FunctionCode
        {
            get { return _functionCode; }
            set { _functionCode = value; }
        }

        public byte SlaveAddress
        {
            get { return _slaveAddress; }
            set { _slaveAddress = value; }
        }

        public ushort StartAddress
        {
            get { return _startAddress; }
            set { _startAddress = value; }
        }

        public ushort NumberOfPoints
        {
            get { return _numberOfPoints; }
            set { _numberOfPoints = value; }
        }

        public byte ByteCount
        {
            get { return _byteCount; }
            set { _byteCount = value; }
        }

        public RegisterCollection Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public void Initialize(byte[] frame)
        {
            if (frame == null)
            {
                throw new ArgumentNullException(nameof(frame));
            }

            if (frame.Length < 7 || frame.Length < 7 + frame[6])
            {
                throw new FormatException("Message frame does not contain enough bytes.");
            }

            SlaveAddress = frame[0];
            FunctionCode = frame[1];
            StartAddress = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
            NumberOfPoints = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4));
            ByteCount = frame[6];
            Data = new RegisterCollection(frame.Skip(7).Take(ByteCount).ToArray());
        }
    }
}
