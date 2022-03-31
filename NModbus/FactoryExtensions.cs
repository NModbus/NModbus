using System;
using NModbus.Device;
using NModbus.IO;

namespace NModbus
{
    /// <summary>
    /// Extension methods for the IModbusFactory interface.
    /// </summary>
    public static class FactoryExtensions
    {

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateRtuClient instead.")]
        public static IModbusSerialMaster CreateRtuMaster(this IModbusFactory factory, IStreamResource streamResource)
        {
            return (IModbusSerialMaster) CreateRtuClient(factory, streamResource);
        }

        /// <summary>
        /// Creates an RTU client.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="streamResource"></param>
        /// <returns></returns>
        public static IModbusSerialClient CreateRtuClient(this IModbusFactory factory, IStreamResource streamResource)
        {
            IModbusRtuTransport transport = factory.CreateRtuTransport(streamResource);

            return new ModbusSerialClient(transport);
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use IModbusSerialClient instead.")]
        public static IModbusSerialMaster CreateAsciiMaster(this IModbusFactory factory, IStreamResource streamResource)
        {
            return (IModbusSerialMaster) CreateAsciiClient(factory, streamResource);
        }

        /// <summary>
        /// Creates an ASCII client.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="streamResource"></param>
        /// <returns></returns>
        public static IModbusSerialClient CreateAsciiClient(this IModbusFactory factory, IStreamResource streamResource)
        {
            IModbusAsciiTransport transport = factory.CreateAsciiTransport(streamResource);

            return new ModbusSerialClient(transport);
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateRtuServerNetwork instead.")]
        public static IModbusSlaveNetwork CreateRtuSlaveNetwork(this IModbusFactory factory, IStreamResource streamResource)
        {
            return (IModbusSlaveNetwork) CreateRtuServerNetwork(factory, streamResource);
        }

        /// <summary>
        /// Creates an RTU server network.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="streamResource"></param>
        /// <returns></returns>
        public static IModbusServerNetwork CreateRtuServerNetwork(this IModbusFactory factory, IStreamResource streamResource)
        {
            IModbusRtuTransport transport = factory.CreateRtuTransport(streamResource);

            return factory.CreateServerNetwork(transport);
        }

        [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use CreateAsciiServerNetwork instead.")]
        public static IModbusSlaveNetwork CreateAsciiSlaveNetwork(this IModbusFactory factory, IStreamResource streamResource)
        {
            return (IModbusSlaveNetwork) CreateAsciiServerNetwork(factory, streamResource);
        }

        /// <summary>
        /// Creates an ASCII server network.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="streamResource"></param>
        /// <returns></returns>
        public static IModbusServerNetwork CreateAsciiServerNetwork(this IModbusFactory factory, IStreamResource streamResource)
        {
            IModbusAsciiTransport transport = factory.CreateAsciiTransport(streamResource);

            return factory.CreateServerNetwork(transport);
        }
           
    }
}