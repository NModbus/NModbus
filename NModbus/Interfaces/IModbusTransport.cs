using System;
using NModbus.IO;
using NModbus.Message;

namespace NModbus.Interfaces
{
    public interface IModbusTransport : IDisposable
    {
        int Retries { get; set; }

        uint RetryOnOldResponseThreshold { get; set; }

        bool SlaveBusyUsesRetryCount { get; set; }

        int WaitToRetryMilliseconds { get; set; }

        int ReadTimeout { get; set; }

        int WriteTimeout { get; set; }

        IStreamResource StreamResource { get; }

        T UnicastMessage<T>(IModbusMessage message) where T : IModbusMessage, new();

        byte[] ReadRequest();

        byte[] BuildMessageFrame(IModbusMessage message);

        void Write(IModbusMessage message);
    }
}