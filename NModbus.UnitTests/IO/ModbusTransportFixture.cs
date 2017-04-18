using System;
using System.IO;
using System.Linq;
using Modbus.Data;
using Modbus.IO;
using Modbus.Message;
using Modbus.Utility;
using Moq;
using Xunit;

namespace Modbus.UnitTests.IO
{
    public class ModbusTransportFixture
    {
        [Fact]
        public void Dispose_MultipleTimes_ShouldNotThrow()
        {
            var streamMock = new Mock<IStreamResource>(MockBehavior.Strict);
            streamMock.Setup(s => s.Dispose());

            var mock = new Mock<ModbusTransport>(streamMock.Object) { CallBase = true };

            using (var transport = mock.Object)
            {
                Assert.NotNull(transport.StreamResource);
                transport.Dispose();
                Assert.Null(transport.StreamResource);
            }
        }

        [Fact]
        public void ReadWriteTimeouts()
        {
            const int expectedReadTimeout = 42;
            const int expectedWriteTimeout = 33;
            var mock = new Mock<IStreamResource>(MockBehavior.Strict);

            mock.SetupProperty(s => s.ReadTimeout, expectedReadTimeout);
            mock.SetupProperty(s => s.WriteTimeout, expectedWriteTimeout);

            var transport = new Mock<ModbusTransport>(MockBehavior.Strict, mock.Object) { CallBase = true }.Object;

            Assert.Equal(expectedReadTimeout, transport.ReadTimeout);
            Assert.Equal(expectedWriteTimeout, transport.WriteTimeout);

            // Swapping
            transport.ReadTimeout = expectedWriteTimeout;
            transport.WriteTimeout = expectedReadTimeout;

            Assert.Equal(expectedWriteTimeout, transport.ReadTimeout);
            Assert.Equal(expectedReadTimeout, transport.WriteTimeout);

            mock.VerifyAll();
        }

        [Fact]
        public void WaitToRetryMilliseconds()
        {
            var mock = new Mock<ModbusTransport>(MockBehavior.Strict) { CallBase = true };
            var transport = mock.Object;

            Assert.Equal(Modbus.DefaultWaitToRetryMilliseconds, transport.WaitToRetryMilliseconds);

            Assert.Throws<ArgumentException>(() => transport.WaitToRetryMilliseconds = -1);

            const int expectedWaitToRetryMilliseconds = 42;
            transport.WaitToRetryMilliseconds = expectedWaitToRetryMilliseconds;
            Assert.Equal(expectedWaitToRetryMilliseconds, transport.WaitToRetryMilliseconds);
        }

        [Fact]
        public void UnicastMessage()
        {
            var data = new DiscreteCollection(true, false, true, false, false, false, false, false);
            var mock = new Mock<ModbusTransport>() { CallBase = true };
            var transport = mock.Object;

            mock.Setup(t => t.Write(It.IsNotNull<IModbusMessage>()));
            mock.Setup(t => t.ReadResponse<ReadCoilsInputsResponse>())
                .Returns(new ReadCoilsInputsResponse(Modbus.ReadCoils, 2, 1, data));
            mock.Setup(t => t.OnValidateResponse(It.IsNotNull<IModbusMessage>(), It.IsNotNull<IModbusMessage>()));

            var request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 2, 3, 4);
            var expectedResponse = new ReadCoilsInputsResponse(Modbus.ReadCoils, 2, 1, data);
            var response = transport.UnicastMessage<ReadCoilsInputsResponse>(request);

            Assert.Equal(expectedResponse.MessageFrame, response.MessageFrame);
            mock.VerifyAll();
        }

        [Fact]
        public void UnicastMessage_WrongResponseFunctionCode()
        {
            var request = new ReadCoilsInputsRequest(Modbus.ReadInputs, 2, 3, 4);
            var mock = new Mock<ModbusTransport>() { CallBase = true };
            var transport = mock.Object;
            int writeCallsCount = 0;
            int readResponseCallsCount = 0;

            mock.Setup(t => t.Write(It.IsNotNull<IModbusMessage>())).Callback(() => ++writeCallsCount);

            mock.Setup(t => t.ReadResponse<ReadCoilsInputsResponse>())
                .Returns(new ReadCoilsInputsResponse(Modbus.ReadCoils, 2, 0, new DiscreteCollection()))
                .Callback(() => ++readResponseCallsCount);

            Assert.Throws<IOException>(() => transport.UnicastMessage<ReadCoilsInputsResponse>(request));
            Assert.Equal(Modbus.DefaultRetries + 1, writeCallsCount);
            Assert.Equal(Modbus.DefaultRetries + 1, readResponseCallsCount);

            mock.VerifyAll();
        }

