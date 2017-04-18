using System;
using System.Linq;
using System.Reflection;
using Modbus.Message;
using Xunit;

namespace Modbus.UnitTests.Message
{
    public class ModbusMessageFixture
    {
        [Fact]
        public void ProtocolDataUnitReadCoilsRequest()
        {
            AbstractModbusMessage message = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 100, 9);
            byte[] expectedResult = { Modbus.ReadCoils, 0, 100, 0, 9 };
            Assert.Equal(expectedResult, message.ProtocolDataUnit);
        }

        [Fact]
        public void MessageFrameReadCoilsRequest()
        {
            AbstractModbusMessage message = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 2, 3);
            byte[] expectedMessageFrame = { 1, Modbus.ReadCoils, 0, 2, 0, 3 };
            Assert.Equal(expectedMessageFrame, message.MessageFrame);
        }

        [Fact]
        public void ModbusMessageToStringOverriden()
        {
            var messageTypes = from message in typeof(AbstractModbusMessage).GetTypeInfo().Assembly.GetTypes()
                let typeInfo = message.GetTypeInfo()
                where !typeInfo.IsAbstract && typeInfo.IsSubclassOf(typeof(AbstractModbusMessage))
                select message;

            foreach (Type messageType in messageTypes)
            {
                Assert.NotNull(
                    messageType.GetMethod("ToString",
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly));
            }
        }

        internal static void AssertModbusMessagePropertiesAreEqual(IModbusMessage obj1, IModbusMessage obj2)
        {
            Assert.Equal(obj1.FunctionCode, obj2.FunctionCode);
            Assert.Equal(obj1.SlaveAddress, obj2.SlaveAddress);
            Assert.Equal(obj1.MessageFrame, obj2.MessageFrame);
            Assert.Equal(obj1.ProtocolDataUnit, obj2.ProtocolDataUnit);
        }
    }
}