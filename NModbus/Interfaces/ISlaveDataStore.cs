namespace NModbus
{
    /// <summary>
    ///  Data store (memory map) for a Modbus slave.
    /// </summary>
    /// <remarks>
    ///     You can control the Modbus exception code returned to the master by throwing
    ///     <see cref="InvalidModbusRequestException"/> from read/write methods, e.g.
    ///     <c>throw new InvalidModbusRequestException(SlaveExceptionCodes.IllegalDataAddress);</c>.
    ///     Any other exception type results in a generic Slave Device Failure (code 4) response.
    /// </remarks>
    public interface ISlaveDataStore
    {
        /// <summary>
        /// Gets the descrete coils.
        /// </summary>
        IPointSource<bool> CoilDiscretes { get; }

        /// <summary>
        /// Gets the discrete inputs.
        /// </summary>
        IPointSource<bool> CoilInputs { get; }

        /// <summary>
        /// Gets the holding registers.
        /// </summary>
        IPointSource<ushort> HoldingRegisters { get; }

        /// <summary>
        /// Gets the input registers.
        /// </summary>
        IPointSource<ushort> InputRegisters { get; }
    }
}