        [Fact]
        public void UnicastMessage_ErrorSlaveException()
        {
            var mock = new Mock<ModbusTransport>() { CallBase = true };
            var request = new ReadCoilsInputsRequest(Modbus.ReadInputs, 2, 3, 4);
            var transport = mock.Object;

            mock.Setup(t => t.Write(It.IsNotNull<IModbusMessage>()));
            mock.Setup(t => t.ReadResponse<ReadCoilsInputsResponse>()).Throws<SlaveException>();

            Assert.Throws<SlaveException>(() => transport.UnicastMessage<ReadCoilsInputsResponse>(request));

            mock.VerifyAll();
        }

        /// <summary>
        /// We should reread the response w/o retransmitting the request.
        /// </summary>
        [Fact]
        public void UnicastMessage_AcknowlegeSlaveException()
        {
            var mock = new Mock<ModbusTransport>() { CallBase = true };
            var transport = mock.Object;
            int callsCount = 0;

            // set the wait to retry property to a small value so the test completes quickly
            transport.WaitToRetryMilliseconds = 5;

            mock.Setup(t => t.Write(It.IsNotNull<IModbusMessage>()));

            // return a slave exception a greater number of times than number of retries to make sure we aren't just retrying
            mock.Setup(t => t.ReadResponse<ReadHoldingInputRegistersResponse>())
                .Returns(() =>
                {
                    if (callsCount < transport.Retries + 1)
                    {
                        ++callsCount;
                        return new SlaveExceptionResponse(1, Modbus.ReadHoldingRegisters + Modbus.ExceptionOffset, Modbus.Acknowledge);
                    }

                    return new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection(1));
                });

            mock.Setup(t => t.OnValidateResponse(It.IsNotNull<IModbusMessage>(), It.IsNotNull<IModbusMessage>()));

