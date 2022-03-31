using System;
using System.IO.Ports;

namespace NModbus.Serial
{
    public static class ModbusFactoryExtensions
    {
        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateRtuClient instead.")]
        public static IModbusSerialMaster CreateRtuMaster(this IModbusFactory factory, SerialPort serialPort)
        {
            return (IModbusSerialMaster) CreateRtuClient(factory, serialPort);
        }

        public static IModbusSerialClient CreateRtuClient(this IModbusFactory factory, SerialPort serialPort)
        {
            var adapter = new SerialPortAdapter(serialPort);
            return factory.CreateRtuClient(adapter);
        }

        public static IModbusRtuTransport CreateRtuTransport(this IModbusFactory factory, SerialPort serialPort)
        {
            var adapter = new SerialPortAdapter(serialPort);
            return factory.CreateRtuTransport(adapter);
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateRtuServerNetwork instead.")]
        public static IModbusSlaveNetwork CreateRtuSlaveNetwork(this IModbusFactory factory, SerialPort serialPort)
        {
            return (IModbusSlaveNetwork) CreateRtuServerNetwork(factory, serialPort);
        }
        public static IModbusServerNetwork CreateRtuServerNetwork(this IModbusFactory factory, SerialPort serialPort)
        {
            var adapter = new SerialPortAdapter(serialPort);
            return factory.CreateRtuServerNetwork(adapter);
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateAsciiClient instead.")]
        public static IModbusSerialMaster CreateAsciiMaster(this IModbusFactory factory, SerialPort serialPort)
        {
            return (IModbusSerialMaster) CreateAsciiClient(factory, serialPort);
        }
        public static IModbusSerialClient CreateAsciiClient(this IModbusFactory factory, SerialPort serialPort)
        {
            var adapter = new SerialPortAdapter(serialPort);
            return factory.CreateAsciiClient(adapter);
        }

        public static IModbusAsciiTransport CreateAsciiTransport(this IModbusFactory factory, SerialPort serialPort)
        {
            var adapter = new SerialPortAdapter(serialPort);
            return factory.CreateAsciiTransport(adapter);
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateAsciiServerNetwork instead.")]
        public static IModbusSlaveNetwork CreateAsciiSlaveNetwork(this IModbusFactory factory, SerialPort serialPort)
        {
            return (IModbusSlaveNetwork)CreateAsciiServerNetwork(factory, serialPort);
        }
        public static IModbusServerNetwork CreateAsciiServerNetwork(this IModbusFactory factory, SerialPort serialPort)
        {
            var adapter = new SerialPortAdapter(serialPort);
            return factory.CreateAsciiServerNetwork(adapter);
        }
    }
}
