using Modbus.Device;
using Xunit;

namespace Modbus.IntegrationTests
{
    internal class NModbusSerialRtuMasterDl06SlaveFixture : ModbusSerialMasterFixture
    {
        public NModbusSerialRtuMasterDl06SlaveFixture()
        {
            MasterSerialPort = CreateAndOpenSerialPort("COM1");
            Master = ModbusSerialMaster.CreateRtu(MasterSerialPort);
        }

        /// <summary>
        /// Not supported by the DL06
        /// </summary>
        public override void ReadWriteMultipleRegisters()
        {
        }

        /// <summary>
        /// Not supported by the DL06
        /// </summary>
        public override void ReturnQueryData()
        {
        }

        [Fact]
        public override void ReadCoils()
        {
            base.ReadCoils();
        }
    }
}
