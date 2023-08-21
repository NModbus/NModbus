using System;
using System.Linq;
using Moq;
using NModbus.Device;
using NModbus.IO;
using Xunit;

namespace NModbus.UnitTests.Device
{
    public class ModbusMasterFixture
    {
        private static IStreamResource StreamRsource => new Mock<IStreamResource>(MockBehavior.Strict).Object;

        private static IModbusSerialTransport Transport => new Mock<IModbusSerialTransport>().Object;

        private ModbusSerialMaster Master => new ModbusSerialMaster(Transport);

        [Fact]
        public void ReadCoils()
        {
            Assert.Throws<ArgumentException>(() => Master.ReadCoils(1, 1, 0));
            Assert.Throws<ArgumentException>(() => Master.ReadCoils(1, 1, 2001));
        }

        [Fact]
        public void ReadInputs()
        {
            Assert.Throws<ArgumentException>(() => Master.ReadInputs(1, 1, 0));
            Assert.Throws<ArgumentException>(() => Master.ReadInputs(1, 1, 2001));
        }

        [Fact]
        public void ReadHoldingRegisters()
        {
            Assert.Throws<ArgumentException>(() => Master.ReadHoldingRegisters(1, 1, 0));
            Assert.Throws<ArgumentException>(() => Master.ReadHoldingRegisters(1, 1, 126));
        }

        [Fact]
        public void ReadInputRegisters()
        {
            Assert.Throws<ArgumentException>(() => Master.ReadInputRegisters(1, 1, 0));
            Assert.Throws<ArgumentException>(() => Master.ReadInputRegisters(1, 1, 126));
        }

        [Fact]
        public void WriteMultipleRegisters()
        {
            Assert.Throws<ArgumentNullException>(() => Master.WriteMultipleRegisters(1, 1, null));
            Assert.Throws<ArgumentException>(() => Master.WriteMultipleRegisters(1, 1, new ushort[0]));
            Assert.Throws<ArgumentException>(() => Master.WriteMultipleRegisters(1, 1, Enumerable.Repeat<ushort>(1, 124).ToArray()));
        }

        [Fact]
        public void WriteMultipleCoils()
        {
            Assert.Throws<ArgumentNullException>(() => Master.WriteMultipleCoils(1, 1, null));
            Assert.Throws<ArgumentException>(() => Master.WriteMultipleCoils(1, 1, new bool[0]));
            Assert.Throws<ArgumentException>(() => Master.WriteMultipleCoils(1, 1, Enumerable.Repeat(false, 1969).ToArray()));
        }

        [Fact]
        public void ReadWriteMultipleRegisters()
        {
            // validate numberOfPointsToRead
            Assert.Throws<ArgumentException>(() => Master.ReadWriteMultipleRegisters(1, 1, 0, 1, new ushort[] { 1 }));
            Assert.Throws<ArgumentException>(() => Master.ReadWriteMultipleRegisters(1, 1, 126, 1, new ushort[] { 1 }));

            // validate writeData
            Assert.Throws<ArgumentNullException>(() => Master.ReadWriteMultipleRegisters(1, 1, 1, 1, null));
            Assert.Throws<ArgumentException>(() => Master.ReadWriteMultipleRegisters(1, 1, 1, 1, new ushort[0]));
            Assert.Throws<ArgumentException>(() => Master.ReadWriteMultipleRegisters(1, 1, 1, 1, Enumerable.Repeat<ushort>(1, 122).ToArray()));
        }

        [Fact]
        public void WriteFileRecord()
        {
            Assert.Throws<ArgumentNullException>(() => Master.WriteFileRecord(1, 1, 2, null));

            // Max message byte size is 256, minus 11 for overhead,
            // 244 data bytes are allowed (must be even), 246 should throw.
            Assert.Throws<ArgumentException>(() => Master.WriteFileRecord(1, 1, 2, Enumerable.Repeat<byte>(1, 246).ToArray()));
        }
    }
}
