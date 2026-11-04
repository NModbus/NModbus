using System.Threading;
using Moq;
using NModbus.Data;
using NModbus.Device;
using NModbus.Logging;
using Xunit;

namespace NModbus.UnitTests.Device
{
    public class ModbusSerialSlaveNetworkFixture
    {
        private static readonly byte[] BroadcastWriteSingleRegisterFrame = { 0, ModbusFunctionCodes.WriteSingleRegister, 0, 1, 0, 5 };

        [Fact]
        public void ListenAsync_BroadcastRequest_DoesNotReadResponseFromBus()
        {
            var factory = new ModbusFactory();
            var dataStore = new DefaultSlaveDataStore();

            using (var cts = new CancellationTokenSource())
            {
                var transport = new Mock<IModbusSerialTransport>(MockBehavior.Loose);
                transport.Setup(t => t.ReadRequest())
                    .Callback(() => cts.Cancel())
                    .Returns(BroadcastWriteSingleRegisterFrame);

                var network = new ModbusSerialSlaveNetwork(transport.Object, factory, NullModbusLogger.Instance);
                network.AddSlave(factory.CreateSlave(1, dataStore));

                network.ListenAsync(cts.Token).Wait();

                // Nobody answers a broadcast, so the slave must not consume the next frame on the bus.
                transport.Verify(t => t.IgnoreResponse(), Times.Never());
                transport.Verify(t => t.Write(It.IsAny<IModbusMessage>()), Times.Never());
                Assert.Equal(5, dataStore.HoldingRegisters.ReadPoints(1, 1)[0]);
            }
        }

        [Fact]
        public void ListenAsync_RequestForOtherSlave_IgnoresResponse()
        {
            var factory = new ModbusFactory();

            using (var cts = new CancellationTokenSource())
            {
                var transport = new Mock<IModbusSerialTransport>(MockBehavior.Loose);
                transport.Setup(t => t.ReadRequest())
                    .Callback(() => cts.Cancel())
                    .Returns(new byte[] { 2, ModbusFunctionCodes.WriteSingleRegister, 0, 1, 0, 5 });

                var network = new ModbusSerialSlaveNetwork(transport.Object, factory, NullModbusLogger.Instance);
                network.AddSlave(factory.CreateSlave(1));

                network.ListenAsync(cts.Token).Wait();

                transport.Verify(t => t.IgnoreResponse(), Times.Once());
            }
        }
    }
}
