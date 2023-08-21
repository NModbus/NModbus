using System;
using System.Linq;
using System.Threading.Tasks;

namespace NModbus.Extensions.Enron
{
    /// <summary>
    ///     Utility extensions for the Enron Modbus dialect.
    /// </summary>
    public static class EnronModbus
	{
		/// <summary>
		///    Reads contiguous block of input registers with 32 bit register size.
		/// </summary>
		/// <param name="master">The Modbus master.</param>
		/// <param name="slaveAddress">Address of device to read values from.</param>
		/// <param name="startAddress">Address to begin reading.</param>
		/// <param name="numberOfPoints">Number of holding registers to read.</param>
		/// <returns>Input registers status.</returns>
		public static uint[] ReadInputRegisters32(this IModbusMaster master, byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
			var registers = master.ReadInputRegisters(slaveAddress, startAddress, (ushort)(numberOfPoints * 2));

			return ConvertTo32(registers);
		}

		/// <summary>
		///    Reads contiguous block of holding registers.
		/// </summary>
		/// <param name="master">The Modbus master.</param>
		/// <param name="slaveAddress">Address of device to read values from.</param>
		/// <param name="startAddress">Address to begin reading.</param>
		/// <param name="numberOfPoints">Number of holding registers to read.</param>
		/// <returns>Holding registers status.</returns>
		public static uint[] ReadHoldingRegisters32(this IModbusMaster master, byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
            var registers = master.ReadHoldingRegisters(slaveAddress, startAddress, (ushort)(numberOfPoints * 2));

            return ConvertTo32(registers);
        }

		/// <summary>
		///    Asynchronously reads contiguous block of input registers with 32 bit register size.
		/// </summary>
		/// <param name="master">The Modbus master.</param>
		/// <param name="slaveAddress">Address of device to read values from.</param>
		/// <param name="startAddress">Address to begin reading.</param>
		/// <param name="numberOfPoints">Number of holding registers to read.</param>
		/// <returns>A task that represents the asynchronous read operation.</returns>
		public static async Task<uint[]> ReadInputRegisters32Async(this IModbusMaster master, byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
            var registers = await master.ReadInputRegistersAsync(slaveAddress, startAddress, (ushort)(numberOfPoints * 2));

            return ConvertTo32(registers);
		}

		/// <summary>
		///    Asynchronously reads contiguous block of holding registers.
		/// </summary>
		/// <param name="master">The Modbus master.</param>
		/// <param name="slaveAddress">Address of device to read values from.</param>
		/// <param name="startAddress">Address to begin reading.</param>
		/// <param name="numberOfPoints">Number of holding registers to read.</param>
		/// <returns>A task that represents the asynchronous read operation.</returns>
		public static async Task<uint[]> ReadHoldingRegisters32Async(this IModbusMaster master, byte slaveAddress, ushort startAddress, ushort numberOfPoints)
		{
            var registers = await master.ReadHoldingRegistersAsync(slaveAddress, startAddress, (ushort)(numberOfPoints * 2));

            return ConvertTo32(registers);
        }

		/// <summary>
		///     Write a single 16 bit holding register.
		/// </summary>
		/// <param name="master">The Modbus master.</param>
		/// <param name="slaveAddress">Address of the device to write to.</param>
		/// <param name="registerAddress">Address to write.</param>
		/// <param name="value">Value to write.</param>
		public static void WriteSingleRegister32(
			this IModbusMaster master,
			byte slaveAddress,
			ushort registerAddress,
			uint value)
		{
			if (master == null) throw new ArgumentNullException(nameof(master));

			master.WriteMultipleRegisters32(slaveAddress, registerAddress, new[] { value });
		}

		/// <summary>
		///     Write a block of contiguous 32 bit holding registers.
		/// </summary>
		/// <param name="master">The Modbus master.</param>
		/// <param name="slaveAddress">Address of the device to write to.</param>
		/// <param name="startAddress">Address to begin writing values.</param>
		/// <param name="data">Values to write.</param>
		public static void WriteMultipleRegisters32(
			this IModbusMaster master,
			byte slaveAddress,
			ushort startAddress,
			uint[] data)
		{
			if (master == null)	throw new ArgumentNullException(nameof(master));
			if (data == null) throw new ArgumentNullException(nameof(data));

			master.WriteMultipleRegisters(slaveAddress, startAddress, ConvertFrom32(data).ToArray());
		}

		/// <summary> Convert the 32 bit registers to two 16 bit values. </summary>
		public static ushort[] ConvertFrom32(uint[] registers)
		{
			var result = new ushort[registers.Length * 2];

			var index = 0;

			foreach (var register in registers)
			{
				var bytes = BitConverter.GetBytes(register);

                // low order value
                result[index++] = BitConverter.ToUInt16(bytes, 2);

				// high order value
				result[index++] = BitConverter.ToUInt16(bytes, 0);
			}

			return result;
		}

        /// <summary> Convert the double 16 bit registers to single 32 bit values. </summary>
        public static uint[] ConvertTo32(ushort[] registers)
		{
			if (registers.Length % 2 != 0)
				throw new ArgumentException("registers must have an even number of elements.", nameof(registers));

			var numberOfResult = registers.Length / 2;

            var result = new uint[numberOfResult];

			for(var index = 0; index < numberOfResult; index++)
			{
				result[index] = ((uint)registers[index * 2]) << 16 | registers[(index * 2) + 1];
			}

			return result;
		}
	}
}
