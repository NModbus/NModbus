using System;
using NModbus.Data;
using NModbus.Message;
using Xunit;

namespace NModbus.UnitTests.Message
{
    public class ReadHoldingInputRegistersResponseFixture
    {
        [Fact]
        public void ReadHoldingInputRegistersResponse_NullData()
        {
            Assert.Throws<ArgumentNullException>(() => new ReadHoldingInputRegistersResponse(0, 0, null));
        }

        [Fact]
        public void ReadHoldingRegistersResponse()
        {
            ReadHoldingInputRegistersResponse response =
                new ReadHoldingInputRegistersResponse(ModbusFunctionCodes.ReadHoldingRegisters, 5, new RegisterCollection(1, 2));
            Assert.Equal(ModbusFunctionCodes.ReadHoldingRegisters, response.FunctionCode);
            Assert.Equal(5, response.SlaveAddress);
            Assert.Equal(4, response.ByteCount);
            RegisterCollection col = new RegisterCollection(1, 2);
            Assert.Equal(col.NetworkBytes, response.Data.NetworkBytes);
        }

        [Fact]
        public void ToString_ReadHoldingRegistersResponse()
        {
            ReadHoldingInputRegistersResponse response =
                new ReadHoldingInputRegistersResponse(ModbusFunctionCodes.ReadHoldingRegisters, 1, new RegisterCollection(1));
            Assert.Equal("Read 1 holding registers.", response.ToString());
        }

        [Fact]
        public void ReadInputRegistersResponse()
        {
            ReadHoldingInputRegistersResponse response = new ReadHoldingInputRegistersResponse(
                ModbusFunctionCodes.ReadInputRegisters, 5, new RegisterCollection(1, 2));
            Assert.Equal(ModbusFunctionCodes.ReadInputRegisters, response.FunctionCode);
            Assert.Equal(5, response.SlaveAddress);
            Assert.Equal(4, response.ByteCount);
            RegisterCollection col = new RegisterCollection(1, 2);
            Assert.Equal(col.NetworkBytes, response.Data.NetworkBytes);
        }

        [Fact]
        public void ToString_ReadInputRegistersResponse()
        {
            ReadHoldingInputRegistersResponse response = new ReadHoldingInputRegistersResponse(
                ModbusFunctionCodes.ReadInputRegisters, 1, new RegisterCollection(1));
            Assert.Equal("Read 1 input registers.", response.ToString());
        }
    }
}