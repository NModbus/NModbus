using System;
using System.Collections.Generic;
using NModbus.Device.MessageHandlers;
using NModbus.Interfaces;
using System.Linq;
using System.Net.Sockets;
using NModbus.Extensions;
using NModbus.Data;
using NModbus.Device;
using NModbus.IO;

namespace NModbus
{
    public class ModbusFactory : IModbusFactory
    {
        /// <summary>
        /// The "built-in" message handlers.
        /// </summary>
        private static readonly IModbusFunctionService[] BuiltInFunctionServices = new IModbusFunctionService[]
        {
            new ReadCoilsService(),
            new ReadInputsService(),
            new ReadHoldingRegistersService(),
            new ReadInputRegistersService(),
            new DiagnosticsService(),
            new WriteSingleCoilService(),
            new WriteSingleRegisterService(),
            new WriteMultipleCoilsService(),
            new WriteMultipleRegistersService(),
            new ReadWriteMultipleRegistersService(),
        };

        private readonly IDictionary<byte, IModbusFunctionService> _functionServices;

        public ModbusFactory()
        {
            _functionServices = BuiltInFunctionServices.ToDictionary(s => s.FunctionCode, s => s);
        }

        public IModbusSlave CreateSlave(byte unitId, ISlaveDataStore dataStore = null)
        {
            if (dataStore == null)
                dataStore = new DefaultSlaveDataStore();

            return new NetworkedSlave(unitId, dataStore, GetAllFunctionServices());
        }

        public IModbusSlaveNetwork CreateSlaveNetwork(IModbusRtuTransport transport)
        {
            return new ModbusSerialSlaveNetwork(transport);
        }

        public IModbusSlaveNetwork CreateSlaveNetwork(IModbusAsciiTransport transport)
        {
            return new ModbusSerialSlaveNetwork(transport);
        }

        public IModbusSlaveNetwork CreateSlaveNetwork(TcpListener tcpListener)
        {
            return new ModbusTcpSlaveNetwork(tcpListener);
        }

        public IModbusSlaveNetwork CreateSlaveNetwork(UdpClient client)
        {
            return new ModbusUdpSlaveNetwork(client);
        }

        public IModbusRtuTransport CreateRtuTransport(IStreamResource streamResource)
        {
            return new ModbusRtuTransport(streamResource, this);
        }

        public IModbusAsciiTransport CreateAsciiTransport(IStreamResource streamResource)
        {
            return new ModbusAsciiTransport(streamResource);
        }

        public IModbusIpTransport CreateIpTransport(IStreamResource streamResource)
        {
            throw new NotImplementedException();
        }

        public IModbusFunctionService[] GetAllFunctionServices()
        {
            return _functionServices
                .Values
                .ToArray();
        }

        public IModbusSerialMaster CreateMaster(IModbusRtuTransport transport)
        {
            throw new NotImplementedException();
        }

        public IModbusSerialMaster CreateMaster(IModbusAsciiTransport transport)
        {
            throw new NotImplementedException();
        }

        public IModbusMaster CreateMaster(UdpClient client)
        {
            var adapter = new UdpClientAdapter(client);

            var transport = new ModbusIpTransport(adapter);

            return new ModbusIpMaster(transport);
        }

        public IModbusMaster CreateMaster(TcpClient client)
        {
            var adapter = new TcpClientAdapter(client);

            var transport = new ModbusIpTransport(adapter);

            return new ModbusIpMaster(transport);
        }

        public IModbusFunctionService GetFunctionService(byte functionCode)
        {
            return _functionServices.GetValueOrDefault(functionCode);
        }

        

        
    }
}