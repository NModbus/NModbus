using System;

namespace NModbus
{
    /// <summary>
    /// Object simulation of a device memory map.
    /// </summary>
    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use IServerDataStore instead.")]
    public interface ISlaveDataStore : IServerDataStore { }

    /// <summary>
    /// Object simulation of a device memory map.
    /// </summary>
    public interface IServerDataStore
    {
        /// <summary>
        /// Gets the discrete coils.
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