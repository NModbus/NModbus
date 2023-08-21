using System;
using System.Linq;
using NModbus.Data;
using NModbus.Message;

namespace NModbus.Device.MessageHandlers
{
    public class ReadWriteMultipleRegistersService : ModbusFunctionServiceBase<ReadWriteMultipleRegistersRequest>
    {
        public ReadWriteMultipleRegistersService() 
            : base(ModbusFunctionCodes.ReadWriteMultipleRegisters)
        {
        }

        public override IModbusMessage CreateRequest(byte[] frame)
        {
            return CreateModbusMessage<ReadWriteMultipleRegistersRequest>(frame);
        }

        public override int GetRtuRequestBytesToRead(byte[] frameStart)
        {
            throw new NotSupportedException();
        }

        public override int GetRtuResponseBytesToRead(byte[] frameStart)
        {
            throw new NotSupportedException();
        }

        protected override IModbusMessage Handle(ReadWriteMultipleRegistersRequest request, ISlaveDataStore dataStore)
        {
            ushort[] pointsToWrite = request.WriteRequest.Data
                .ToArray();

            dataStore.HoldingRegisters.WritePoints(request.ReadRequest.StartAddress, pointsToWrite);

            ushort[] readPoints = dataStore.HoldingRegisters.ReadPoints(request.ReadRequest.StartAddress,
                request.ReadRequest.NumberOfPoints);

            RegisterCollection data = new RegisterCollection(readPoints);

            return new ReadHoldingInputRegistersResponse(
                request.FunctionCode,
                request.SlaveAddress,
                data);
        }
    }
}