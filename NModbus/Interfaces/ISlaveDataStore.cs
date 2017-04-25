namespace NModbus
{
    /// <summary>
    /// Object simulation of a device memory map.
    /// </summary>
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