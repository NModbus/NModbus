using Modbus.Device;
using Xunit;

namespace Modbus.IntegrationTests
{
    internal class NModbusSerialRtuMasterNModbusSerialRtuSlaveFixture : ModbusSerialMasterFixture
    {
        public NModbusSerialRtuMasterNModbusSerialRtuSlaveFixture()
        {
            SetupSlaveSerialPort();
            Slave = ModbusSerialSlave.CreateRtu(SlaveAddress, SlaveSerialPort);
            StartSlave();

            MasterSerialPort = CreateAndOpenSerialPort(DefaultMasterSerialPortName);
            Master = ModbusSerialMaster.CreateRtu(MasterSerialPort);
        }

        [Fact]
        public override void ReadCoils()
        {
            base.ReadCoils();
        }

        [Fact]
        public override void ReadHoldingRegisters()
        {
            base.ReadHoldingRegisters();
        }

        [Fact]
        public override void ReadInputs()
        {
            base.ReadInputs();
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

        [Fact(Skip = "Need to fix RTU slave for this function code")]
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
