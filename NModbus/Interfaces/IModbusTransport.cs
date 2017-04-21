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

        IModbusMessage CreateResponse<T>(byte[] frame) where T : IModbusMessage, new();

        void ValidateResponse(IModbusMessage request, IModbusMessage response);

        bool ShouldRetryResponse(IModbusMessage request, IModbusMessage response);

        bool OnShouldRetryResponse(IModbusMessage request, IModbusMessage response);

        byte[] ReadRequest();

        IModbusMessage ReadResponse<T>() where T : IModbusMessage, new();

        byte[] BuildMessageFrame(IModbusMessage message);

        void Write(IModbusMessage message);



    }
}