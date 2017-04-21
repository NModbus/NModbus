using System.Linq;
using NModbus.Interfaces;
using NModbus.Message;

namespace NModbus.Device.MessageHandlers
{
    internal class WriteMultipleCoilsService : ModbusFunctionServiceBase<WriteMultipleCoilsRequest>
    {
        public WriteMultipleCoilsService() 
            : base(ModbusFunctionCodes.WriteMultipleCoils)
        {
        }

        public override int GetRtuRequestBytesToRead(byte[] frameStart)
        {
            return frameStart[6] + 2;
        }

        public override int GetRtuResponseBytesToRead(byte[] frameStart)
        {
            return 4;
        }

        protected override IModbusMessage Handle(WriteMultipleCoilsRequest request, ISlaveDataStore dataStore)
        {
            bool[] points = request.Data
                .Take(request.NumberOfPoints)
                .ToArray();

            dataStore.CoilDiscretes.WritePoints(request.StartAddress, points);

            return new WriteMultipleCoilsResponse(
               request.SlaveAddress,
               request.StartAddress,
               request.NumberOfPoints);
        }
    }
}