using System;
using Modbus.Message;
using Xunit;

namespace Modbus.UnitTests.Message
{
    public class ReadCoilsInputsRequestFixture
    {
        [Fact]
        public void CreateReadCoilsRequest()
        {
            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 5, 1, 10);
            Assert.Equal(Modbus.ReadCoils, request.FunctionCode);
            Assert.Equal(5, request.SlaveAddress);
            Assert.Equal(1, request.StartAddress);
            Assert.Equal(10, request.NumberOfPoints);
        }

        [Fact]
        public void CreateReadInputsRequest()
        {
            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadInputs, 5, 1, 10);
            Assert.Equal(Modbus.ReadInputs, request.FunctionCode);
            Assert.Equal(5, request.SlaveAddress);
            Assert.Equal(1, request.StartAddress);
            Assert.Equal(10, request.NumberOfPoints);
        }

        [Fact]
        public void CreateReadCoilsInputsRequestTooMuchData()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 2, Modbus.MaximumDiscreteRequestResponseSize + 1));
        }

        [Fact]
        public void CreateReadCoilsInputsRequestMaxSize()
        {
            ReadCoilsInputsRequest response = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 2,
                Modbus.MaximumDiscreteRequestResponseSize);
            Assert.Equal(Modbus.MaximumDiscreteRequestResponseSize, response.NumberOfPoints);
        }

        [Fact]
        public void ToString_ReadCoilsRequest()
        {
            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 5, 1, 10);

            Assert.Equal("Read 10 coils starting at address 1.", request.ToString());
        }

        [Fact]
        public void ToString_ReadInputsRequest()
        {
            ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadInputs, 5, 1, 10);

            Assert.Equal("Read 10 inputs starting at address 1.", request.ToString());
        }
    }
}