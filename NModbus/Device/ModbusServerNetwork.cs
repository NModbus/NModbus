using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NModbus.Extensions;
using NModbus.Logging;

namespace NModbus.Device
{

    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ModbusServerNetwork instead.")]
    internal abstract class ModbusSlaveNetwork : ModbusServerNetwork, IModbusSlaveNetwork
    {
        protected ModbusSlaveNetwork(IModbusTransport transport, IModbusFactory modbusFactory, IModbusLogger logger) : base(transport, modbusFactory, logger) { }
    }

    internal abstract class ModbusServerNetwork : ModbusDevice, IModbusServerNetwork
    {
        private readonly IModbusLogger _logger;
        private readonly IDictionary<byte, IModbusServer> _servers = new ConcurrentDictionary<byte, IModbusServer>();

        protected ModbusServerNetwork(IModbusTransport transport, IModbusFactory modbusFactory, IModbusLogger logger) 
            : base(transport)
        {
            ModbusFactory = modbusFactory;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected IModbusFactory ModbusFactory { get; }

        protected IModbusLogger Logger
        {
            get { return _logger; }
        }

        /// <summary>
        /// Start server listening for requests.
        /// </summary>
        public abstract Task ListenAsync(CancellationToken cancellationToken = new CancellationToken());

        /// <summary>
        /// Apply the request.
        /// </summary>
        /// <param name="request"></param>
        protected IModbusMessage ApplyRequest(IModbusMessage request)
        {
            //Check for broadcast requests
            if (request.ServerAddress == 0)
            {
                //Grab each server
                foreach (var server in _servers.Values)
                {
                    try
                    {
                        //Apply the request
                        server.ApplyRequest(request);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Error applying request to server {server.UnitId}: {ex.Message}");
                    }
                }
            }
            else
            {
                //Attempt to find a server for this address
                IModbusServer server = GetServer(request.ServerAddress);

                // only service requests addressed to our servers
                if (server == null)
                {
                    Console.WriteLine($"NModbus Server Network ignoring request intended for NModbus Server {request.ServerAddress}");
                }
                else
                {
                    // perform action
                    return server.ApplyRequest(request);
                }
            }

            return null;
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use AddServer instead.")]
        public void AddSlave(IModbusSlave slave) { AddServer(slave); }
        public void AddServer(IModbusServer server)
        {
            if (server == null) throw new ArgumentNullException(nameof(server));

            _servers.Add(server.UnitId, server);

            Logger.Information($"Server {server.UnitId} added to server network.");
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use RemoveServer instead.")]
        public void RemoveSlave(byte unitId) { RemoveServer(unitId); }
        public void RemoveServer(byte unitId)
        {
            _servers.Remove(unitId);

            Logger.Information($"Server {unitId} removed from server network.");
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use GetServer instead.")]
        public IModbusSlave GetSlave(byte unitId) { return (IModbusSlave) GetServer(unitId); }
        public IModbusServer GetServer(byte unitId)
        {
            return _servers.GetValueOrDefault(unitId);
        }
    }
}