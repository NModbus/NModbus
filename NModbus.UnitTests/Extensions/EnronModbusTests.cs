using NModbus.Extensions.Enron;
using Xunit;

namespace NModbus.UnitTests.Extensions
{
    public class EnronModbusTests
    {

        [Theory]
        [InlineData(new uint[] { 0x01234567 }, new ushort[] { 0x0123, 0x4567 })]
        public void ConvertFrom32(uint[] input, ushort[] expected)
        {
            var registers = EnronModbus.ConvertFrom32(input);

            Assert.Equal(expected, registers);
        }

        [Theory]
        [InlineData(new ushort[] { 0x0123, 0x4567 }, new uint[] { 0x01234567 })]
        public void ConvertTo32(ushort[] input, uint[] expected)
        {
            var registers = EnronModbus.ConvertTo32(input);

            Assert.Equal(expected, registers);
        }
    }
}
