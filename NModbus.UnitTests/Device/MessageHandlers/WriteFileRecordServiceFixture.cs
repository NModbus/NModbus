using NModbus.Device.MessageHandlers;
using Xunit;

namespace NModbus.UnitTests.Device.MessageHandlers
{
    public class WriteFileRecordServiceFixture
    {
        [Fact]
        public void GetRtuRequestBytesToRead()
        {
            var service = new WriteFileRecordService();
            Assert.Equal(4, service.GetRtuRequestBytesToRead(new byte[] { 1, 21, 3 }));
        }

        [Fact]
        public void GetRtuResponseBytesToRead()
        {
            var service = new WriteFileRecordService();
            Assert.Equal(45, service.GetRtuResponseBytesToRead(new byte[] { 1, 21, 44 }));
        }
    }
}
