using System.Collections.Generic;
using NModbus.Data;
using NModbus.Interfaces;

namespace NModbus.Device
{
    public static class SlaveFactory
    {
        private static readonly ISlaveMessageHandler[] Handlers = new ISlaveMessageHandler[]
        {
            
        };

        public static IModbusSlave Create(byte unitId, ISlaveDataStore dataStore = null)
        {
            if (dataStore == null)
                dataStore = new DefaultSlaveDataStore();

            return new NetworkedSlave(unitId, dataStore, Handlers);
        }
    }
}