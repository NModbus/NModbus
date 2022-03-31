using System;
using NModbus.Message;

namespace NModbus.Device.MessageHandlers
{
    internal class WriteSingleCoilService : ModbusFunctionServiceBase<WriteSingleCoilRequestResponse>
    {
        public WriteSingleCoilService() 
            : base(ModbusFunctionCodes.WriteSingleCoil)
        {
        }

        public override IModbusMessage CreateRequest(byte[] frame)
        {
            return CreateModbusMessage<WriteSingleCoilRequestResponse>(frame);
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
        protected override IModbusMessage Handle(WriteSingleCoilRequestResponse request, ISlaveDataStore dataStore)
        {
            return Handle(request, dataStore as IServerDataStore);
        }
        protected override IModbusMessage Handle(WriteSingleCoilRequestResponse request, IServerDataStore dataStore)
        {
            bool[] values = new bool[]
            {
                request.Data[0] == Modbus.CoilOn
            };

             dataStore.CoilDiscretes.WritePoints(request.StartAddress, values);

            return request;
        }
    }
}