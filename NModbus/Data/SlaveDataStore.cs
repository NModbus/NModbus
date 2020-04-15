namespace NModbus.Data
{
    public class SlaveDataStore : ISlaveDataStore
    {
        public PointSource<ushort> HoldingRegisters { get; } = new PointSource<ushort>();

        public PointSource<ushort> InputRegisters { get; } = new PointSource<ushort>();

        public PointSource<bool> CoilDiscretes { get; } = new PointSource<bool>();

        public PointSource<bool> CoilInputs { get; } = new PointSource<bool>();

        #region ISlaveDataStore

        IPointSource<ushort> ISlaveDataStore.HoldingRegisters => HoldingRegisters;

        IPointSource<ushort> ISlaveDataStore.InputRegisters => InputRegisters;

        IPointSource<bool> ISlaveDataStore.CoilDiscretes => CoilDiscretes;

        IPointSource<bool> ISlaveDataStore.CoilInputs => CoilInputs;

        #endregion
    }
}
