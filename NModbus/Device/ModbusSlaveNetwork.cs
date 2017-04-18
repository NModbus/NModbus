using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Modbus.IO;

namespace Modbus.Device
{
    public abstract class ModbusSlaveNetwork : ModbusDevice
    {
        private readonly IDictionary<byte, ModbusSlave> _slaves = new ConcurrentDictionary<byte, ModbusSlave>();

        protected ModbusSlaveNetwork(ModbusTransport transport) 
            : base(transport)
        {
        }

        /// <summary>
        ///     Start slave listening for requests.
        /// </summary>
        public abstract Task ListenAsync();

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

        protected ModbusSlave GetSlave(byte unitId)
        {
            ModbusSlave slave;

            _slaves.TryGetValue(unitId, out slave);

            return slave;
        }
    }
}