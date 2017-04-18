using NModbus.Data;
using NModbus.Message;
using Xunit;

namespace NModbus.UnitTests.Message
{
    public class DiagnosticsRequestResponseFixture
    {
        [Fact]
        public void ToString_Test()
        {
            DiagnosticsRequestResponse response;

            response = new DiagnosticsRequestResponse(Modbus.DiagnosticsReturnQueryData, 3, new RegisterCollection(5));
            Assert.Equal("Diagnostics message, sub-function return query data - {5}.", response.ToString());
        }
    }
}