            var request = new ReadHoldingInputRegistersRequest(Modbus.ReadHoldingRegisters, 1, 1, 1);
            var expectedResponse = new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection(1));
            var response = transport.UnicastMessage<ReadHoldingInputRegistersResponse>(request);

            Assert.Equal(transport.Retries + 1, callsCount);
            Assert.Equal(expectedResponse.MessageFrame, response.MessageFrame);
            mock.VerifyAll();
        }

        /// <summary>
        ///     We should retransmit the request.
        /// </summary>
        [Fact]
        public void UnicastMessage_SlaveDeviceBusySlaveException()
        {
            var mock = new Mock<ModbusTransport>() { CallBase = true };
            var transport = mock.Object;
            int writeCallsCount = 0;
            int readResponseCallsCount = 0;

            // set the wait to retry property to a small value so the test completes quickly
            transport.WaitToRetryMilliseconds = 5;

            mock.Setup(t => t.Write(It.IsNotNull<IModbusMessage>()))
                .Callback(() => ++writeCallsCount);

            // return a slave exception a greater number of times than number of retries to make sure we aren't just retrying
            mock.Setup(t => t.ReadResponse<ReadHoldingInputRegistersResponse>())
                .Returns(() =>
                {
                    if (readResponseCallsCount == 0)
                    {
                        return new SlaveExceptionResponse(1, Modbus.ReadHoldingRegisters + Modbus.ExceptionOffset, Modbus.SlaveDeviceBusy);
                    }
                    else
                    {
                        return new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection(1));
                    }
                })
                .Callback(() => ++readResponseCallsCount);

            mock.Setup(t => t.OnValidateResponse(It.IsNotNull<IModbusMessage>(), It.IsNotNull<IModbusMessage>()));

            var request = new ReadHoldingInputRegistersRequest(Modbus.ReadHoldingRegisters, 1, 1, 1);
            var expectedResponse = new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection(1));
            var response = transport.UnicastMessage<ReadHoldingInputRegistersResponse>(request);

            Assert.Equal(2, writeCallsCount);
            Assert.Equal(2, readResponseCallsCount);
            Assert.Equal(expectedResponse.MessageFrame, response.MessageFrame);

            mock.VerifyAll();
        }

        /// <summary>
        ///     We should retransmit the request.
        /// </summary>
        [Fact]
        public void UnicastMessage_SlaveDeviceBusySlaveExceptionDoesNotFailAfterExceedingRetries()
        {
            var mock = new Mock<ModbusTransport>() { CallBase = true };
            var transport = mock.Object;
            int writeCallsCount = 0;
            int readResponseCallsCount = 0;

            // set the wait to retry property to a small value so the test completes quickly
            transport.WaitToRetryMilliseconds = 5;

            mock.Setup(t => t.Write(It.IsNotNull<IModbusMessage>()))
                .Callback(() => ++writeCallsCount);

            // return a slave exception a greater number of times than number of retries to make sure we aren't just retrying
            mock.Setup(t => t.ReadResponse<ReadHoldingInputRegistersResponse>())
                .Returns(() =>
                {
                    if (readResponseCallsCount < transport.Retries)
                    {
                        return new SlaveExceptionResponse(1, Modbus.ReadHoldingRegisters + Modbus.ExceptionOffset, Modbus.SlaveDeviceBusy);
                    }

                    return new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection(1));
                })
                .Callback(() => ++readResponseCallsCount);

            mock.Setup(t => t.OnValidateResponse(It.IsNotNull<IModbusMessage>(), It.IsNotNull<IModbusMessage>()));

            var request = new ReadHoldingInputRegistersRequest(Modbus.ReadHoldingRegisters, 1, 1, 1);
            var expectedResponse = new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection(1));
            var response = transport.UnicastMessage<ReadHoldingInputRegistersResponse>(request);

            Assert.Equal(transport.Retries + 1, writeCallsCount);
            Assert.Equal(transport.Retries + 1, readResponseCallsCount);
            Assert.Equal(expectedResponse.MessageFrame, response.MessageFrame);

            mock.VerifyAll();
        }

        // TODO: uncomment test below and rewrite with Moq

        ////[Theory]
        ////[InlineData(typeof(TimeoutException))]
        ////[InlineData(typeof(IOException))]
        ////[InlineData(typeof(NotImplementedException))]
        ////[InlineData(typeof(FormatException))]
        ////public void UnicastMessage_SingleFailingException(Type exceptionType)
        ////{
        ////    MockRepository mocks = new MockRepository();
        ////    ModbusTransport transport = mocks.PartialMock<ModbusTransport>();
        ////    transport.Retries = 1;
        ////    transport.Write(null);
        ////    LastCall.IgnoreArguments().Repeat.Times(2);
        ////    Expect.Call(transport.ReadResponse<ReadCoilsInputsResponse>())
        ////        .Do((ThrowExceptionDelegate)delegate { throw (Exception)Activator.CreateInstance(exceptionType); });

        ////    Expect.Call(transport.ReadResponse<ReadCoilsInputsResponse>())
        ////        .Return(new ReadCoilsInputsResponse(Modbus.ReadCoils, 2, 1,
        ////            new DiscreteCollection(true, false, true, false, false, false, false, false)));

        ////    transport.OnValidateResponse(null, null);
        ////    LastCall.IgnoreArguments();

        ////    mocks.ReplayAll();

        ////    ReadCoilsInputsRequest request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 2, 3, 4);
        ////    transport.UnicastMessage<ReadCoilsInputsResponse>(request);

        ////    mocks.VerifyAll();
        ////}

        [Theory]
        [InlineData(typeof(TimeoutException))]
        [InlineData(typeof(IOException))]
        [InlineData(typeof(NotImplementedException))]
        [InlineData(typeof(FormatException))]
        public void UnicastMessage_TooManyFailingExceptions(Type exceptionType)
        {
            var mock = new Mock<ModbusTransport>() { CallBase = true };
            var transport = mock.Object;
            int writeCallsCount = 0;
            int readResponseCallsCount = 0;

            mock.Setup(t => t.Write(It.IsNotNull<IModbusMessage>()))
                .Callback(() => ++writeCallsCount);

            mock.Setup(t => t.ReadResponse<ReadCoilsInputsResponse>())
                .Callback(() => ++readResponseCallsCount)
                .Throws((Exception)Activator.CreateInstance(exceptionType));

            var request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 2, 3, 4);

            Assert.Throws(exceptionType, () => transport.UnicastMessage<ReadCoilsInputsResponse>(request));
            Assert.Equal(transport.Retries + 1, writeCallsCount);
            Assert.Equal(transport.Retries + 1, readResponseCallsCount);
            mock.VerifyAll();
        }

        [Fact]
        public void UnicastMessage_TimeoutException()
        {
            var mock = new Mock<ModbusTransport>() { CallBase = true };
            var transport = mock.Object;
            int writeCallsCount = 0;
            int readResponseCallsCount = 0;

            mock.Setup(t => t.Write(It.IsNotNull<IModbusMessage>()))
                .Callback(() => ++writeCallsCount);

            mock.Setup(t => t.ReadResponse<ReadCoilsInputsResponse>())
                .Callback(() => ++readResponseCallsCount)
                .Throws<TimeoutException>();

            var request = new ReadCoilsInputsRequest(Modbus.ReadInputs, 2, 3, 4);
            Assert.Throws<TimeoutException>(() => transport.UnicastMessage<ReadCoilsInputsResponse>(request));
            Assert.Equal(Modbus.DefaultRetries + 1, writeCallsCount);
            Assert.Equal(Modbus.DefaultRetries + 1, readResponseCallsCount);
            mock.VerifyAll();
        }

        [Fact]
        public void UnicastMessage_Retries()
        {
            var mock = new Mock<ModbusTransport>() { CallBase = true };
            var transport = mock.Object;
            int writeCallsCount = 0;
            int readResponseCallsCount = 0;

            mock.Setup(t => t.Write(It.IsNotNull<IModbusMessage>()))
                .Callback(() => ++writeCallsCount);

            transport.Retries = 5;
            mock.Setup(t => t.ReadResponse<ReadCoilsInputsResponse>())
                .Callback(() => ++readResponseCallsCount)
                .Throws<TimeoutException>();

            var request = new ReadCoilsInputsRequest(Modbus.ReadInputs, 2, 3, 4);

            Assert.Throws<TimeoutException>(() => transport.UnicastMessage<ReadCoilsInputsResponse>(request));
            Assert.Equal(transport.Retries + 1, writeCallsCount);
            Assert.Equal(transport.Retries + 1, readResponseCallsCount);
            mock.VerifyAll();
        }

        [Fact]
        public void UnicastMessage_ReReads_IfShouldRetryReturnTrue()
        {
            var mock = new Mock<ModbusTransport>() { CallBase = true };
            var transport = mock.Object;
            var expectedResponse = new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection(1));
            int readResponseCallsCount = 0;
            int onShouldRetryResponseCallsCount = 0;
            bool[] expectedReturn = { true, false };

            transport.RetryOnOldResponseThreshold = 3;

            mock.Setup(t => t.Write(It.IsNotNull<IModbusMessage>()));
            ////transport.Stub(x => x.Write(null)).IgnoreArguments();

            mock.Setup(t => t.OnValidateResponse(It.IsNotNull<IModbusMessage>(), It.IsNotNull<IModbusMessage>()));

            mock.Setup(t => t.OnShouldRetryResponse(It.IsNotNull<IModbusMessage>(), It.IsNotNull<IModbusMessage>()))
                .Returns(() => expectedReturn[onShouldRetryResponseCallsCount++]);

            mock.Setup(t => t.ReadResponse<ReadHoldingInputRegistersResponse>())
                .Returns(expectedResponse)
                .Callback(() => ++readResponseCallsCount);

            var request = new ReadHoldingInputRegistersRequest(Modbus.ReadHoldingRegisters, 1, 1, 1);
            var response = transport.UnicastMessage<ReadHoldingInputRegistersResponse>(request);

            Assert.Equal(2, readResponseCallsCount);
            Assert.Equal(2, onShouldRetryResponseCallsCount);
            Assert.Equal(expectedResponse.MessageFrame, response.MessageFrame);
            mock.VerifyAll();
        }

        [Fact]
        public void CreateResponse_SlaveException()
        {
            var mock = new Mock<ModbusTransport>() { CallBase = true };
            var transport = mock.Object;

            byte[] frame = { 2, 129, 2 };
            byte lrc = ModbusUtility.CalculateLrc(frame);
            var message = transport.CreateResponse<ReadCoilsInputsResponse>(Enumerable.Concat(frame, new byte[] { lrc }).ToArray());
            Assert.IsType<SlaveExceptionResponse>(message);
        }

        [Fact]
        public void ShouldRetryResponse_ReturnsFalse_IfDifferentMessage()
        {
            var mock = new Mock<ModbusTransport>(MockBehavior.Strict) { CallBase = true };
            var transport = mock.Object;

            IModbusMessage request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 2, 1, 1);
            IModbusMessage response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 1, null);

            Assert.False(transport.ShouldRetryResponse(request, response));
        }

        [Fact]
        public void ValidateResponse_MismatchingFunctionCodes()
        {
            var mock = new Mock<ModbusTransport>(MockBehavior.Strict) { CallBase = true };
            var transport = mock.Object;

            IModbusMessage request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 1);
            IModbusMessage response = new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection());

            Assert.Throws<IOException>(() => transport.ValidateResponse(request, response));
        }

        [Fact]
        public void ValidateResponse_MismatchingSlaveAddress()
        {
            var mock = new Mock<ModbusTransport>(MockBehavior.Strict) { CallBase = true };
            var transport = mock.Object;

            IModbusMessage request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 42, 1, 1);
            IModbusMessage response = new ReadHoldingInputRegistersResponse(Modbus.ReadCoils, 33, new RegisterCollection());

            Assert.Throws<IOException>(() => transport.ValidateResponse(request, response));
        }

        [Fact]
        public void ValidateResponse_CallsOnValidateResponse()
        {
            var mock = new Mock<ModbusTransport>(MockBehavior.Strict) { CallBase = true };
            var transport = mock.Object;

            mock.Setup(t => t.OnValidateResponse(It.IsNotNull<IModbusMessage>(), It.IsNotNull<IModbusMessage>()));

            IModbusMessage request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 1);
            IModbusMessage response = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 1, new DiscreteCollection());

            transport.ValidateResponse(request, response);
            mock.VerifyAll();
        }
    }
}
