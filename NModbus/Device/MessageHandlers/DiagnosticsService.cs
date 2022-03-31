using System;

namespace NModbus.Device.MessageHandlers
{
    using Message;

    internal class DiagnosticsService : ModbusFunctionServiceBase<IModbusMessage>
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

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use Handle with IServerDataStore parameter instead.")]
        protected override IModbusMessage Handle(IModbusMessage request, ISlaveDataStore dataStore)
        {
            return Handle(request, dataStore as IServerDataStore);
        }
        protected override IModbusMessage Handle(IModbusMessage request, IServerDataStore dataStore)
        {
            return request;
        }
    }
}