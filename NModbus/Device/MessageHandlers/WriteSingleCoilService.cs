using NModbus.Interfaces;
using NModbus.Message;

namespace NModbus.Device.MessageHandlers
{
    internal class WriteSingleCoilService : ModbusFunctionServiceBase<WriteSingleCoilRequestResponse>
    {
        public WriteSingleCoilService() 
            : base(ModbusFunctionCodes.WriteSingleCoil)
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

        protected override IModbusMessage Handle(WriteSingleCoilRequestResponse request, ISlaveDataStore dataStore)
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