using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;
using NModbus.IO;

namespace CaptiveAire.NModbus.Serial.UWP
{
    /// <summary>
    /// UWP SerialDevice Modbus Adapter
    /// </summary>
    public class SerialDeviceAdapter : IStreamResource
    {
        private readonly SerialDevice _serialDevice;
        private readonly DataReader inputStream;
        private readonly DataWriter outputStream;
        public int InfiniteTimeout => Timeout.Infinite;

        public int ReadTimeout { get => _serialDevice.ReadTimeout.Milliseconds; set => _serialDevice.ReadTimeout = TimeSpan.FromMilliseconds(value); }
        public int WriteTimeout { get => _serialDevice.WriteTimeout.Milliseconds; set => _serialDevice.WriteTimeout = TimeSpan.FromMilliseconds(value); }

        public SerialDeviceAdapter(SerialDevice serialDevice)
        {
            Debug.Assert(serialDevice != null, "Argument serialDevice cannot be null");
            _serialDevice = serialDevice;
            inputStream = new DataReader(_serialDevice.InputStream);
            outputStream = new DataWriter(_serialDevice.OutputStream);
        }

        public void DiscardInBuffer()
        {
            Task.Run(async () => await _serialDevice.OutputStream.FlushAsync());
        }

        public void Dispose()
        {
            _serialDevice.Dispose();
        }

        public int Read(byte[] buffer, int offset, int count)
        {

            Task t = Task.Run(async () => await inputStream.LoadAsync((uint)count));
            t.Wait();
            while (inputStream.UnconsumedBufferLength > 0)
            {
                inputStream.ReadBytes(buffer);
            }
            return buffer.Length;

        }

        public void Write(byte[] buffer, int offset, int count)
        {
            outputStream.WriteBytes(buffer);
            outputStream.StoreAsync();
        }
    }
}
