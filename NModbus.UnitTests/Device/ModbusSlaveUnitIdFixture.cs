using Moq;
using NModbus.Device;
using NModbus.Logging;
using System;
using Xunit;

namespace NModbus.UnitTests.Device
{
    public class ModbusSlaveUnitIdFixture
    {
        /// <summary>
        ///     Serial line spec: a slave's own address must be in the range 1-247.
        ///     Address 0 is broadcast-only and 248-255 are reserved, so AddSlave
        ///     must reject slaves configured with an invalid unit ID.
        /// </summary>
        [Theory]
        [InlineData(0)]    // broadcast address, not valid for a slave
        [InlineData(248)]  // reserved by the Modbus spec
        [InlineData(255)]  // out of range
        public void AddSlave_RejectsInvalidUnitId(byte unitId)
        {
            var network = new ModbusSerialSlaveNetwork(new Mock<IModbusSerialTransport>().Object, new ModbusFactory(), NullModbusLogger.Instance);
            var slave = new ModbusFactory().CreateSlave(unitId);

            Assert.Throws<ArgumentOutOfRangeException>(() => network.AddSlave(slave));
        }

        /// <summary>
        ///     Boundary values 1 and 247 are valid slave addresses per the
        ///     serial line spec and must be accepted by AddSlave.
        /// </summary>
        [Theory]
        [InlineData(1)]    // lower bound
        [InlineData(247)]  // upper bound
        public void AddSlave_AcceptsValidUnitId(byte unitId)
        {
            var network = new ModbusSerialSlaveNetwork(new Mock<IModbusSerialTransport>().Object, new ModbusFactory(), NullModbusLogger.Instance);
            var slave = new ModbusFactory().CreateSlave(unitId);

            network.AddSlave(slave);   // no exception = pass
            Assert.Same(slave, network.GetSlave(unitId));
        }
    }
}
