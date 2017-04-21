using NModbus.Data;
using NModbus.Device.MessageHandlers;
using NModbus.Interfaces;

namespace NModbus.Device
{
    /// <summary>
    /// Creates Modbus slaves.
    /// </summary>
    public static class SlaveFactory
    {
        /// <summary>
        /// The "built-in" message handlers.
        /// </summary>
        private static readonly IModbusFunctionService[] Handlers = new IModbusFunctionService[]
        {
            new ReadCoilsService(), 
            new ReadInputsService(),
            new ReadHoldingRegistersService(),
            new ReadInputRegistersService(), 
            new DiagnosticsService(), 
            new WriteSingleCoilService(), 
            new WriteSingleRegisterService(), 
            new WriteMultipleCoilsService(), 
            new WriteMultipleRegistersService(), 
            new ReadWriteMultipleRegistersService(), 
        };

        public static IModbusSlave Create(byte unitId, ISlaveDataStore dataStore = null)
        {
            if (dataStore == null)
                dataStore = new DefaultSlaveDataStore();

            return new NetworkedSlave(unitId, dataStore, Handlers);
        }
    }
}