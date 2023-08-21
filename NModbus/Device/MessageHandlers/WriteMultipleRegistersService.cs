using System.Linq;
using NModbus.Message;

namespace NModbus.Device.MessageHandlers
{
    public class WriteMultipleRegistersService 
        : ModbusFunctionServiceBase<WriteMultipleRegistersRequest>
    {
        public WriteMultipleRegistersService() 
            : base(ModbusFunctionCodes.WriteMultipleRegisters)
        {
        }

        public override IModbusMessage CreateRequest(byte[] frame)
        {
            return CreateModbusMessage<WriteMultipleRegistersRequest>(frame);
        }

        public override int GetRtuRequestBytesToRead(byte[] frameStart)
        {
            return frameStart[6] + 2;
        }

        public override int GetRtuResponseBytesToRead(byte[] frameStart)
        {
            return 4;
        }

        protected override IModbusMessage Handle(WriteMultipleRegistersRequest request, ISlaveDataStore dataStore)
        {
            ushort[] registers = request.Data.ToArray();

            dataStore.HoldingRegisters.WritePoints(request.StartAddress, registers);

            return new WriteMultipleRegistersResponse(
                request.SlaveAddress,
                request.StartAddress,
                request.NumberOfPoints);
        }
    }
}