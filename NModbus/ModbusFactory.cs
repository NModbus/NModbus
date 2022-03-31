using System;
using System.Collections.Generic;
using NModbus.Device.MessageHandlers;
using System.Linq;
using System.Net.Sockets;
using NModbus.Extensions;
using NModbus.Data;
using NModbus.Device;
using NModbus.IO;
using NModbus.Logging;

namespace NModbus
{
    public class ModbusFactory : IModbusFactory
    {
        /// <summary>
        /// The "built-in" message handlers.
        /// </summary>
        private static readonly IModbusFunctionService[] BuiltInFunctionServices = 
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
            new WriteFileRecordService(),
            new ReadWriteMultipleRegistersService(),
        };

        private readonly IDictionary<byte, IModbusFunctionService> _functionServices;

        /// <summary>
        /// Create a factory which uses the built in standard server function handlers.
        /// </summary>
        public ModbusFactory()
        {
            _functionServices = BuiltInFunctionServices.ToDictionary(s => s.FunctionCode, s => s);

            Logger = NullModbusLogger.Instance;
        }

        /// <summary>
        /// Create a factory which optionally uses the built in function services and allows custom services to be added.
        /// </summary>
        /// <param name="functionServices">User provided function services.</param>
        /// <param name="includeBuiltIn">If true, the built in function services are included. Otherwise, all function services will come from the functionService parameter.</param>
        /// <param name="logger">Logger</param>
        public ModbusFactory(
            IEnumerable<IModbusFunctionService> functionServices = null, 
            bool includeBuiltIn = true, 
            IModbusLogger logger = null)
        {
            Logger = logger ?? NullModbusLogger.Instance;

            //Determine if we're including the built in services
            if (includeBuiltIn)
            {
                //Make a dictionary out of the built in services
                _functionServices = BuiltInFunctionServices
                    .ToDictionary(s => s.FunctionCode, s => s);
            }
            else
            {
                //Create an empty dictionary
                _functionServices = new Dictionary<byte, IModbusFunctionService>();
            }

            if (functionServices != null)
            {
                //Add and replace the provided function services as necessary.
                foreach (IModbusFunctionService service in functionServices)
                {
                    //This will add or replace the service.
                    _functionServices[service.FunctionCode] = service;
                }
            }
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateServer instead.")]
        public IModbusSlave CreateSlave(byte unitId, ISlaveDataStore dataStore = null) { return (IModbusSlave) CreateServer(unitId, dataStore); }
        public IModbusServer CreateServer(byte unitId, IServerDataStore dataStore = null)
        {
            if (dataStore == null)
                dataStore = new DefaultServerDataStore();

            return new ModbusServer(unitId, dataStore, GetAllFunctionServices());
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateServerNetwork instead.")]
        public IModbusSlaveNetwork CreateSlaveNetwork(IModbusRtuTransport transport) { return (IModbusSlaveNetwork) CreateServerNetwork(transport); }
        public IModbusServerNetwork CreateServerNetwork(IModbusRtuTransport transport)
        {
            return new ModbusSerialServerNetwork(transport, this, Logger);
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateServerNetwork instead.")]
        public IModbusSlaveNetwork CreateSlaveNetwork(IModbusAsciiTransport transport) { return (IModbusSlaveNetwork) CreateServerNetwork(transport); }
        public IModbusServerNetwork CreateServerNetwork(IModbusAsciiTransport transport)
        {
            return new ModbusSerialServerNetwork(transport, this, Logger);
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateServerNetwork instead.")]
        public IModbusSlaveNetwork CreateSlaveNetwork(TcpListener tcpListener) { return (IModbusSlaveNetwork) CreateServerNetwork(tcpListener); }
        public IModbusServerNetwork CreateServerNetwork(TcpListener tcpListener)
        {
            return new ModbusTcpServerNetwork(tcpListener, this, Logger);
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateServerNetwork instead.")]
        public IModbusSlaveNetwork CreateSlaveNetwork(UdpClient client) { return (IModbusSlaveNetwork)CreateServerNetwork(client); }
        public IModbusServerNetwork CreateServerNetwork(UdpClient client)
        {
            return new ModbusUdpServerNetwork(client, this, Logger);
        }

        public IModbusRtuTransport CreateRtuTransport(IStreamResource streamResource)
        {
            return new ModbusRtuTransport(streamResource, this, Logger);
        }

        public IModbusAsciiTransport CreateAsciiTransport(IStreamResource streamResource)
        {
            return new ModbusAsciiTransport(streamResource, this, Logger);
        }

        public IModbusLogger Logger { get; }

        public IModbusFunctionService[] GetAllFunctionServices()
        {
            return _functionServices
                .Values
                .ToArray();
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateClient instead.")]
        public IModbusSerialMaster CreateMaster(IModbusSerialTransport transport) { return (IModbusSerialMaster) CreateClient(transport); }
        public IModbusSerialClient CreateClient(IModbusSerialTransport transport)
        {
            return new ModbusSerialClient(transport);
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateClient instead.")]
        public IModbusMaster CreateMaster(UdpClient client) { return (IModbusMaster) CreateClient(client); }
        public IModbusClient CreateClient(UdpClient client)
        {
            var adapter = new UdpClientAdapter(client);

            var transport = new ModbusIpTransport(adapter, this, Logger);

            return new ModbusIpClient(transport);
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateClient instead.")]
        public IModbusMaster CreateMaster(TcpClient client) { return (IModbusMaster) CreateClient(client); }
        public IModbusClient CreateClient(TcpClient client)
        {
            var adapter = new TcpClientAdapter(client);

            var transport = new ModbusIpTransport(adapter, this, Logger);

            return new ModbusIpClient(transport);
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateClient instead.")]
        public IModbusMaster CreateMaster(Socket client) { return (IModbusMaster) CreateClient(client); }
        public IModbusClient CreateClient(Socket client)
        {
            var adapter = new SocketAdapter(client);

            var transport = new ModbusRtuTransport(adapter, this, Logger);

            return new ModbusSerialClient(transport);
        }

        public IModbusFunctionService GetFunctionService(byte functionCode)
        {
            return _functionServices.GetValueOrDefault(functionCode);
        }
    }
}