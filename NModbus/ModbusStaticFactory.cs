using NModbus.Data;
using NModbus.Device;
using NModbus.IO;
using NModbus.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NModbus
{
    /// <summary>
    /// 静态工厂
    /// </summary>
    public static partial class ModbusStaticFactory
    {
        /// <summary>
        /// 创建ModbusTcp主站，
        /// <para>Create Modbus TCP Master</para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static ModbusIpMaster CreateModbusTcpMaster(TcpClient client, IModbusLogger logger = null)
        {
            var adapter = new TcpClientAdapter(client);
            var factory = new ModbusFactory() { Logger = logger ?? NullModbusLogger.Instance };
            var transport = new ModbusIpTransport(adapter, factory, factory.Logger);
            return new ModbusIpMaster(transport);
        }

        /// <summary>
        /// 创建ModbusUdp主站，
        /// <para>Create Modbus UDP Master</para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="endpoint"></param>
        /// <param name="logger"></param>
        public static ModbusIpMaster CreateModbusUdpMaster(UdpClient client, EndPoint endpoint, IModbusLogger logger = null)
        {
            var adapter = new UdpClientAdapter(client,endpoint);
            var factory = new ModbusFactory() { Logger = logger ?? NullModbusLogger.Instance };
            var transport = new ModbusIpTransport(adapter, factory, factory.Logger);
            return new ModbusIpMaster(transport);
        }


        /// <summary>
        /// 创建ModbusRtuOverTcp主站
        /// <para>Create Modbus RtuOverTcp Master</para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public static ModbusSerialMaster CreateModbusRtuOverTcpMaster(TcpClient client, IModbusLogger logger = null)
        {
            var adapter = new TcpClientAdapter(client);
            var factory = new ModbusFactory() { Logger = logger ?? NullModbusLogger.Instance };
            var transport = new ModbusRtuTransport(adapter, factory, factory.Logger);
            return new ModbusSerialMaster(transport);
        }

        /// <summary>
        /// 创建ModbusRtuOverUdp主站
        /// <para>Create Modbus RtuOverUdp Master</para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="endpoint"></param>
        /// <param name="logger"></param>
        public static ModbusSerialMaster CreateModbusRtuOverUdpMaster(UdpClient client, EndPoint endpoint, IModbusLogger logger = null)
        {
            var adapter = new UdpClientAdapter(client,endpoint);
            var factory = new ModbusFactory() { Logger = logger ?? NullModbusLogger.Instance };
            var transport = new ModbusRtuTransport(adapter, factory, factory.Logger);
            return new ModbusSerialMaster(transport);
        }


        /// <summary>
        /// 创建ModbusAsciiOverTcp主站
        /// <para>Create Modbus AsciiOverTcp Master</para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public static ModbusSerialMaster CreateModbusAsciiOverTcpMaster(TcpClient client, IModbusLogger logger = null)
        {
            var adapter = new TcpClientAdapter(client);
            var factory = new ModbusFactory() { Logger = logger ?? NullModbusLogger.Instance };
            var transport = new ModbusAsciiTransport(adapter, factory, factory.Logger);
            return new ModbusSerialMaster(transport);
        }

        /// <summary>
        /// 创建ModbusAsciiOverUdp主站
        /// <para>Create Modbus AsciiOverUdp Master</para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="endpoint"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static ModbusSerialMaster CreateModbusAsciiOverUdpMaster(UdpClient client, EndPoint endpoint, IModbusLogger logger = null)
        {
            var adapter = new UdpClientAdapter(client,endpoint);
            var factory = new ModbusFactory() { Logger = logger ?? NullModbusLogger.Instance };
            var transport = new ModbusAsciiTransport(adapter, factory, factory.Logger);
            return new ModbusSerialMaster(transport);
        }

        /// <summary>
        /// 创建ModbusTcp从站网络
        /// <para>Create Modbus TCP SlaveNetwork</para>
        /// </summary>
        /// <param name="server"></param>
        /// <param name="logger"></param>
        public static ModbusTcpSlaveNetwork CreateModbusTcpSlaveNetwork(TcpListener server, IModbusLogger logger = null)
        {
            var factory = new ModbusFactory() { Logger = logger ?? NullModbusLogger.Instance };
            return new ModbusTcpSlaveNetwork(server, factory, factory.Logger);
        }

        /// <summary>
        /// 创建ModbusUdp从站网络
        /// <para>Create Modbus UDP SlaveNetwork</para>
        /// </summary>
        /// <param name="server"></param>
        /// <param name="logger"></param>
        public static ModbusUdpSlaveNetwork CreateModbusUdpSlaveNetwork(UdpClient server, IModbusLogger logger = null)
        {
            var factory = new ModbusFactory() { Logger = logger ?? NullModbusLogger.Instance };
            return new ModbusUdpSlaveNetwork(server, factory, factory.Logger);
        }


        /// <summary>
        /// 创建ModbusRtuOverTcp从站网络
        /// <para>Create Modbus RtuOverTcp SlaveNetwork</para>
        /// </summary>
        /// <param name="server"></param>
        /// <param name="logger"></param>
        public static ModbusRtuOverTcpSlaveNetwork CreateModbusRtuOverTcpSlaveNetwork(TcpListener server, IModbusLogger logger = null)
        {
            var factory = new ModbusFactory() { Logger = logger ?? NullModbusLogger.Instance };
            return new ModbusRtuOverTcpSlaveNetwork(server, factory, factory.Logger);
        }

        /// <summary>
        /// 创建ModbusRtuOverUdp从站网络
        /// <para>Create Modbus RtuOverUdp SlaveNetwork</para>
        /// </summary>
        /// <param name="server"></param>
        /// <param name="logger"></param>
        public static ModbusRtuOverUdpSlaveNetwork CreateModbusRtuOverUdpSlaveNetwork(UdpClient server, IModbusLogger logger = null)
        {
            var factory = new ModbusFactory() { Logger = logger ?? NullModbusLogger.Instance };
            return new ModbusRtuOverUdpSlaveNetwork(server, factory, factory.Logger);
        }


        /// <summary>
        /// 创建ModbusAsciiOverTcp从站网络
        /// <para>Create Modbus AsciiOverTcp SlaveNetwork</para>
        /// </summary>
        /// <param name="server"></param>
        /// <param name="logger"></param>
        public static ModbusAsciiOverTcpSlaveNetwork CreateModbusAsciiOverTcpSlaveNetwork(TcpListener server, IModbusLogger logger = null)
        {
            var factory = new ModbusFactory() { Logger = logger ?? NullModbusLogger.Instance };
            return new ModbusAsciiOverTcpSlaveNetwork(server, factory, factory.Logger);
        }

        /// <summary>
        /// 创建ModbusAsciiOverUdp从站网络
        /// <para>Create Modbus AsciiOverUdp SlaveNetwork</para>
        /// </summary>
        /// <param name="server"></param>
        /// <param name="logger"></param>
        public static ModbusAsciiOverUdpSlaveNetwork CreateModbusAsciiOverUdpSlaveNetwork(UdpClient server, IModbusLogger logger = null)
        {
            var factory = new ModbusFactory() { Logger = logger ?? NullModbusLogger.Instance };
            return new ModbusAsciiOverUdpSlaveNetwork(server, factory, factory.Logger);
        }

        /// <summary>
        /// 创建Modbus从站
        /// </summary>
        /// <param name="unitId"></param>
        /// <param name="dataStore"></param>
        /// <returns></returns>
        public static IModbusSlave CreateSlave(byte unitId, ISlaveDataStore dataStore = null)
        {
            if (dataStore == null)
                dataStore = new DefaultSlaveDataStore();

            return new ModbusSlave(unitId, dataStore, ModbusFactory.BuiltInFunctionServices);
        }

    }
}
