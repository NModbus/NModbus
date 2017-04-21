using NModbus.Data;
using NModbus.Message;
using Xunit;

namespace NModbus.UnitTests.Message
{
    public class ModbusMessageWithDataFixture
    {
        [Fact]
        public void ModbusMessageWithDataFixtureCtorInitializesProperties()
        {
            AbstractModbusMessageWithData<DiscreteCollection> message = new ReadCoilsInputsResponse(ModbusFunctionCodes.ReadCoils, 10, 1,
                new DiscreteCollection(true, false, true));
            Assert.Equal(ModbusFunctionCodes.ReadCoils, message.FunctionCode);
            Assert.Equal(10, message.SlaveAddress);
        }

        [Fact]
        public void ProtocolDataUnitReadCoilsResponse()
        {
            AbstractModbusMessageWithData<DiscreteCollection> message = new ReadCoilsInputsResponse(ModbusFunctionCodes.ReadCoils, 1, 2,
                new DiscreteCollection(true));
            byte[] expectedResult = { 1, 2, 1 };
            Assert.Equal(expectedResult, message.ProtocolDataUnit);
        }

        [Fact]
        public void DataReadCoilsResponse()
        {
            DiscreteCollection col = new DiscreteCollection(false, true, false, true, false, true, false, false, false,
                false);
            AbstractModbusMessageWithData<DiscreteCollection> message = new ReadCoilsInputsResponse(ModbusFunctionCodes.ReadCoils, 11, 1, col);
            Assert.Equal(col.Count, message.Data.Count);
            Assert.Equal(col.NetworkBytes, message.Data.NetworkBytes);
        }
    }
}