namespace NModbus
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IConcurrentModbusMaster : IDisposable
    {
        Task<ushort[]> ReadInputRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, ushort blockSize = 125, CancellationToken cancellationToken = default(CancellationToken));

        Task<ushort[]> ReadHoldingRegistersAsync(byte slaveAddress, ushort startAddress, ushort numberOfPoints, ushort blockSize = 125, CancellationToken cancellationToken = default(CancellationToken));

        Task WriteMultipleRegistersAsync(byte slaveAddress, ushort startAddress, ushort[] data, ushort blockSize = 121, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool[]> ReadCoilsAsync(byte slaveAddress, ushort startAddress, ushort number, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool[]> ReadDiscretesAsync(byte slaveAddress, ushort startAddress, ushort number, CancellationToken cancellationToken = default(CancellationToken));

        Task WriteCoilsAsync(byte slaveAddress, ushort startAddress, bool[] data, CancellationToken cancellationToken = default(CancellationToken));
        
        Task WriteSingleCoilAsync(byte slaveAddress, ushort coilAddress, bool value, CancellationToken cancellationToken = default(CancellationToken));

        Task WriteSingleRegisterAsync(byte slaveAddress, ushort address, ushort value, CancellationToken cancellationToken = default(CancellationToken));
    }
}