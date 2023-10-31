using System;
using System.Threading;
using System.Threading.Tasks;
using NModbus;
using NModbus.Data;
using NModbus.Extensions.Enron;
using Xunit;

namespace Modbus.IntegrationTests
{
    public abstract class IntegrationTestBase
    {
        protected virtual IModbusFactory Factory { get; } = new ModbusFactory();

        [Theory]
        [InlineData(0, new ushort[] { 1 })]
        [InlineData(0, new ushort[] { 42, 55, 1000 })]
        [InlineData(22, new ushort[] { 68, 677, 8788, 60000 })]
        [InlineData(65533, new ushort[] { 1, 2, 3 })]
        [InlineData(65535, new ushort[] { 12312 })]
        public async Task ReadRegisters(ushort startingAddress, ushort[] values)
        {
            await TestAsync(c =>
            {
                var dataStore = new DefaultSlaveDataStore();

                dataStore.HoldingRegisters.WritePoints(startingAddress, values);

                c.SlaveNetwork.AddSlave(Factory.CreateSlave(1, dataStore));
                
                var registers = c.Master.ReadHoldingRegisters(1, startingAddress, (ushort)values.Length);

                Assert.Equal(values, registers);

                return Task.CompletedTask;
            });
        }

        [Theory]
        [InlineData(0, new uint[] { 256, 80000, 90 })]
        public async Task ReadRegisters32(ushort startingAddress, uint[] values)
        {
            await TestAsync(c =>
            {
                var dataStore = new DefaultSlaveDataStore();

                dataStore.HoldingRegisters.WritePoints(startingAddress, EnronModbus.ConvertFrom32(values));

                c.SlaveNetwork.AddSlave(Factory.CreateSlave(1, dataStore));

                var registers = c.Master.ReadHoldingRegisters32(1, startingAddress, (ushort)values.Length);

                Assert.Equal(values, registers);

                return Task.CompletedTask;
            });
        }

        [Theory]
        [InlineData(1000, new ushort[] { 4, 5, 6 }, 2000, new ushort[] { 100, 200, 300, 400 })]
        [InlineData(500, new ushort[] { 78, 25, 1, 906, 10000 }, 10000, new ushort[] { 4000, 3000, 200 })]
        [InlineData(10, new ushort[] { 20 }, 30, new ushort[] { 40 })]
        [InlineData(10, new ushort[] { 20 }, 30, new ushort[] { 40, 41 })]
        public async Task ReadWriteRegisters(ushort startReadAddress, ushort[] registersToRead, ushort startWriteAddress, ushort[] registersToWrite)
        {
            await TestAsync(async c =>
            {
                var dataStore = new DefaultSlaveDataStore();

                dataStore.HoldingRegisters.WritePoints(startReadAddress, registersToRead);

                c.SlaveNetwork.AddSlave(Factory.CreateSlave(1, dataStore));

                var registersThatWereRead = await c.Master.ReadWriteMultipleRegistersAsync(1, startReadAddress, (ushort)registersToRead.Length, startWriteAddress, registersToWrite);

                Assert.Equal(registersToRead, registersThatWereRead);
                Assert.Equal(registersToWrite, dataStore.HoldingRegisters.ReadPoints(startWriteAddress, (ushort)registersToWrite.Length));
            });
        }

        //TODO: Add way more tests

        protected async Task TestAsync(Func<IntegrationTestContext, Task> test)
        {
            using (var cancellationTokenSource = new CancellationTokenSource())
            using (var slaveNetwork = await CreateSlaveNetworkAsync())
            using (var listenTask = Task.Factory.StartNew(async () => await slaveNetwork.ListenAsync(cancellationTokenSource.Token), TaskCreationOptions.LongRunning))
            using (var master = await CreateMasterAsync())
            {
                //Create some context
                var context = new IntegrationTestContext(master, slaveNetwork);

                //Performt the test
                await test(context);

                //Cancel the listenTask
                cancellationTokenSource.Cancel();

                //Wait for the listenTask to complete
                await listenTask;
            }
        }

        protected abstract Task<IModbusSlaveNetwork> CreateSlaveNetworkAsync();

        protected abstract Task<IModbusMaster> CreateMasterAsync();

        protected class IntegrationTestContext
        {
            public IntegrationTestContext(IModbusMaster master, IModbusSlaveNetwork slaveNetwork)
            {
                Master = master ?? throw new ArgumentNullException(nameof(master));
                SlaveNetwork = slaveNetwork ?? throw new ArgumentNullException(nameof(slaveNetwork));
            }

            public IModbusMaster Master { get; }

            public IModbusSlaveNetwork SlaveNetwork { get; }
        }
    }
}
