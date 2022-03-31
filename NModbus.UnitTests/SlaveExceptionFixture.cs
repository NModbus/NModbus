using System;
using System.IO;
using NModbus.Message;
using Xunit;
#if NET46
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace NModbus.UnitTests
{
    public class SlaveExceptionFixture
    {
        [Fact]
        public void EmptyConstructor()
        {
            var e = new ServerException();
            Assert.Equal($"Exception of type '{typeof(ServerException).FullName}' was thrown.", e.Message);
            Assert.Equal(0, e.SlaveAddress);
            Assert.Equal(0, e.FunctionCode);
            Assert.Equal(0, e.SlaveExceptionCode);
            Assert.Null(e.InnerException);
        }

        [Fact]
        public void ConstructorWithMessage()
        {
            var e = new ServerException("Hello World");
            Assert.Equal("Hello World", e.Message);
            Assert.Equal(0, e.SlaveAddress);
            Assert.Equal(0, e.FunctionCode);
            Assert.Equal(0, e.SlaveExceptionCode);
            Assert.Null(e.InnerException);
        }

        [Fact]
        public void ConstructorWithMessageAndInnerException()
        {
            var inner = new IOException("Bar");
            var e = new ServerException("Foo", inner);
            Assert.Equal("Foo", e.Message);
            Assert.Same(inner, e.InnerException);
            Assert.Equal(0, e.SlaveAddress);
            Assert.Equal(0, e.FunctionCode);
            Assert.Equal(0, e.SlaveExceptionCode);
        }

        [Fact]
        public void ConstructorWithSlaveExceptionResponse()
        {
            var response = new ServerExceptionResponse(12, ModbusFunctionCodes.ReadCoils, 1);
            var e = new ServerException(response);

            Assert.Equal(12, e.SlaveAddress);
            Assert.Equal(ModbusFunctionCodes.ReadCoils, e.FunctionCode);
            Assert.Equal(1, e.SlaveExceptionCode);
            Assert.Null(e.InnerException);

            Assert.Equal(
                $@"Exception of type '{typeof(ServerException).FullName}' was thrown.{Environment.NewLine}Function Code: {response.FunctionCode}{Environment.NewLine}Exception Code: {response.SlaveExceptionCode} - {Resources.IllegalFunction}",
                e.Message);
        }

        [Fact]
        public void ConstructorWithCustomMessageAndSlaveExceptionResponse()
        {
            var response = new ServerExceptionResponse(12, ModbusFunctionCodes.ReadCoils, 2);
            string customMessage = "custom message";
            var e = new ServerException(customMessage, response);

            Assert.Equal(12, e.SlaveAddress);
            Assert.Equal(ModbusFunctionCodes.ReadCoils, e.FunctionCode);
            Assert.Equal(2, e.SlaveExceptionCode);
            Assert.Null(e.InnerException);

            Assert.Equal(
                $@"{customMessage}{Environment.NewLine}Function Code: {response.FunctionCode}{Environment.NewLine}Exception Code: {response.SlaveExceptionCode} - {Resources.IllegalDataAddress}",
                e.Message);
        }

#if NET46
        [Fact]
        public void Serializable()
        {
            var formatter = new BinaryFormatter();
            var e = new ServerException(new ServerExceptionResponse(1, 2, 3));

            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, e);
                stream.Position = 0;

                var e2 = (ServerException)formatter.Deserialize(stream);
                Assert.NotNull(e2);
                Assert.Equal(1, e2.SlaveAddress);
                Assert.Equal(2, e2.FunctionCode);
                Assert.Equal(3, e2.SlaveExceptionCode);
            }
        }
#endif
    }
}
