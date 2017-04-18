using System;
using System.Linq;
using Modbus.Data;
using Xunit;

namespace Modbus.UnitTests.Data
{
    public class DataStoreEventArgsFixture
    {
        [Fact]
        public void CreateDataStoreEventArgs()
        {
            var eventArgs = DataStoreEventArgs.CreateDataStoreEventArgs(5, ModbusDataType.HoldingRegister,
                new ushort[] { 1, 2, 3 });
            Assert.Equal(ModbusDataType.HoldingRegister, eventArgs.ModbusDataType);
            Assert.Equal(5, eventArgs.StartAddress);
            Assert.Equal(new ushort[] { 1, 2, 3 }, eventArgs.Data.B.ToArray());
        }

        [Fact]
        public void CreateDataStoreEventArgs_InvalidType()
        {
            Assert.Throws<ArgumentException>(() => DataStoreEventArgs.CreateDataStoreEventArgs(5, ModbusDataType.HoldingRegister,
                new int[] { 1, 2, 3 }));
        }

        [Fact]
        public void CreateDataStoreEventArgs_DataNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
                DataStoreEventArgs.CreateDataStoreEventArgs(5, ModbusDataType.HoldingRegister,
                    default(ushort[])));
        }
    }
}