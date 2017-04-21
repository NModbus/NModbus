using System.Threading.Tasks;

namespace NModbus.Interfaces
{
    public interface IModbusSlaveNetwork 
    {
        Task ListenAsync();

        void AddSlave(IModbusSlave slave);

        IModbusSlave GetSlave(byte unitId);

        void RemoveSlave(byte unitId);
    }
}