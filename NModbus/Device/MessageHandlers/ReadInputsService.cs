using System;
using NModbus.Data;
using NModbus.Message;

namespace NModbus.Device.MessageHandlers
{
    internal class ReadInputsService : ModbusFunctionServiceBase<ReadCoilsInputsRequest>
    {
        public ReadInputsService() 
            : base(ModbusFunctionCodes.ReadInputs)
        {
        }

        public override IModbusMessage CreateRequest(byte[] frame)
        {
            return CreateModbusMessage<ReadCoilsInputsRequest>(frame);
        }

        public override int GetRtuRequestBytesToRead(byte[] frameStart)
        {
            return 1;
        }

        public override int GetRtuResponseBytesToRead(byte[] frameStart)
        {
            return frameStart[2] + 1;
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use Handle with IServerDataStore parameter instead.")]
        protected override IModbusMessage Handle(ReadCoilsInputsRequest request, ISlaveDataStore dataStore)
        {
            return Handle(request, dataStore as IServerDataStore);
        }
        protected override IModbusMessage Handle(ReadCoilsInputsRequest request, IServerDataStore dataStore)
        {
            bool[] discretes = dataStore.CoilInputs.ReadPoints(request.StartAddress, request.NumberOfPoints);

            DiscreteCollection data = new DiscreteCollection(discretes);

            return new ReadCoilsInputsResponse(
                request.FunctionCode,
                request.ServerAddress,
                data.ByteCount,
                data);
        }
    }
}