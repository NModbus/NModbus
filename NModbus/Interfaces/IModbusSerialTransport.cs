using NModbus.Message;

namespace NModbus.Interfaces
{
    public interface IModbusSerialTransport : IModbusTransport
    {
        void DiscardInBuffer();

        bool CheckFrame { get; set; }

        bool ChecksumsMatch(IModbusMessage message, byte[] messageFrame);
    }
}