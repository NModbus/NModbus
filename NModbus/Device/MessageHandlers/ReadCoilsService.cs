using NModbus.Data;
using NModbus.Interfaces;
using NModbus.Message;

namespace NModbus.Device.MessageHandlers
{
    internal class ReadCoilsService : ModbusFunctionServiceBase<ReadCoilsInputsRequest>
    {
        public ReadCoilsService() 
            : base(ModbusFunctionCodes.ReadCoils)
        {
        }

        public override int GetRtuRequestBytesToRead(byte[] frameStart)
        {
            return 1;
        }

        public override int GetRtuResponseBytesToRead(byte[] frameStart)
        {
            return frameStart[2] + 1;
        }

        protected override IModbusMessage Handle(ReadCoilsInputsRequest request, ISlaveDataStore dataStore)
        {
            bool[] discretes = dataStore.CoilDiscretes.ReadPoints(request.StartAddress, request.NumberOfPoints);

            DiscreteCollection data = new DiscreteCollection(discretes);

            return new ReadCoilsInputsResponse(
                request.FunctionCode, 
                request.SlaveAddress, 
                data.ByteCount, data);
        }
    }
}