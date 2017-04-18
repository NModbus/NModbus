using System;
using Modbus.Message;
using Xunit;

namespace Modbus.UnitTests.Message
{
    public class WriteMultipleCoilsResponseFixture
    {
        [Fact]
        public void CreateWriteMultipleCoilsResponse()
        {
            WriteMultipleCoilsResponse response = new WriteMultipleCoilsResponse(17, 19, 45);
            Assert.Equal(Modbus.WriteMultipleCoils, response.FunctionCode);
            Assert.Equal(17, response.SlaveAddress);
            Assert.Equal(19, response.StartAddress);
            Assert.Equal(45, response.NumberOfPoints);
        }

        [Fact]
        public void CreateWriteMultipleCoilsResponseTooMuchData()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new WriteMultipleCoilsResponse(1, 2, Modbus.MaximumDiscreteRequestResponseSize + 1));
        }

        [Fact]
        public void CreateWriteMultipleCoilsResponseMaxSize()
        {
            WriteMultipleCoilsResponse response = new WriteMultipleCoilsResponse(1, 2,
                Modbus.MaximumDiscreteRequestResponseSize);
            Assert.Equal(Modbus.MaximumDiscreteRequestResponseSize, response.NumberOfPoints);
        }

        [Fact]
        public void ToString_Test()
        {
            var response = new WriteMultipleCoilsResponse(1, 2, 3);

            Assert.Equal("Wrote 3 coils starting at address 2.", response.ToString());
        }
    }
}