﻿using System;
using System.Linq;
using NModbus.Message;

namespace NModbus.Device.MessageHandlers
{
    internal class WriteSingleRegisterService : ModbusFunctionServiceBase<WriteSingleRegisterRequestResponse>
    {
        public WriteSingleRegisterService() 
            : base(ModbusFunctionCodes.WriteSingleRegister)
        {
        }

        public override IModbusMessage CreateRequest(byte[] frame)
        {
            return CreateModbusMessage<WriteSingleRegisterRequestResponse>(frame);
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
        protected override IModbusMessage Handle(WriteSingleRegisterRequestResponse request, ISlaveDataStore dataStore)
        {
            return Handle(request, dataStore as IServerDataStore);
        }
        protected override IModbusMessage Handle(WriteSingleRegisterRequestResponse request, IServerDataStore dataStore)
        {
            ushort[] points = request.Data
                .ToArray();

            dataStore.HoldingRegisters.WritePoints(request.StartAddress, points);

            return request;
        }
    }
}