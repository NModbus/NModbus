using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NModbus.Extensions;
using NModbus.IO;
using NModbus.Message;

namespace NModbus.Device
{
    public abstract class ModbusSlaveNetwork : ModbusDevice
    {
        private readonly IDictionary<byte, ModbusSlave> _slaves = new ConcurrentDictionary<byte, ModbusSlave>();

        protected ModbusSlaveNetwork(ModbusTransport transport) 
            : base(transport)
        {
        }

        /// <summary>
        /// Start slave listening for requests.
        /// </summary>
        public abstract Task ListenAsync();

        /// <summary>
        /// Apply the request.
        /// </summary>
        /// <param name="request"></param>
        protected void ApplyRequest(IModbusMessage request)
        {
            ModbusSlave slave = GetSlave(request.SlaveAddress);

            // only service requests addressed to our slaves
            if (slave == null)
            {
                Debug.WriteLine($"NModbus Slave Network ignoring request intended for NModbus Slave {request.SlaveAddress}");
            }
            else
            {
                // perform action
                IModbusMessage response = slave.ApplyRequest(request);

                // write response
                Transport.Write(response);
            }
        }

        public Task AddSlaveAsync(ModbusSlave slave)
        {
            if (slave == null) throw new ArgumentNullException(nameof(slave));

            _slaves.Add(slave.UnitId, slave);

            return Task.FromResult(0);
        }

        public Task RemoveSlaveAsync(byte unitId)
        {
            _slaves.Remove(unitId);

            return Task.FromResult(0);
        }

        private ModbusSlave GetSlave(byte unitId)
        {
            return _slaves.GetValueOrDefault(unitId);
        }
    }
}