using NModbus.Message;

namespace NModbus.Interfaces
{
    public interface ISlaveMessageHandler
    {
        byte FunctionCode { get; }

        IModbusMessage Handle(IModbusMessage request, ISlaveDataStore dataStore);
    }
}