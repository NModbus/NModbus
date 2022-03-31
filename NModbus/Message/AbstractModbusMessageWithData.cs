using NModbus.Data;

namespace NModbus.Message
{
    internal abstract class AbstractModbusMessageWithData<TData> : AbstractModbusMessage
        where TData : IModbusMessageDataCollection
    {
        internal AbstractModbusMessageWithData()
        {
        }

        internal AbstractModbusMessageWithData(byte serverAddress, byte functionCode)
            : base(serverAddress, functionCode)
        {
        }

        public TData Data
        {
            get => (TData)MessageImpl.Data;
            set => MessageImpl.Data = value;
        }
    }
}
