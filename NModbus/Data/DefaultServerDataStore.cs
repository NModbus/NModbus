using System;

namespace NModbus.Data
{
    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use DefaultServerDataStore instead.")]
    internal class DefaultSlaveDataStore : DefaultServerDataStore, ISlaveDataStore { }

    internal class DefaultServerDataStore : IServerDataStore
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