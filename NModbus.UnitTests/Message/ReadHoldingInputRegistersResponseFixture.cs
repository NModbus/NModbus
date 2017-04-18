using System;
using Modbus.Data;
using Modbus.Message;
using Xunit;

namespace Modbus.UnitTests.Message
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
                new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 5, new RegisterCollection(1, 2));
            Assert.Equal(Modbus.ReadHoldingRegisters, response.FunctionCode);
            Assert.Equal(5, response.SlaveAddress);
            Assert.Equal(4, response.ByteCount);
            RegisterCollection col = new RegisterCollection(1, 2);
            Assert.Equal(col.NetworkBytes, response.Data.NetworkBytes);
        }

        [Fact]
        public void ToString_ReadHoldingRegistersResponse()
        {
            ReadHoldingInputRegistersResponse response =
                new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection(1));
            Assert.Equal("Read 1 holding registers.", response.ToString());
        }

        [Fact]
        public void ReadInputRegistersResponse()
        {
            ReadHoldingInputRegistersResponse response = new ReadHoldingInputRegistersResponse(
                Modbus.ReadInputRegisters, 5, new RegisterCollection(1, 2));
            Assert.Equal(Modbus.ReadInputRegisters, response.FunctionCode);
            Assert.Equal(5, response.SlaveAddress);
            Assert.Equal(4, response.ByteCount);
            RegisterCollection col = new RegisterCollection(1, 2);
            Assert.Equal(col.NetworkBytes, response.Data.NetworkBytes);
        }

        [Fact]
        public void ToString_ReadInputRegistersResponse()
        {
            ReadHoldingInputRegistersResponse response = new ReadHoldingInputRegistersResponse(
                Modbus.ReadInputRegisters, 1, new RegisterCollection(1));
            Assert.Equal("Read 1 input registers.", response.ToString());
        }
    }
}