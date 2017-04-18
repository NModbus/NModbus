using System.IO;
#if NET46
using System.Runtime.Serialization.Formatters.Binary;
#endif
using Xunit;

namespace Modbus.UnitTests
{
    public class InvalidModbusRequestExceptionFixture
    {
        [Fact]
        public void ConstructorWithExceptionCode()
        {
            var e = new InvalidModbusRequestException(Modbus.SlaveDeviceBusy);
            Assert.Equal($"Modbus exception code {Modbus.SlaveDeviceBusy}.", e.Message);
            Assert.Equal(Modbus.SlaveDeviceBusy, e.ExceptionCode);
            Assert.Null(e.InnerException);
        }

        [Fact]
        public void ConstructorWithExceptionCodeAndInnerException()
        {
            var inner = new IOException("Bar");
            var e = new InvalidModbusRequestException(42, inner);
            Assert.Equal("Modbus exception code 42.", e.Message);
            Assert.Equal(42, e.ExceptionCode);
            Assert.Same(inner, e.InnerException);
        }

        [Fact]
        public void ConstructorWithMessageAndExceptionCode()
        {
            var e = new InvalidModbusRequestException("Hello World", Modbus.IllegalFunction);
            Assert.Equal("Hello World", e.Message);
            Assert.Equal(Modbus.IllegalFunction, e.ExceptionCode);
            Assert.Null(e.InnerException);
        }

        [Fact]
        public void ConstructorWithCustomMessageAndSlaveExceptionResponse()
        {
            var inner = new IOException("Bar");
            var e = new InvalidModbusRequestException("Hello World", Modbus.IllegalDataAddress, inner);
            Assert.Equal("Hello World", e.Message);
            Assert.Equal(Modbus.IllegalDataAddress, e.ExceptionCode);
            Assert.Same(inner, e.InnerException);
        }

#if NET46
        [Fact]
        public void Serializable()
        {
            var formatter = new BinaryFormatter();
            var e = new InvalidModbusRequestException(Modbus.SlaveDeviceBusy);

            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, e);
                stream.Position = 0;

                var e2 = (InvalidModbusRequestException)formatter.Deserialize(stream);
                Assert.NotNull(e2);
                Assert.Equal(Modbus.SlaveDeviceBusy, e2.ExceptionCode);
                Assert.Equal($"Modbus exception code {Modbus.SlaveDeviceBusy}.", e2.Message);
            }
        }
#endif
    }
}
