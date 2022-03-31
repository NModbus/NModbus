using System;
using System.Net.Sockets;
using NModbus.IO;

namespace NModbus
{
    /// <summary>
    /// Container for modbus function services.
    /// </summary>
    public interface IModbusFactory
    {
        /// <summary>
        /// Get the service for a given function code.
        /// </summary>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        IModbusFunctionService GetFunctionService(byte functionCode);

        /// <summary>
        /// Gets all of the services.
        /// </summary>
        /// <returns></returns>
        IModbusFunctionService[] GetAllFunctionServices();

        #region Client

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateClient instead.")]
        IModbusSerialMaster CreateMaster(IModbusSerialTransport transport);

        /// <summary>
        /// Create an rtu client.
        /// </summary>
        /// <param name="transport"></param>
        /// <returns></returns>
        IModbusSerialClient CreateClient(IModbusSerialTransport transport);

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateClient instead.")]
        IModbusMaster CreateMaster(UdpClient client);

        /// <summary>
        /// Create a TCP client.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        IModbusClient CreateClient(UdpClient client);

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateClient instead.")]
        IModbusMaster CreateMaster(TcpClient client);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        IModbusClient CreateClient(TcpClient client);

        #endregion

        #region Server

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateServer instead.")]
        IModbusSlave CreateSlave(byte unitId, ISlaveDataStore dataStore = null);

        /// <summary>
        /// Creates a Modbus Server.
        /// </summary>
        /// <param name="unitId">The address of this server on the Modbus network.</param>
        /// <param name="dataStore">Optionally specify a custom data store for the created server.</param>
        /// <returns></returns>
        IModbusServer CreateServer(byte unitId, IServerDataStore dataStore = null);

        #endregion

        #region Server Networks

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateServerNetwork instead.")]
        IModbusSlaveNetwork CreateSlaveNetwork(IModbusRtuTransport transport);

        /// <summary>
        /// Creates a server network based on the RTU transport.
        /// </summary>
        /// <param name="transport"></param>
        /// <returns></returns>
        IModbusServerNetwork CreateServerNetwork(IModbusRtuTransport transport);

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateServerNetwork instead.")]
        IModbusSlaveNetwork CreateSlaveNetwork(IModbusAsciiTransport transport);

        /// <summary>
        /// Creates an ascii server network.
        /// </summary>
        /// <param name="transport">The ascii transport to base this on.</param>
        /// <returns></returns>
        IModbusServerNetwork CreateServerNetwork(IModbusAsciiTransport transport);

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateServerNetwork instead.")]
        IModbusSlaveNetwork CreateSlaveNetwork(TcpListener tcpListener);

        /// <summary>
        /// Create a server network based on TCP.
        /// </summary>
        /// <param name="tcpListener"></param>
        /// <returns></returns>
        IModbusServerNetwork CreateServerNetwork(TcpListener tcpListener);


        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateServerNetwork instead.")]
        IModbusSlaveNetwork CreateSlaveNetwork(UdpClient client);
        /// <summary>
        /// Creates a UDP modbus server network.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        IModbusServerNetwork CreateServerNetwork(UdpClient client);


        #endregion

        #region Transport

        /// <summary>
        /// Creates an RTU transport. 
        /// </summary>
        /// <param name="streamResource"></param>
        /// <returns></returns>
        IModbusRtuTransport CreateRtuTransport(IStreamResource streamResource);

        /// <summary>
        /// Creates an Ascii Transport.
        /// </summary>
        /// <param name="streamResource"></param>
        /// <returns></returns>
        IModbusAsciiTransport CreateAsciiTransport(IStreamResource streamResource);

        #endregion

        IModbusLogger Logger { get; }
    }
}