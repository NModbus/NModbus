using System;
using System.IO.Ports;
using Modbus.Device;
using Xunit;

namespace Modbus.IntegrationTests
{
    internal class NModbusSerialAsciiMasterFixture
    {
        [Fact]
        public void NModbusAsciiMaster_ReadTimeout()
        {
            SerialPort port = ModbusMasterFixture.CreateAndOpenSerialPort(ModbusMasterFixture.DefaultMasterSerialPortName);
            using (IModbusSerialMaster master = ModbusSerialMaster.CreateAscii(port))
            {
                master.Transport.ReadTimeout = master.Transport.WriteTimeout = 1000;
                Assert.Throws<TimeoutException>(() => master.ReadCoils(100, 1, 1));
            }
        }
    }
}
