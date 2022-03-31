using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NModbus.Extensions;
using NModbus.Message;

namespace NModbus.Device 
{
    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ModbusServer instead.")]
    internal class ModbusSlave : ModbusServer, IModbusSlave
    {
        public ModbusSlave(byte unitId, ISlaveDataStore dataStore, IEnumerable<IModbusFunctionService> handlers) : base(unitId, dataStore, handlers) { }
    }

    internal class ModbusServer : IModbusServer
    {
        private readonly byte _unitId;
        private readonly IServerDataStore _dataStore;

        private readonly IDictionary<byte, IModbusFunctionService> _handlers;

        public ModbusServer(byte unitId, IServerDataStore dataStore, IEnumerable<IModbusFunctionService> handlers)
        {
            if (dataStore == null) throw new ArgumentNullException(nameof(dataStore));
            if (handlers == null) throw new ArgumentNullException(nameof(handlers));

            _unitId = unitId;
            _dataStore = dataStore;
            _handlers = handlers.ToDictionary(h => h.FunctionCode, h => h);
        }

        public byte UnitId => _unitId;

        public IServerDataStore DataStore => _dataStore;

        public IModbusMessage ApplyRequest(IModbusMessage request)
        {
            IModbusMessage response;

            try
            {
                //Try to get a handler for this function.
                IModbusFunctionService handler = _handlers.GetValueOrDefault(request.FunctionCode);

                //Check to see if we found a handler for this function code.
                if (handler == null)
                {
                    throw new InvalidModbusRequestException(ServerExceptionCodes.IllegalFunction);
                }

                //Process the request
                response = handler.HandleServerRequest(request, DataStore);
            }
            catch (InvalidModbusRequestException ex)
            {
                // Catches the exception for an illegal function or a custom exception from the ModbusServerRequestReceived event.
                response = new ServerExceptionResponse(
                    request.ServerAddress,
                    (byte) (Modbus.ExceptionOffset + request.FunctionCode),
                    ex.ExceptionCode);
            }
#if NET45 || NET46
            catch (Exception ex)
            {
                //Okay - this is no beuno.
                response = new ServerExceptionResponse(request.ServerAddress,
                    (byte) (Modbus.ExceptionOffset + request.FunctionCode),
                    ServerExceptionCodes.ServerDeviceFailure);

                //Give the consumer a chance at seeing what the *(&& happened.
                Trace.WriteLine(ex.ToString());
            }
#else
            catch (Exception)
            {
                //Okay - this is no beuno.
                response = new ServerExceptionResponse(request.ServerAddress,
                    (byte)(Modbus.ExceptionOffset + request.FunctionCode),
                    ServerExceptionCodes.ServerDeviceFailure);
            }
#endif


            return response;
        }
    }
}