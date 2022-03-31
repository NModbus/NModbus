using System;
using NModbus.Data;
using NModbus.Message;

namespace NModbus.Device.MessageHandlers
{
    internal class ReadInputRegistersService : ModbusFunctionServiceBase<ReadHoldingInputRegistersRequest>
    {
        public ReadInputRegistersService() 
            : base(ModbusFunctionCodes.ReadInputRegisters)
        {
        }

        public override IModbusMessage CreateRequest(byte[] frame)
        {
            return CreateModbusMessage<ReadHoldingInputRegistersRequest>(frame);
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
        protected override IModbusMessage Handle(ReadHoldingInputRegistersRequest request, ISlaveDataStore dataStore)
        {
            return Handle(request, dataStore as IServerDataStore);
        }
        protected override IModbusMessage Handle(ReadHoldingInputRegistersRequest request, IServerDataStore dataStore)
        {
            ushort[] registers = dataStore.InputRegisters.ReadPoints(request.StartAddress, request.NumberOfPoints);

            RegisterCollection regsiterCollection = new RegisterCollection(registers);

            return new ReadHoldingInputRegistersResponse(request.FunctionCode, request.ServerAddress, regsiterCollection);
        }
    }
}