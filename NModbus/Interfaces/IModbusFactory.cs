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

        #region Master

        /// <summary>
        /// Create an rtu master.
        /// </summary>
        /// <param name="transport"></param>
        /// <returns></returns>
        IModbusSerialMaster CreateMaster(IModbusSerialTransport transport);

        /// <summary>
        /// Create a TCP master.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        IModbusMaster CreateMaster(UdpClient client);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        IModbusMaster CreateMaster(TcpClient client);

        /// <summary>
        /// Create an IP master from IStreamResource
        /// </summary>
        /// <param name="streamResource">IStreamResource transport</param>
        /// <returns></returns>
        IModbusMaster CreateIpMaster(IStreamResource streamResource);

        #endregion

        #region Slave

        /// <summary>
        /// Creates a Modbus Slave.
        /// </summary>
        /// <param name="unitId">The address of this slave on the Modbus network.</param>
        /// <param name="dataStore">Optionally specify a custom data store for the created slave.</param>
        /// <returns></returns>
        IModbusSlave CreateSlave(byte unitId, ISlaveDataStore dataStore = null);

        #endregion

        #region Slave Networks

        /// <summary>
        /// Creates a slave network based on the RTU transport.
        /// </summary>
        /// <param name="transport"></param>
        /// <returns></returns>
        IModbusSlaveNetwork CreateSlaveNetwork(IModbusRtuTransport transport);

        /// <summary>
        /// Creates an ascii slave network.
        /// </summary>
        /// <param name="transport">The ascii transport to base this on.</param>
        /// <returns></returns>
        IModbusSlaveNetwork CreateSlaveNetwork(IModbusAsciiTransport transport);

        /// <summary>
        /// Create a slave network based on TCP.
        /// </summary>
        /// <param name="tcpListener"></param>
        /// <returns></returns>
        IModbusSlaveNetwork CreateSlaveNetwork(TcpListener tcpListener);

        /// <summary>
        /// Creates a UDP modbus slave network.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        IModbusSlaveNetwork CreateSlaveNetwork(UdpClient client);

        #endregion

        #region Transport

        /// <summary>
        /// Creates an RTU transpoort. 
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