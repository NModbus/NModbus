namespace NModbus.Data
{
    public class SlaveDataStore : ISlaveDataStore
    {
        public IPointSource<ushort> HoldingRegisters { get; } = new PointSource<ushort>();

        public IPointSource<ushort> InputRegisters { get; } = new PointSource<ushort>();

        public IPointSource<bool> CoilDiscretes { get; } = new PointSource<bool>();

        public IPointSource<bool> CoilInputs { get; } = new PointSource<bool>();
    }
}
