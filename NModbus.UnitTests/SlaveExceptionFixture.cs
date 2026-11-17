using System;
using System.Globalization;
using System.IO;
using System.Threading;
using NModbus.Message;
using Xunit;
using NModbus.Device;
using NModbus.Data;
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
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            var e = new SlaveException();
            Assert.Equal($"Exception of type '{typeof(SlaveException).FullName}' was thrown.", e.Message);
            Assert.Equal(0, e.SlaveAddress);
            Assert.Equal(0, e.FunctionCode);
            Assert.Equal(0, e.SlaveExceptionCode);
            Assert.Null(e.InnerException);
        }

        [Fact]
        public void ConstructorWithMessage()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            var e = new SlaveException("Hello World");
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
            var e = new SlaveException("Foo", inner);
            Assert.Equal("Foo", e.Message);
            Assert.Same(inner, e.InnerException);
            Assert.Equal(0, e.SlaveAddress);
            Assert.Equal(0, e.FunctionCode);
            Assert.Equal(0, e.SlaveExceptionCode);
        }

        [Fact]
        public void ConstructorWithSlaveExceptionResponse()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            var response = new SlaveExceptionResponse(12, ModbusFunctionCodes.ReadCoils, 1);
            var e = new SlaveException(response);

            Assert.Equal(12, e.SlaveAddress);
            Assert.Equal(ModbusFunctionCodes.ReadCoils, e.FunctionCode);
            Assert.Equal(1, e.SlaveExceptionCode);
            Assert.Null(e.InnerException);

            Assert.Equal(
                $@"Exception of type '{typeof(SlaveException).FullName}' was thrown.{Environment.NewLine}Function Code: {response.FunctionCode}{Environment.NewLine}Exception Code: {response.SlaveExceptionCode} - {Resources.IllegalFunction}",
                e.Message);
        }

        [Fact]
        public void ConstructorWithCustomMessageAndSlaveExceptionResponse()
        {
            var response = new SlaveExceptionResponse(12, ModbusFunctionCodes.ReadCoils, 2);
            string customMessage = "custom message";
            var e = new SlaveException(customMessage, response);

            Assert.Equal(12, e.SlaveAddress);
            Assert.Equal(ModbusFunctionCodes.ReadCoils, e.FunctionCode);
            Assert.Equal(2, e.SlaveExceptionCode);
            Assert.Null(e.InnerException);

            Assert.Equal(
                $@"{customMessage}{Environment.NewLine}Function Code: {response.FunctionCode}{Environment.NewLine}Exception Code: {response.SlaveExceptionCode} - {Resources.IllegalDataAddress}",
                e.Message);
        }

        [Fact]
        public void ApplyRequest_ReturnsCustomExceptionCode_WhenDataStoreThrowsInvalidModbusRequestException()
        {
            var dataStore = new ThrowingDataStore();
            var slave = new ModbusSlave(1, dataStore, new ModbusFactory().GetAllFunctionServices());
            var request = new ReadHoldingInputRegistersRequest(ModbusFunctionCodes.ReadHoldingRegisters, 1, 0, 1);

            var response = slave.ApplyRequest(request) as SlaveExceptionResponse;

            Assert.NotNull(response);
            Assert.Equal(SlaveExceptionCodes.IllegalDataAddress, response.SlaveExceptionCode);
        }

        [Fact]
        public void ApplyRequest_ReturnsSlaveDeviceFailure_WhenDataStoreThrowsUnexpectedException()
        {
            var dataStore = new ThrowingDataStore();
            var slave = new ModbusSlave(1, dataStore, new ModbusFactory().GetAllFunctionServices());
            var request = new WriteMultipleRegistersRequest(1, 0, new RegisterCollection(new ushort[] { 1 }));

            var response = slave.ApplyRequest(request) as SlaveExceptionResponse;

            Assert.NotNull(response);
            Assert.Equal(SlaveExceptionCodes.SlaveDeviceFailure, response.SlaveExceptionCode);
        }

#if NET46
        [Fact]
        public void Serializable()
        {
            var formatter = new BinaryFormatter();
            var e = new SlaveException(new SlaveExceptionResponse(1, 2, 3));

            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, e);
                stream.Position = 0;

                var e2 = (SlaveException)formatter.Deserialize(stream);
                Assert.NotNull(e2);
                Assert.Equal(1, e2.SlaveAddress);
                Assert.Equal(2, e2.FunctionCode);
                Assert.Equal(3, e2.SlaveExceptionCode);
            }
        }
#endif

        private class ThrowingDataStore : ISlaveDataStore
        {
            public IPointSource<bool> CoilDiscretes { get; } = new PointSource<bool>();

            public IPointSource<bool> CoilInputs { get; } = new PointSource<bool>();

            public IPointSource<ushort> HoldingRegisters { get; } = new ThrowingPointSource<ushort>();

            public IPointSource<ushort> InputRegisters { get; } = new PointSource<ushort>();
        }

        private class ThrowingPointSource<TPoint> : IPointSource<TPoint>
        {
            public TPoint[] ReadPoints(ushort startAddress, ushort numberOfPoints)
            {
                throw new InvalidModbusRequestException(SlaveExceptionCodes.IllegalDataAddress);
            }

            public void WritePoints(ushort startAddress, TPoint[] points)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
