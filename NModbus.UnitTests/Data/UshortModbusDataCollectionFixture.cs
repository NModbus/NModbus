using System.Collections.ObjectModel;
using Modbus.Data;
using Xunit;

namespace Modbus.UnitTests.Data
{
    public class UshortModbusDataCollectionFixture : ModbusDataCollectionFixture<ushort>
    {
        [Fact]
        public void Remove_FromReadOnly()
        {
            ushort[] source = this.GetArray();
            var col = new ModbusDataCollection<ushort>(new ReadOnlyCollection<ushort>(source));
            int expectedCount = source.Length;

            Assert.False(col.Remove(this.GetNonExistentElement()));
            Assert.True(col.Remove(source[3]));

            Assert.Equal(expectedCount, col.Count);
        }

        protected override ushort[] GetArray()
        {
            return new ushort[] { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        }

        protected override ushort GetNonExistentElement()
        {
            return 42;
        }
    }
}
