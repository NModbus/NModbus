using Modbus.Device;
using Xunit;

namespace Modbus.IntegrationTests
{
    internal class NModbusSerialAsciiMasterNModbusSerialAsciiSlaveFixture : ModbusSerialMasterFixture
    {
        public NModbusSerialAsciiMasterNModbusSerialAsciiSlaveFixture()
        {
            MasterSerialPort = CreateAndOpenSerialPort(DefaultMasterSerialPortName);
            Master = ModbusSerialMaster.CreateAscii(MasterSerialPort);
            SetupSlaveSerialPort();
            Slave = ModbusSerialSlave.CreateAscii(SlaveAddress, SlaveSerialPort);

            StartSlave();
        }

        [Fact]
        public override void ReadCoils()
        {
            base.ReadCoils();
        }

        [Fact]
        public override void ReadInputs()
        {
            base.ReadInputs();
        }

        [Fact]
        public override void ReadHoldingRegisters()
        {
            base.ReadHoldingRegisters();
        }

        [Fact]
        public override void ReadInputRegisters()
        {
            base.ReadInputRegisters();
        }

        [Fact]
        public override void WriteSingleCoil()
        {
            base.WriteSingleCoil();
        }

        [Fact]
        public override void WriteMultipleCoils()
        {
            base.WriteMultipleCoils();
        }

        [Fact]
        public override void WriteSingleRegister()
        {
            base.WriteSingleRegister();
        }

        [Fact]
        public override void WriteMultipleRegisters()
        {
            base.WriteMultipleRegisters();
        }

        [Fact]
        public override void ReadWriteMultipleRegisters()
        {
            base.ReadWriteMultipleRegisters();
        }

        [Fact]
        public override void ReturnQueryData()
        {
            base.ReturnQueryData();
        }
    }
}
