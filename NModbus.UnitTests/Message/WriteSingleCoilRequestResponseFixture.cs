using Modbus.Message;
using Xunit;

namespace Modbus.UnitTests.Message
{
    public class WriteSingleCoilRequestResponseFixture
    {
        [Fact]
        public void NewWriteSingleCoilRequestResponse()
        {
            WriteSingleCoilRequestResponse request = new WriteSingleCoilRequestResponse(11, 5, true);
            Assert.Equal(11, request.SlaveAddress);
            Assert.Equal(5, request.StartAddress);
            Assert.Equal(1, request.Data.Count);
            Assert.Equal(Modbus.CoilOn, request.Data[0]);
        }

        [Fact]
        public void ToString_True()
        {
            var request = new WriteSingleCoilRequestResponse(11, 5, true);

            Assert.Equal("Write single coil 1 at address 5.", request.ToString());
        }

        [Fact]
        public void ToString_False()
        {
            var request = new WriteSingleCoilRequestResponse(11, 5, false);

            Assert.Equal("Write single coil 0 at address 5.", request.ToString());
        }
    }
}