using NModbus.IO;
using System;

namespace NModbus
{
    public interface IModbusTransport : IDisposable
    {
        int Retries { get; set; }

        uint RetryOnOldResponseThreshold { get; set; }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ServerBusyUsesRetryCount instead.")]
        bool SlaveBusyUsesRetryCount { get; set; }
        bool ServerBusyUsesRetryCount { get; set; }

        int WaitToRetryMilliseconds { get; set; }

        int ReadTimeout { get; set; }

        int WriteTimeout { get; set; }

        T UnicastMessage<T>(IModbusMessage message) where T : IModbusMessage, new();

        byte[] ReadRequest();

        byte[] BuildMessageFrame(IModbusMessage message);

        void Write(IModbusMessage message);

        IStreamResource StreamResource { get; }
    }
}