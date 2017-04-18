using Modbus.Extensions.Enron;
using Xunit;

namespace Modbus.IntegrationTests
{
    internal class EnronFixture : NModbusSerialRtuMasterDl06SlaveFixture
    {
        [Fact]
        public virtual void ReadHoldingRegisters32()
        {
            uint[] registers = Master.ReadHoldingRegisters32(SlaveAddress, 104, 2);
            Assert.Equal(new uint[] { 0, 0 }, registers);
        }

        [Fact]
        public virtual void ReadInputRegisters32()
        {
            uint[] registers = Master.ReadInputRegisters32(SlaveAddress, 104, 2);
            Assert.Equal(new uint[] { 0, 0 }, registers);
        }

        [Fact]
        public virtual void WriteSingleRegister32()
        {
            ushort testAddress = 200;
            uint testValue = 350;

            uint originalValue = Master.ReadHoldingRegisters32(SlaveAddress, testAddress, 1)[0];
            Master.WriteSingleRegister32(SlaveAddress, testAddress, testValue);
            Assert.Equal(testValue, Master.ReadHoldingRegisters32(SlaveAddress, testAddress, 1)[0]);
            Master.WriteSingleRegister32(SlaveAddress, testAddress, originalValue);
            Assert.Equal(originalValue, Master.ReadHoldingRegisters(SlaveAddress, testAddress, 1)[0]);
        }

        [Fact]
        public virtual void WriteMultipleRegisters32()
        {
            ushort testAddress = 120;
            uint[] testValues = new uint[] { 10, 20, 30, 40, 50 };

            uint[] originalValues = Master.ReadHoldingRegisters32(SlaveAddress, testAddress, (ushort)testValues.Length);
            Master.WriteMultipleRegisters32(SlaveAddress, testAddress, testValues);
            uint[] newValues = Master.ReadHoldingRegisters32(SlaveAddress, testAddress, (ushort)testValues.Length);
            Assert.Equal(testValues, newValues);
            Master.WriteMultipleRegisters32(SlaveAddress, testAddress, originalValues);
        }
    }
}
