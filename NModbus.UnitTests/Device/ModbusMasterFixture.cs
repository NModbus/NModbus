using System;
using System.Linq;
using Modbus.Device;
using Modbus.IO;
using Moq;
using Xunit;

namespace Modbus.UnitTests.Device
{
    public class ModbusMasterFixture
    {
        private static IStreamResource StreamRsource => new Mock<IStreamResource>(MockBehavior.Strict).Object;

        private ModbusSerialMaster Master => ModbusSerialMaster.CreateRtu(StreamRsource);

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
    }
}
