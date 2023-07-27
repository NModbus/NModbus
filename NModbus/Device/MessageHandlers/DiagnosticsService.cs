namespace NModbus.Device.MessageHandlers
{
    using Message;

    public class DiagnosticsService : ModbusFunctionServiceBase<IModbusMessage>
    {
        public DiagnosticsService() 
            : base(ModbusFunctionCodes.Diagnostics)
        {
        }

        public override IModbusMessage CreateRequest(byte[] frame)
        {
            return CreateModbusMessage<DiagnosticsRequestResponse>(frame);
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