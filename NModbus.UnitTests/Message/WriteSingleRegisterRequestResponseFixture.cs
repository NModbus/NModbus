using NModbus.Message;
using Xunit;

namespace NModbus.UnitTests.Message
{
    public class WriteSingleRegisterRequestResponseFixture
    {
        [Fact]
        public void NewWriteSingleRegisterRequestResponse()
        {
            WriteSingleRegisterRequestResponse message = new WriteSingleRegisterRequestResponse(12, 5, 1200);
            Assert.Equal(12, message.SlaveAddress);
            Assert.Equal(5, message.StartAddress);
            Assert.Single(message.Data);
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