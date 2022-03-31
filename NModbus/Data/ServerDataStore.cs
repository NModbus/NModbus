using System;

namespace NModbus.Data
{
    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ServerDataStore instead.")]
    public class SlaveDataStore : ServerDataStore, ISlaveDataStore {}

    public class ServerDataStore : IServerDataStore
    {
        public PointSource<ushort> HoldingRegisters { get; } = new PointSource<ushort>();

        public PointSource<ushort> InputRegisters { get; } = new PointSource<ushort>();

        public PointSource<bool> CoilDiscretes { get; } = new PointSource<bool>();

        public PointSource<bool> CoilInputs { get; } = new PointSource<bool>();

        #region IServerDataStore

        IPointSource<ushort> IServerDataStore.HoldingRegisters => HoldingRegisters;

        IPointSource<ushort> IServerDataStore.InputRegisters => InputRegisters;

        IPointSource<bool> IServerDataStore.CoilDiscretes => CoilDiscretes;

        IPointSource<bool> IServerDataStore.CoilInputs => CoilInputs;

        #endregion
    }
}
