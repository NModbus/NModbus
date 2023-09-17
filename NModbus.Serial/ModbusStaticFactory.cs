using NModbus.Data;
using NModbus.Device;
using NModbus.IO;
using NModbus.Logging;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NModbus.Serial
{
    /// <summary>
    /// 静态工厂
    /// </summary>
    public static partial class ModbusStaticFactory
    {
        /// <summary>
        /// 创建ModbusRtu主站，
        /// <para>Create Modbus Rtu Master</para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        /// <returns></returns>
        public static ModbusSerialMaster CreateModbusRtuMaster(SerialPort client, IModbusLogger logger = null)
        {
            var adapter = new SerialPortAdapter(client);
            var factory = new ModbusFactory() { Logger = logger ?? NullModbusLogger.Instance };
            var transport = new ModbusRtuTransport(adapter, factory, factory.Logger);
            return new ModbusSerialMaster(transport); 
        }

        /// <summary>
        /// 创建ModbusAscii主站，
        /// <para>Create Modbus Ascii Master</para>
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static ModbusSerialMaster CreateModbusAsciiMaster(SerialPort client, IModbusLogger logger = null)
        {
            var adapter = new SerialPortAdapter(client);
            var factory = new ModbusFactory() { Logger = logger ?? NullModbusLogger.Instance };
            var transport = new ModbusAsciiTransport(adapter, factory, factory.Logger);
            return new ModbusSerialMaster(transport);
        }

        /// <summary>
        /// 创建ModbusRtu从站网络
        /// <para>Create Modbus Rtu SlaveNetwork</para>
        /// </summary>
        /// <param name="server"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static ModbusSerialSlaveNetwork CreateModbusRtuSlaveNetwork(SerialPort server, IModbusLogger logger = null)
        {
            var adapter = new SerialPortAdapter(server);
            var factory = new ModbusFactory() { Logger = logger ?? NullModbusLogger.Instance };
            var transport = new ModbusRtuTransport(adapter, factory, factory.Logger);
            return new ModbusSerialSlaveNetwork(transport, factory, factory.Logger);
        }

        /// <summary>
        /// 创建ModbusAscii从站网络
        /// <para>Create Modbus Rtu SlaveNetwork</para>
        /// </summary>
        /// <param name="server"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static ModbusSerialSlaveNetwork CreateModbusAsciiSlaveNetwork(SerialPort server, IModbusLogger logger = null)
        {
            var adapter = new SerialPortAdapter(server);
            var factory = new ModbusFactory() { Logger = logger ?? NullModbusLogger.Instance };
            var transport = new ModbusAsciiTransport(adapter, factory, factory.Logger);
            return new ModbusSerialSlaveNetwork(transport, factory, factory.Logger);
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
