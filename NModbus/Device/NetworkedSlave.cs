using System;
using System.Collections.Generic;
using System.Linq;
using NModbus.Extensions;
using NModbus.Interfaces;
using NModbus.Message;

namespace NModbus.Device
{
    public class NetworkedSlave : IModbusSlave
    {
        private readonly byte _unitId;
        private readonly ISlaveDataStore _dataStore;

        private readonly IDictionary<byte, ISlaveMessageHandler> _handlers;

        public NetworkedSlave(byte unitId, ISlaveDataStore dataStore, IEnumerable<ISlaveMessageHandler> handlers)
        {
            if (dataStore == null) throw new ArgumentNullException(nameof(dataStore));

            _unitId = unitId;
            _dataStore = dataStore;
            _handlers = handlers.ToDictionary(h => h.FunctionCode, h => h);
        }

        public byte UnitId
        {
            get { return _unitId; }
        }

        public ISlaveDataStore DataStore
        {
            get { return _dataStore; }
        }

        public IModbusMessage ApplyRequest(IModbusMessage request)
        {
            IModbusMessage response;

            try
            {
                //Try to get a handler for this function.
                ISlaveMessageHandler handler = _handlers.GetValueOrDefault(request.FunctionCode);

                //Check to see if we found a handler for this function code.
                if (handler == null)
                {
                    throw new InvalidModbusRequestException(Modbus.IllegalFunction);
                }

                //Process the request
                response = handler.Handle(request, DataStore);
            }
            catch (InvalidModbusRequestException ex)
            {
                // Catches the exception for an illegal function or a custom exception from the ModbusSlaveRequestReceived event.
                response = new SlaveExceptionResponse(
                    request.SlaveAddress,
                    (byte) (Modbus.ExceptionOffset + request.FunctionCode),
                    ex.ExceptionCode);
            }

            return response;
        }
    }
}