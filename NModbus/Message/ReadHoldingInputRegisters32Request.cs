using System.Diagnostics;
using System.IO;

namespace NModbus.Message
{
    public class ReadHoldingInputRegisters32Request : ReadHoldingInputRegistersRequest
    {
        public ReadHoldingInputRegisters32Request()
        {
        }

        public ReadHoldingInputRegisters32Request(byte functionCode, byte slaveAddress, ushort startAddress, ushort numberOfPoints)
            : base(functionCode, slaveAddress, startAddress, numberOfPoints)
        {
            StartAddress = startAddress;
            NumberOfPoints = numberOfPoints;
        }

        public override void ValidateResponse(IModbusMessage response)
        {
            var typedResponse = response as ReadHoldingInputRegistersResponse;
            Debug.Assert(typedResponse != null, "Argument response should be of type ReadHoldingInputRegistersResponse.");
            var expectedByteCount = NumberOfPoints * 4;

            if (expectedByteCount != typedResponse.ByteCount)
            {
                string msg = $"Unexpected byte count. Expected {expectedByteCount}, received {typedResponse.ByteCount}.";
                throw new IOException(msg);
            }
        }

        public override string ToString()
        {
            string msg = $"Read {NumberOfPoints} {(FunctionCode == ModbusFunctionCodes.ReadHoldingRegisters ? "holding" : "input")} registers starting at address {StartAddress}.";
            return msg;
        }
    }
}
