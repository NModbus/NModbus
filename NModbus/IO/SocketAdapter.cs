using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using NModbus.Unme.Common;

namespace NModbus.IO
{
    /// <summary>
    ///     Concrete Implementor - http://en.wikipedia.org/wiki/Bridge_Pattern
    ///     This implementation is for sockets that Convert Rs485 to Ethernet.
    /// </summary>
    public class SocketAdapter : IStreamResource
    {
        private Socket _socketClient;

        public SocketAdapter(Socket socketClient)
        {
            Debug.Assert(socketClient != null, "Argument socketClient van not be null");
            _socketClient = socketClient;
        }

        public int InfiniteTimeout => Timeout.Infinite;
        public int ReadTimeout 
        { 
            get => _socketClient.SendTimeout;
            set => _socketClient.SendTimeout = value;

        }
        public int WriteTimeout
        {
            get => _socketClient.ReceiveTimeout;
            set => _socketClient.ReceiveTimeout = value;
        }
        public void DiscardInBuffer()
        {
            // socket does not hold buffers.
            return;
        }

        public int Read(byte[] buffer, int offset, int size)
        {
            
            return _socketClient.Receive(buffer,offset,size,0);
        }

        public void Write(byte[] buffer, int offset, int size)
        {
            _socketClient.Send(buffer,offset,size,0);
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
                DisposableUtility.Dispose(ref _socketClient);
            }
        }
    }
}