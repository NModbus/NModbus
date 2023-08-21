using System;
using NModbus.Unme.Common;

namespace NModbus.Device
{
    /// <summary>
    ///     Modbus device.
    /// </summary>
    public abstract class ModbusDevice : IDisposable
    {
        private IModbusTransport _transport;

        protected ModbusDevice(IModbusTransport transport)
        {
            _transport = transport;
        }

        /// <summary>
        ///     Gets the Modbus Transport.
        /// </summary>
        public IModbusTransport Transport => _transport;

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources;
        ///     <c>false</c> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposableUtility.Dispose(ref _transport);
            }
        }
    }
}
