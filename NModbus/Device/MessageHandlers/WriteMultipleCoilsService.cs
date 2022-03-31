using System;
using System.Linq;
using NModbus.Message;

namespace NModbus.Device.MessageHandlers
{
    internal class WriteMultipleCoilsService : ModbusFunctionServiceBase<WriteMultipleCoilsRequest>
    {
        public WriteMultipleCoilsService() 
            : base(ModbusFunctionCodes.WriteMultipleCoils)
        {
        }

        public override IModbusMessage CreateRequest(byte[] frame)
        {
            return CreateModbusMessage<WriteMultipleCoilsRequest>(frame);
        }

        public override int GetRtuRequestBytesToRead(byte[] frameStart)
        {
            return frameStart[6] + 2;
        }

        public override int GetRtuResponseBytesToRead(byte[] frameStart)
        {
            return 4;
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use Handle with IServerDataStore parameter instead.")]
        protected override IModbusMessage Handle(WriteMultipleCoilsRequest request, ISlaveDataStore dataStore)
        {
            return Handle(request, dataStore as IServerDataStore);
        }

        protected override IModbusMessage Handle(WriteMultipleCoilsRequest request, IServerDataStore dataStore)
        {
            bool[] points = request.Data
                .Take(request.NumberOfPoints)
                .ToArray();

            dataStore.CoilDiscretes.WritePoints(request.StartAddress, points);

            return new WriteMultipleCoilsResponse(
               request.ServerAddress,
               request.StartAddress,
               request.NumberOfPoints);
        }
    }
}