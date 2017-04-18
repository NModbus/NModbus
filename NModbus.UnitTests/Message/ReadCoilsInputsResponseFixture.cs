using Modbus.Data;
using Modbus.Message;
using Xunit;

namespace Modbus.UnitTests.Message
{
    public class ReadCoilsInputsResponseFixture
    {
        [Fact]
        public void CreateReadCoilsResponse()
        {
            ReadCoilsInputsResponse response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 5, 2,
                new DiscreteCollection(true, true, true, true, true, true, false, false, true, true, false));
            Assert.Equal(Modbus.ReadCoils, response.FunctionCode);
            Assert.Equal(5, response.SlaveAddress);
            Assert.Equal(2, response.ByteCount);
            DiscreteCollection col = new DiscreteCollection(true, true, true, true, true, true, false, false, true, true,
                false);
            Assert.Equal(col.NetworkBytes, response.Data.NetworkBytes);
        }

        [Fact]
        public void CreateReadInputsResponse()
        {
            ReadCoilsInputsResponse response = new ReadCoilsInputsResponse(Modbus.ReadInputs, 5, 2,
                new DiscreteCollection(true, true, true, true, true, true, false, false, true, true, false));
            Assert.Equal(Modbus.ReadInputs, response.FunctionCode);
            Assert.Equal(5, response.SlaveAddress);
            Assert.Equal(2, response.ByteCount);
            DiscreteCollection col = new DiscreteCollection(true, true, true, true, true, true, false, false, true, true,
                false);
            Assert.Equal(col.NetworkBytes, response.Data.NetworkBytes);
        }

        [Fact]
        public void ToString_Coils()
        {
            ReadCoilsInputsResponse response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 5, 2,
                new DiscreteCollection(true, true, true, true, true, true, false, false, true, true, false));

            Assert.Equal("Read 11 coils - {1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0}.", response.ToString());
        }

        [Fact]
        public void ToString_Inputs()
        {
            ReadCoilsInputsResponse response = new ReadCoilsInputsResponse(Modbus.ReadInputs, 5, 2,
                new DiscreteCollection(true, true, true, true, true, true, false, false, true, true, false));

            Assert.Equal("Read 11 inputs - {1, 1, 1, 1, 1, 1, 0, 0, 1, 1, 0}.", response.ToString());
        }
    }
}