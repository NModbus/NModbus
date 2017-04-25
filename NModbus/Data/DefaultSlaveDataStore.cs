namespace NModbus.Data
{
    internal class DefaultSlaveDataStore : ISlaveDataStore
    {
        private readonly IPointSource<ushort> _holdingRegisters = new DefaultPointSource<ushort>();
        private readonly IPointSource<ushort> _inputRegisters = new DefaultPointSource<ushort>();
        private readonly IPointSource<bool> _coilDiscretes = new DefaultPointSource<bool>();
        private readonly IPointSource<bool> _coilInputs = new DefaultPointSource<bool>();

        public IPointSource<ushort> HoldingRegisters
        {
            get { return _holdingRegisters; }
        }

        public IPointSource<ushort> InputRegisters
        {
            get { return _inputRegisters; }
        }

        public IPointSource<bool> CoilDiscretes
        {
            get { return _coilDiscretes; }
        }

        public IPointSource<bool> CoilInputs
        {
            get { return _coilInputs; }
        }
    }
}