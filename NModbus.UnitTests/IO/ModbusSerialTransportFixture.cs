using System;
using System.IO;
using Moq;
using NModbus.Data;
using NModbus.IO;
using NModbus.Logging;
using NModbus.Message;
using NModbus.UnitTests.Message;
using NModbus.Utility;
using Xunit;

namespace NModbus.UnitTests.IO
{
    public class ModbusSerialTransportFixture
    {
        private static IStreamResource StreamResource => new Mock<IStreamResource>(MockBehavior.Strict).Object;

        [Fact]
        public void CreateResponse()
        {
            var transport = new ModbusAsciiTransport(StreamResource, new ModbusFactory(), NullModbusLogger.Instance);
            var expectedResponse = new ReadCoilsInputsResponse(ModbusFunctionCodes.ReadCoils, 2, 1, new DiscreteCollection(true, false, false, false, false, false, false, true));
            byte lrc = ModbusUtility.CalculateLrc(expectedResponse.MessageFrame);
            var response = transport.CreateResponse<ReadCoilsInputsResponse>(new byte[] { 2, ModbusFunctionCodes.ReadCoils, 1, 129, lrc });

            Assert.IsType<ReadCoilsInputsResponse>(response);
            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
        }

        [Fact]
        public void CreateResponseErroneousLrc()
        {
            var transport = new ModbusAsciiTransport(StreamResource, new ModbusFactory(), NullModbusLogger.Instance) { CheckFrame = true };
            var frame = new byte[] { 19, ModbusFunctionCodes.ReadCoils, 0, 0, 0, 2, 115 };

            Assert.Throws<IOException>(
                () => transport.CreateResponse<ReadCoilsInputsResponse>(frame));
        }

        [Fact]
        public void CreateResponseErroneousLrcDoNotCheckFrame()
        {
            var transport = new ModbusAsciiTransport(StreamResource, new ModbusFactory(), NullModbusLogger.Instance) { CheckFrame = false };

            transport.CreateResponse<ReadCoilsInputsResponse>(new byte[] { 19, ModbusFunctionCodes.ReadCoils, 0, 0, 0, 2, 115 });
        }

        /// <summary>
        /// When using the serial RTU protocol the beginning of the message could get mangled leading to an unsupported message type.
        /// We want to be sure to try the message again so clear the RX buffer and try again.
        /// </summary>
        [Fact]
        public void UnicastMessage_PurgeReceiveBuffer()
        {
            var mock = new Mock<IStreamResource>(MockBehavior.Strict);
            IStreamResource serialResource = mock.Object;
            var factory = new ModbusFactory();
            var transport = new ModbusRtuTransport(serialResource, factory, NullModbusLogger.Instance);

            mock.Setup(s => s.DiscardInBuffer());
            mock.Setup(s => s.Write(It.IsAny<byte[]>(), 0, 0));

            serialResource.DiscardInBuffer();
            serialResource.Write(null, 0, 0);

            // mangled response
            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 4), 0, 4)).Returns(4);

            serialResource.DiscardInBuffer();
            serialResource.Write(null, 0, 0);

            // normal response
            var response = new ReadCoilsInputsResponse(ModbusFunctionCodes.ReadCoils, 2, 1, new DiscreteCollection(true, false, true, false, false, false, false, false));

            // write request
            mock.Setup(s => s.Write(It.Is<byte[]>(x => x.Length == 8), 0, 8));

            // read header
            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 4), 0, 4))
                .Returns((byte[] buf, int offset, int count) =>
                {
                    Array.Copy(response.MessageFrame, 0, buf, 0, 4);
                    return 4;
                });

            // read remainder
            mock.Setup(s => s.Read(It.Is<byte[]>(x => x.Length == 2), 0, 2))
                .Returns((byte[] buf, int offset, int count) =>
                {
                    Array.Copy(ModbusUtility.CalculateCrc(response.MessageFrame), 0, buf, 0, 2);
                    return 2;
                });

            var request = new ReadCoilsInputsRequest(ModbusFunctionCodes.ReadCoils, 2, 3, 4);
            var actualResponse = transport.UnicastMessage<ReadCoilsInputsResponse>(request);

            ModbusMessageFixture.AssertModbusMessagePropertiesAreEqual(response, actualResponse);
            mock.VerifyAll();
        }

        /// <summary>
        ///     Serial line spec allows slave addresses 0-247 on the wire (0 = broadcast).
        ///     Reserved addresses 248-255 must be rejected before any frame is written.
        /// </summary>
        [Theory]
        [InlineData(248)]  // reserved by the Modbus spec
        [InlineData(255)]  // out of range
        public void Write_RejectsReservedSlaveAddress(byte slaveAddress)
        {
            var transport = new ModbusAsciiTransport(StreamResource, new ModbusFactory(), NullModbusLogger.Instance);
            var message = new ReadHoldingInputRegistersRequest(3, slaveAddress, 1, 1);

            Assert.Throws<ArgumentOutOfRangeException>(() => transport.Write(message));
        }

        [Theory]
        [InlineData(0)]    // broadcast
        [InlineData(1)]    // lower bound
        [InlineData(247)]  // upper bound
        public void Write_AllowsValidSlaveAddress(byte slaveAddress)
        {
            var streamResource = new Mock<IStreamResource>();
            var transport = new ModbusAsciiTransport(streamResource.Object, new ModbusFactory(), NullModbusLogger.Instance);
            var message = new ReadHoldingInputRegistersRequest(3, slaveAddress, 1, 1);

            transport.Write(message);
        }
    }
}
