using System.IO.Ports;

namespace NModbus.Serial
{
    public static class ModbusFactoryExtensions
    {
        public static IModbusSerialMaster CreateRtuMaster(this IModbusFactory factory, SerialPort serialPort)
        {
            var adapter = new SerialPortAdapter(serialPort);
            return factory.CreateRtuMaster(adapter);
        }

        public static IModbusRtuTransport CreateRtuTransport(this IModbusFactory factory, SerialPort serialPort)
        {
            var adapter = new SerialPortAdapter(serialPort);
            return factory.CreateRtuTransport(adapter);
        }

        public static IModbusSlaveNetwork CreateRtuSlaveNetwork(this IModbusFactory factory, SerialPort serialPort)
        {
            var adapter = new SerialPortAdapter(serialPort);
            return factory.CreateRtuSlaveNetwork(adapter);
        }

        public static IModbusSerialMaster CreateAsciiMaster(this IModbusFactory factory, SerialPort serialPort)
        {
            var adapter = new SerialPortAdapter(serialPort);
            return factory.CreateAsciiMaster(adapter);
        }

        public static IModbusAsciiTransport CreateAsciiTransport(this IModbusFactory factory, SerialPort serialPort)
        {
            var adapter = new SerialPortAdapter(serialPort);
            return factory.CreateAsciiTransport(adapter);
        }

        public static IModbusSlaveNetwork CreateAsciiSlaveNetwork(this IModbusFactory factory, SerialPort serialPort)
        {
            var adapter = new SerialPortAdapter(serialPort);
            return factory.CreateAsciiSlaveNetwork(adapter);
        }
    }
}
