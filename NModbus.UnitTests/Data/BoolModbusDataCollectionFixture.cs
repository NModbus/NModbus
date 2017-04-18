using System.Collections.ObjectModel;
using Modbus.Data;
using Xunit;

namespace Modbus.UnitTests.Data
{
    public class BoolModbusDataCollectionFixture : ModbusDataCollectionFixture<bool>
    {
        [Fact]
        public void Remove_FromReadOnly()
        {
            bool[] source = { false, false, false, true, false, false };
            var col = new ModbusDataCollection<bool>(new ReadOnlyCollection<bool>(source));
            int expectedCount = source.Length;

            Assert.True(col.Remove(source[3]));

            Assert.Equal(expectedCount, col.Count);
        }

        protected override bool[] GetArray()
        {
            return new[] { false, false, true, false, false };
        }

        protected override bool GetNonExistentElement()
        {
            return true;
        }
    }
}
