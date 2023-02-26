using System;
using System.Collections.Generic;
using System.Linq;

namespace NModbus.Extensions.Enron
{
	/// <summary>
	///     Utility extensions for the Enron Modbus dialect.
	/// </summary>
	public static class EnronModbus
	{
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
			if (master == null)
			{
				throw new ArgumentNullException(nameof(master));
			}

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
			if (master == null)
			{
				throw new ArgumentNullException(nameof(master));
			}

			if (data == null)
			{
				throw new ArgumentNullException(nameof(data));
			}

			if (data.Length == 0 || data.Length > 61)
			{
				throw new ArgumentException("The length of argument data must be between 1 and 61 inclusive.");
			}

			master.WriteMultipleRegisters(slaveAddress, startAddress, Convert(data).ToArray());
		}

		/// <summary>
		///     Convert the 32 bit registers to two 16 bit values.
		/// </summary>
		private static IEnumerable<ushort> Convert(uint[] registers)
		{
			foreach (var register in registers)
			{
				// low order value
				yield return BitConverter.ToUInt16(BitConverter.GetBytes(register), 2);

				// high order value
				yield return BitConverter.ToUInt16(BitConverter.GetBytes(register), 0);
			}
		}
	}
}
