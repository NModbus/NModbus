namespace NModbus.Data
{
    public class DefaultSlaveDataStore : ISlaveDataStore
    {
        private readonly IPointSource<ushort> _holdingRegisters = new DefaultPointSource<ushort>();
        private readonly IPointSource<ushort> _inputRegisters = new DefaultPointSource<ushort>();
        private readonly IPointSource<bool> _coilDiscretes = new DefaultPointSource<bool>();
        private readonly IPointSource<bool> _coilInputs = new DefaultPointSource<bool>();

        public IPointSource<ushort> HoldingRegisters => _holdingRegisters;

        public IPointSource<ushort> InputRegisters => _inputRegisters;

        public IPointSource<bool> CoilDiscretes => _coilDiscretes;

        public IPointSource<bool> CoilInputs => _coilInputs;
    }
}