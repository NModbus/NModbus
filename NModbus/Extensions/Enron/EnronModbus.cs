using System;
using System.Collections.Generic;
using System.Linq;
using NModbus.Utility;

namespace NModbus.Extensions.Enron
{
    /// <summary>
    ///     Utility extensions for the Enron Modbus dialect.
    /// </summary>
    public static class EnronModbus
    {
        /// <summary>
        ///     Read contiguous block of 32 bit holding registers.
        /// </summary>
        /// <param name="client">The Modbus client.</param>
        /// <param name="serverAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>Holding registers status</returns>
        public static uint[] ReadHoldingRegisters32(
            this IModbusClient client,
            byte serverAddress,
            ushort startAddress,
            ushort numberOfPoints)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            ValidateNumberOfPoints(numberOfPoints, 62);

            // read 16 bit chunks and perform conversion
            var rawRegisters = client.ReadHoldingRegisters(
                serverAddress,
                startAddress,
                (ushort)(numberOfPoints * 2));

            return Convert(rawRegisters).ToArray();
        }

        /// <summary>
        ///     Read contiguous block of 32 bit input registers.
        /// </summary>
        /// <param name="client">The Modbus client.</param>
        /// <param name="serverAddress">Address of device to read values from.</param>
        /// <param name="startAddress">Address to begin reading.</param>
        /// <param name="numberOfPoints">Number of holding registers to read.</param>
        /// <returns>Input registers status</returns>
        public static uint[] ReadInputRegisters32(
            this IModbusClient client,
            byte serverAddress,
            ushort startAddress,
            ushort numberOfPoints)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            ValidateNumberOfPoints(numberOfPoints, 62);

            var rawRegisters = client.ReadInputRegisters(
                serverAddress,
                startAddress,
                (ushort)(numberOfPoints * 2));

            return Convert(rawRegisters).ToArray();
        }

        /// <summary>
        ///     Write a single 16 bit holding register.
        /// </summary>
        /// <param name="client">The Modbus client.</param>
        /// <param name="serverAddress">Address of the device to write to.</param>
        /// <param name="registerAddress">Address to write.</param>
        /// <param name="value">Value to write.</param>
        public static void WriteSingleRegister32(
            this IModbusClient client,
            byte serverAddress,
            ushort registerAddress,
            uint value)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            client.WriteMultipleRegisters32(serverAddress, registerAddress, new[] { value });
        }

        /// <summary>
        ///     Write a block of contiguous 32 bit holding registers.
        /// </summary>
        /// <param name="client">The Modbus client.</param>
        /// <param name="serverAddress">Address of the device to write to.</param>
        /// <param name="startAddress">Address to begin writing values.</param>
        /// <param name="data">Values to write.</param>
        public static void WriteMultipleRegisters32(
            this IModbusClient client,
            byte serverAddress,
            ushort startAddress,
            uint[] data)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length == 0 || data.Length > 61)
            {
                throw new ArgumentException("The length of argument data must be between 1 and 61 inclusive.");
            }

            client.WriteMultipleRegisters(serverAddress, startAddress, Convert(data).ToArray());
        }

        /// <summary>
        ///     Convert the 32 bit registers to two 16 bit values.
        /// </summary>
        private static IEnumerable<ushort> Convert(uint[] registers)
        {
            foreach (var register in registers)
            {
                // low order value
                yield return BitConverter.ToUInt16(BitConverter.GetBytes(register), 0);

                // high order value
                yield return BitConverter.ToUInt16(BitConverter.GetBytes(register), 2);
            }
        }

        /// <summary>
        ///     Convert the 16 bit registers to 32 bit registers.
        /// </summary>
        private static IEnumerable<uint> Convert(ushort[] registers)
        {
            for (int i = 0; i < registers.Length; i++)
            {
                yield return ModbusUtility.GetUInt32(registers[i + 1], registers[i]);
                i++;
            }
        }

        private static void ValidateNumberOfPoints(ushort numberOfPoints, ushort maxNumberOfPoints)
        {
            if (numberOfPoints < 1 || numberOfPoints > maxNumberOfPoints)
            {
                string msg = $"Argument numberOfPoints must be between 1 and {maxNumberOfPoints} inclusive.";
                throw new ArgumentException(msg);
            }
        }
    }
}
