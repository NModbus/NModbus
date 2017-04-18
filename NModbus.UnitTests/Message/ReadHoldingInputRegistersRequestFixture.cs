using System;
using Modbus.Message;
using Xunit;

namespace Modbus.UnitTests.Message
{
    public class ReadHoldingInputRegistersRequestFixture
    {
        [Fact]
        public void CreateReadHoldingRegistersRequest()
        {
            ReadHoldingInputRegistersRequest request = new ReadHoldingInputRegistersRequest(
                Modbus.ReadHoldingRegisters, 5, 1, 10);
            Assert.Equal(Modbus.ReadHoldingRegisters, request.FunctionCode);
            Assert.Equal(5, request.SlaveAddress);
            Assert.Equal(1, request.StartAddress);
            Assert.Equal(10, request.NumberOfPoints);
        }

        [Fact]
        public void CreateReadInputRegistersRequest()
        {
            ReadHoldingInputRegistersRequest request = new ReadHoldingInputRegistersRequest(Modbus.ReadInputRegisters, 5,
                1, 10);
            Assert.Equal(Modbus.ReadInputRegisters, request.FunctionCode);
            Assert.Equal(5, request.SlaveAddress);
            Assert.Equal(1, request.StartAddress);
            Assert.Equal(10, request.NumberOfPoints);
        }

        [Fact]
        public void CreateReadHoldingInputRegistersRequestTooMuchData()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadHoldingInputRegistersRequest(Modbus.ReadHoldingRegisters, 1, 2,
                Modbus.MaximumRegisterRequestResponseSize + 1));
        }

        [Fact]
        public void CreateReadHoldingInputRegistersRequestMaxSize()
        {
            ReadHoldingInputRegistersRequest response = new ReadHoldingInputRegistersRequest(
                Modbus.ReadHoldingRegisters, 1, 2, Modbus.MaximumRegisterRequestResponseSize);
            Assert.Equal(Modbus.MaximumRegisterRequestResponseSize, response.NumberOfPoints);
        }

        [Fact]
        public void ToString_ReadHoldingRegistersRequest()
        {
            ReadHoldingInputRegistersRequest request = new ReadHoldingInputRegistersRequest(
                Modbus.ReadHoldingRegisters, 5, 1, 10);

            Assert.Equal("Read 10 holding registers starting at address 1.", request.ToString());
        }

        [Fact]
        public void ToString_ReadInputRegistersRequest()
        {
            ReadHoldingInputRegistersRequest request = new ReadHoldingInputRegistersRequest(Modbus.ReadInputRegisters, 5,
                1, 10);

            Assert.Equal("Read 10 input registers starting at address 1.", request.ToString());
        }
    }
}