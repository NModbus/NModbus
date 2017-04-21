using NModbus.Message;
using Xunit;

namespace NModbus.UnitTests.Message
{
    public class SlaveExceptionResponseFixture
    {
        [Fact]
        public void CreateSlaveExceptionResponse()
        {
            SlaveExceptionResponse response = new SlaveExceptionResponse(11, ModbusFunctionCodes.ReadCoils + Modbus.ExceptionOffset,
                2);
            Assert.Equal(11, response.SlaveAddress);
            Assert.Equal(ModbusFunctionCodes.ReadCoils + Modbus.ExceptionOffset, response.FunctionCode);
            Assert.Equal(2, response.SlaveExceptionCode);
        }

        [Fact]
        public void SlaveExceptionResponsePDU()
        {
            SlaveExceptionResponse response = new SlaveExceptionResponse(11, ModbusFunctionCodes.ReadCoils + Modbus.ExceptionOffset,
                2);
            Assert.Equal(new byte[] { response.FunctionCode, response.SlaveExceptionCode }, response.ProtocolDataUnit);
        }
    }
}