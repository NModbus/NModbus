using System;
using NModbus.Data;
using NModbus.Message;
using Xunit;

namespace NModbus.UnitTests.Message
{
    public class WriteMultipleRegistersRequestFixture
    {
        [Fact]
        public void CreateWriteMultipleRegistersRequestFixture()
        {
            RegisterCollection col = new RegisterCollection(10, 20, 30, 40, 50);
            WriteMultipleRegistersRequest request = new WriteMultipleRegistersRequest(11, 34, col);
            Assert.Equal(ModbusFunctionCodes.WriteMultipleRegisters, request.FunctionCode);
            Assert.Equal(11, request.SlaveAddress);
            Assert.Equal(34, request.StartAddress);
            Assert.Equal(10, request.ByteCount);
            Assert.Equal(col.NetworkBytes, request.Data.NetworkBytes);
        }

        [Fact]
        public void CreateWriteMultipleRegistersRequestTooMuchData()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new WriteMultipleRegistersRequest(1, 2,
                MessageUtility.CreateDefaultCollection<RegisterCollection, ushort>(3, Modbus.MaximumRegisterRequestResponseSize + 1)));
        }

        [Fact]
        public void CreateWriteMultipleRegistersRequestMaxSize()
        {
            WriteMultipleRegistersRequest request = new WriteMultipleRegistersRequest(1, 2,
                MessageUtility.CreateDefaultCollection<RegisterCollection, ushort>(3, Modbus.MaximumRegisterRequestResponseSize));
            Assert.Equal(Modbus.MaximumRegisterRequestResponseSize, request.NumberOfPoints);
        }

        [Fact]
        public void ToString_WriteMultipleRegistersRequest()
        {
            RegisterCollection col = new RegisterCollection(10, 20, 30, 40, 50);
            WriteMultipleRegistersRequest request = new WriteMultipleRegistersRequest(11, 34, col);

            Assert.Equal("Write 5 holding registers starting at address 34.", request.ToString());
        }
    }
}