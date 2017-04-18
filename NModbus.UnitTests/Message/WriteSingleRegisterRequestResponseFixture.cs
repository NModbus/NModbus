using Modbus.Message;
using Xunit;

namespace Modbus.UnitTests.Message
{
    public class WriteSingleRegisterRequestResponseFixture
    {
        [Fact]
        public void NewWriteSingleRegisterRequestResponse()
        {
            WriteSingleRegisterRequestResponse message = new WriteSingleRegisterRequestResponse(12, 5, 1200);
            Assert.Equal(12, message.SlaveAddress);
            Assert.Equal(5, message.StartAddress);
            Assert.Equal(1, message.Data.Count);
            Assert.Equal(1200, message.Data[0]);
        }

        [Fact]
        public void ToStringOverride()
        {
            WriteSingleRegisterRequestResponse message = new WriteSingleRegisterRequestResponse(12, 5, 1200);
            Assert.Equal("Write single holding register 1200 at address 5.", message.ToString());
        }
    }
}