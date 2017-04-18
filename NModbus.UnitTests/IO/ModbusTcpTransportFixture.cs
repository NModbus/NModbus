using System;
using System.IO;
using System.Linq;
using Modbus.Data;
using Modbus.IO;
using Modbus.Message;
using Modbus.UnitTests.Message;
using Moq;
using Xunit;

namespace Modbus.UnitTests.IO
{
    public class ModbusTcpTransportFixture
    {
        private IStreamResource StreamResourceMock => new Mock<IStreamResource>(MockBehavior.Strict).Object;

        [Fact]
        public void BuildMessageFrame()
        {
            var mock = new Mock<ModbusIpTransport>(StreamResourceMock) { CallBase = true };
            var message = new ReadCoilsInputsRequest(Modbus.ReadCoils, 2, 10, 5);

            byte[] result = mock.Object.BuildMessageFrame(message);
            Assert.Equal(new byte[] { 0, 0, 0, 0, 0, 6, 2, 1, 0, 10, 0, 5 }, result);
            mock.VerifyAll();
        }

        [Fact]
        public void GetMbapHeader()
        {
            var message = new WriteMultipleRegistersRequest(3, 1, MessageUtility.CreateDefaultCollection<RegisterCollection, ushort>(0, 120));
            message.TransactionId = 45;
            Assert.Equal(new byte[] { 0, 45, 0, 0, 0, 247, 3 }, ModbusIpTransport.GetMbapHeader(message));
        }

        [Fact]
        public void Write()
        {
            var streamMock = new Mock<IStreamResource>(MockBehavior.Strict);
            var mock = new Mock<ModbusIpTransport>(streamMock.Object) { CallBase = true };
            var request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 3);

            streamMock.Setup(s => s.Write(It.IsNotNull<byte[]>(), 0, 12));

            mock.Setup(t => t.GetNewTransactionId()).Returns(ushort.MaxValue);

            mock.Object.Write(request);

            Assert.Equal(ushort.MaxValue, request.TransactionId);

            mock.VerifyAll();
            streamMock.VerifyAll();
        }

        [Fact]
        public void ReadRequestResponse()
        {
            var mock = new Mock<IStreamResource>(MockBehavior.Strict);
            var request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 3);
            int calls = 0;
            byte[][] source =
            {
                new byte[] { 45, 63, 0, 0, 0, 6 },
                new byte[] { 1 }.Concat(request.ProtocolDataUnit).ToArray()
            };

            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 6), 0, 6))
                .Returns((byte[] buf, int offset, int count) =>
                {
                    Array.Copy(source[calls++], buf, 6);
                    return 6;
                });

            Assert.Equal(
                new byte[] { 45, 63, 0, 0, 0, 6, 1, 1, 0, 1, 0, 3 },
                ModbusIpTransport.ReadRequestResponse(mock.Object));

            mock.VerifyAll();
        }

        [Fact]
        public void ReadRequestResponse_ConnectionAbortedWhileReadingMBAPHeader()
        {
            var mock = new Mock<IStreamResource>(MockBehavior.Strict);
            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 6), 0, 6)).Returns(3);
            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 6), 3, 3)).Returns(0);

            Assert.Throws<IOException>(() => ModbusIpTransport.ReadRequestResponse(mock.Object));
            mock.VerifyAll();
        }

        [Fact]
        public void ReadRequestResponse_ConnectionAbortedWhileReadingMessageFrame()
        {
            var mock = new Mock<IStreamResource>(MockBehavior.Strict);

            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 6), 0, 6)).Returns(6);
            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 6), 0, 6)).Returns(3);
            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 6), 3, 3)).Returns(0);

            Assert.Throws<IOException>(() => ModbusIpTransport.ReadRequestResponse(mock.Object));
            mock.VerifyAll();
        }

        [Fact]
        public void GetNewTransactionId()
        {
            var transport = new ModbusIpTransport(StreamResourceMock);

            Assert.Equal(1, transport.GetNewTransactionId());
            Assert.Equal(2, transport.GetNewTransactionId());
        }

        [Fact]
        public void OnShouldRetryResponse_ReturnsTrue_IfWithinThreshold()
        {
            var transport = new ModbusIpTransport(StreamResourceMock);
            var request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 1);
            var response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 1, null);

            request.TransactionId = 5;
            response.TransactionId = 4;
            transport.RetryOnOldResponseThreshold = 3;

            Assert.True(transport.OnShouldRetryResponse(request, response));
        }

        [Fact]
        public void OnShouldRetryResponse_ReturnsFalse_IfThresholdDisabled()
        {
            var transport = new ModbusIpTransport(StreamResourceMock);
            var request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 1);
            var response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 1, null);

            request.TransactionId = 5;
            response.TransactionId = 4;
            transport.RetryOnOldResponseThreshold = 0;

            Assert.False(transport.OnShouldRetryResponse(request, response));
        }

        [Fact]
        public void OnShouldRetryResponse_ReturnsFalse_IfEqualTransactionId()
        {
            var transport = new ModbusIpTransport(StreamResourceMock);
            var request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 1);
            var response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 1, null);

            request.TransactionId = 5;
            response.TransactionId = 5;
            transport.RetryOnOldResponseThreshold = 3;

            Assert.False(transport.OnShouldRetryResponse(request, response));
        }

        [Fact]
        public void OnShouldRetryResponse_ReturnsFalse_IfOutsideThreshold()
        {
            var transport = new ModbusIpTransport(StreamResourceMock);
            var request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 1);
            var response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 1, null);

            request.TransactionId = 5;
            response.TransactionId = 2;
            transport.RetryOnOldResponseThreshold = 3;

            Assert.False(transport.OnShouldRetryResponse(request, response));
        }

        [Fact]
        public void ValidateResponse_MismatchingTransactionIds()
        {
            var transport = new ModbusIpTransport(StreamResourceMock);

            var request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 1);
            request.TransactionId = 5;
            var response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 1, null);
            response.TransactionId = 6;

            Assert.Throws<IOException>(() => transport.ValidateResponse(request, response));
        }

        [Fact]
        public void ValidateResponse()
        {
            var transport = new ModbusIpTransport(StreamResourceMock);

            var request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 1);
            request.TransactionId = 5;
            var response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 1, null);
            response.TransactionId = 5;

            // no exception is thrown
            transport.ValidateResponse(request, response);
        }
    }
}
