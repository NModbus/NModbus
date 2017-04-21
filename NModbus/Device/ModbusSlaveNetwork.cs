using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using NModbus.Extensions;
using NModbus.Interfaces;
using NModbus.IO;
using NModbus.Message;

namespace NModbus.Device
{
    public abstract class ModbusSlaveNetwork : ModbusDevice, IModbusSlaveNetwork
    {
        private readonly IDictionary<byte, IModbusSlave> _slaves = new ConcurrentDictionary<byte, IModbusSlave>();

        protected ModbusSlaveNetwork(IModbusTransport transport) 
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
            IModbusSlave slave = GetSlave(request.SlaveAddress);

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

        public void AddSlave(IModbusSlave slave)
        {
            if (slave == null) throw new ArgumentNullException(nameof(slave));

            _slaves.Add(slave.UnitId, slave);
        }

        public void RemoveSlave(byte unitId)
        {
            _slaves.Remove(unitId);
        }

        private IModbusSlave GetSlave(byte unitId)
        {
            return _slaves.GetValueOrDefault(unitId);
        }
    }
}