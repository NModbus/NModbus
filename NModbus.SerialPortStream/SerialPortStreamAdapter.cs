﻿using System;
using NModbus.IO;

namespace NModbus.SerialPortStream
{
    /// <summary>
    /// An adapter for the SerialPortStream class. Useful for getting serial port access on non-Windows devices.
    /// </summary>
    public class SerialPortStreamAdapter : IStreamResource
    {
        private readonly RJCP.IO.Ports.SerialPortStream _serialPortStream;

        public SerialPortStreamAdapter(RJCP.IO.Ports.SerialPortStream serialPortStream)
        {
            _serialPortStream = serialPortStream;
        }

        public int InfiniteTimeout => RJCP.IO.Ports.SerialPortStream.InfiniteTimeout;

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

        public string Name => _serialPortStream.PortName;

        public void DiscardInBuffer()
        {
            _serialPortStream.DiscardInBuffer();
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
