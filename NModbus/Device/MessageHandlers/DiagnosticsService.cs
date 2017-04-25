namespace NModbus.Device.MessageHandlers
{
    internal class DiagnosticsService : ModbusFunctionServiceBase<IModbusMessage>
    {
        public DiagnosticsService() 
            : base(ModbusFunctionCodes.Diagnostics)
        {
        }

        public override int GetRtuRequestBytesToRead(byte[] frameStart)
        {
            return 1;
        }

        public override int GetRtuResponseBytesToRead(byte[] frameStart)
        {
            return 4;
        }

        protected override IModbusMessage Handle(IModbusMessage request, ISlaveDataStore dataStore)
        {
            return request;
        }
    }
}