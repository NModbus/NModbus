using System;
using System.Linq;
using System.Reflection;
using NModbus.Message;
using Xunit;

namespace NModbus.UnitTests.Message
{
    public class ModbusMessageFixture
    {
        [Fact]
        public void ProtocolDataUnitReadCoilsRequest()
        {
            AbstractModbusMessage message = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 1, 100, 9);
            byte[] expectedResult = { ModbusFunctionCodes.ReadCoils, 0, 100, 0, 9 };
            Assert.Equal(expectedResult, message.ProtocolDataUnit);
        }

        [Fact]
        public void MessageFrameReadCoilsRequest()
        {
            AbstractModbusMessage message = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 1, 2, 3);
            byte[] expectedMessageFrame = { 1, ModbusFunctionCodes.ReadCoils, 0, 2, 0, 3 };
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
                bool hasDirectOverride = messageType.GetMethod("ToString",
                  BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) != null;
                bool nonAbstractBaseHasOverride = messageType.BaseType != null &&
                                                  !messageType.BaseType.IsAbstract &&
                                                  messageType.BaseType.GetMethod("ToString",
                                                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly) != null;
                Assert.True(hasDirectOverride || nonAbstractBaseHasOverride);
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