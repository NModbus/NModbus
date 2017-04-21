using System.Threading.Tasks;

namespace NModbus.Interfaces
{
    public interface IModbusSlaveNetwork 
    {
        Task ListenAsync();

        void AddSlave(IModbusSlave slave);

        void RemoveSlave(byte unitId);
    }
}