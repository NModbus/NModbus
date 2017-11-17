using System;
using RJCP.IO.Ports;
using NModbus.IO;

// ReSharper disable once CheckNamespace
namespace NModbus.Serial
{
    /// <summary>
    /// An adapter for the SerialPortStream class. Useful for getting serial port access on non-Windows devices.
    /// </summary>
    public class SerialPortStreamAdapter : IStreamResource
    {
        private readonly SerialPortStream _serialPortStream;

        public SerialPortStreamAdapter(SerialPortStream serialPortStream)
        {
            _serialPortStream = serialPortStream;
        }

        public int InfiniteTimeout => SerialPortStream.InfiniteTimeout;

        public int ReadTimeout
        {
            get => _serialPortStream.ReadTimeout;
            set => _serialPortStream.ReadTimeout = value;
        }
        public int WriteTimeout
        {
            get => _serialPortStream.WriteTimeout;
            set => _serialPortStream.WriteTimeout = value;
        }

        public void DiscardInBuffer()
        {
            _serialPortStream.Flush();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int result = _serialPortStream.Read(buffer, offset, count);

            if (result == 0)
                throw new TimeoutException();

            return result;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _serialPortStream.Write(buffer, offset, count);
        }

        public void Dispose()
        {
            _serialPortStream.Dispose();
        }
    }
}
