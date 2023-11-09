﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using NModbus.Unme.Common;

namespace NModbus.IO
{
    /// <summary>
    ///     Concrete Implementor - http://en.wikipedia.org/wiki/Bridge_Pattern
    /// </summary>
    public class UdpClientAdapter : IStreamResource
    {
        // strategy for cross platform r/w
        private const int MaxBufferSize = ushort.MaxValue;
        private UdpClient _udpClient;
        private readonly byte[] _buffer = new byte[MaxBufferSize];
        private int _bufferOffset;
        //Slave
        internal EndPoint _endPoint;

        public UdpClientAdapter(UdpClient udpClient,EndPoint endpoint = null)
        {
            if (udpClient == null)
            {
                throw new ArgumentNullException(nameof(udpClient));
            }

            _udpClient = udpClient;
            this._endPoint = endpoint?? new IPEndPoint(IPAddress.Any, 502);
        }

        public int InfiniteTimeout => Timeout.Infinite;

        public int ReadTimeout
        {
            get => _udpClient.Client.ReceiveTimeout;
            set => _udpClient.Client.ReceiveTimeout = value;
        }

        public int WriteTimeout
        {
            get => _udpClient.Client.SendTimeout;
            set => _udpClient.Client.SendTimeout = value;
        }

        public string Name => _endPoint.ToString();

        public void DiscardInBuffer()
        {
            // no-op
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(offset),
                    "Argument offset must be greater than or equal to 0.");
            }

            if (offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(offset),
                    "Argument offset cannot be greater than the length of buffer.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    "Argument count must be greater than or equal to 0.");
            }

            if (count > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    "Argument count cannot be greater than the length of buffer minus offset.");
            }

            if (_bufferOffset == 0)
            {
                _bufferOffset = _udpClient.Client.ReceiveFrom(_buffer, ref _endPoint);
            }

            if (_bufferOffset < count)
            {
                throw new IOException("Not enough bytes in the datagram.");
            }

            Buffer.BlockCopy(_buffer, 0, buffer, offset, count);
            _bufferOffset -= count;
            Buffer.BlockCopy(_buffer, count, _buffer, 0, _bufferOffset);

            return count;
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(offset), 
                    "Argument offset must be greater than or equal to 0.");
            }

            if (offset > buffer.Length)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(offset), 
                    "Argument offset cannot be greater than the length of buffer.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count), 
                    "Argument count must be greater than or equal to 0.");
            }

            if (count > buffer.Length - offset)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count), 
                    "Argument count cannot be greater than the length of buffer minus offset.");
            }

            _udpClient.Client.SendTo(buffer.Skip(offset).Take(count).ToArray(), _endPoint); 
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposableUtility.Dispose(ref _udpClient);
            }
        }
    }
}
