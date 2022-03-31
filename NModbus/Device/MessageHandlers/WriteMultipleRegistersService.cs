using System;
using System.Linq;
using NModbus.Message;

namespace NModbus.Device.MessageHandlers
{
    internal class WriteMultipleRegistersService 
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

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use Handle with IServerDataStore parameter instead.")]
        protected override IModbusMessage Handle(WriteMultipleRegistersRequest request, ISlaveDataStore dataStore)
        {
            return Handle(request, dataStore as IServerDataStore);
        }
        protected override IModbusMessage Handle(WriteMultipleRegistersRequest request, IServerDataStore dataStore)
        {
            ushort[] registers = request.Data.ToArray();

            dataStore.HoldingRegisters.WritePoints(request.StartAddress, registers);

            return new WriteMultipleRegistersResponse(
                request.ServerAddress,
                request.StartAddress,
                request.NumberOfPoints);
        }
    }
}