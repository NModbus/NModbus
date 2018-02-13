namespace NModbus.Data
{
    public class SlaveDataStore : ISlaveDataStore
    {
        private readonly IPointSource<ushort> _holdingRegisters = new PointSource<ushort>();
        private readonly IPointSource<ushort> _inputRegisters = new PointSource<ushort>();
        private readonly IPointSource<bool> _coilDiscretes = new PointSource<bool>();
        private readonly IPointSource<bool> _coilInputs = new PointSource<bool>();

        IPointSource<ushort> ISlaveDataStore.HoldingRegisters => _holdingRegisters;

        IPointSource<ushort> ISlaveDataStore.InputRegisters => _inputRegisters;

        IPointSource<bool> ISlaveDataStore.CoilDiscretes => _coilDiscretes;

        IPointSource<bool> ISlaveDataStore.CoilInputs => _coilInputs;
    }
}
