﻿namespace NModbus.Device
{
    using NModbus;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    [Obsolete("Master/Slave terminology is deprecated and replaced with Client/Server. Use ConcurrentModbusClient instead.")]
    public class ConcurrentModbusMaster : ConcurrentModbusClient, IConcurrentModbusMaster
    {
        public ConcurrentModbusMaster(IModbusMaster master, TimeSpan minInterval) : base(master, minInterval) {}
    }

    /// <summary>
    /// Provides concurrency control across multiple Modbus readers/writers.
    /// </summary>
    public class ConcurrentModbusClient : IConcurrentModbusClient
    {
        private readonly IModbusClient _client;
        private readonly TimeSpan _minInterval;

        private bool _isDisposed;

        private readonly Stopwatch _stopwatch = new Stopwatch();

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public ConcurrentModbusClient(IModbusClient client, TimeSpan minInterval)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _minInterval = minInterval;

            _stopwatch.Start();
        }

        private Task WaitAsync(CancellationToken cancellationToken)
        {
            int difference = (int)(_minInterval - _stopwatch.Elapsed).TotalMilliseconds;

            if (difference > 0)
            {
                return Task.Delay(difference, cancellationToken);
            }

#if NET45
            return CompletedTask();
#else
            return Task.CompletedTask;
#endif
        }

#if NET45
        private static readonly Task completedTask = Task.FromResult(false);

        public static Task CompletedTask()
        {
            return completedTask;
        }
#endif

        private async Task<T> PerformFuncAsync<T>(Func<Task<T>> action, CancellationToken cancellationToken)
        {
            T value = default(T);

            await PerformAsync(async () => value = await action(), cancellationToken);

            return value;
        }

        private async Task PerformAsync(Func<Task> action, CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                await WaitAsync(cancellationToken);

                await action();
            }
            finally
            {
                _semaphore.Release();

                _stopwatch.Restart();
            }
        }

        public async Task<ushort[]> ReadInputRegistersAsync(byte serverAddress, ushort startAddress, ushort numberOfPoints, ushort blockSize, CancellationToken cancellationToken)
        {
            return await PerformFuncAsync(async ()  =>
            {
                List<ushort> registers = new List<ushort>(numberOfPoints);

                int soFar = 0;
                int thisRead = blockSize;

                while (soFar < numberOfPoints)
                {
                    //If we're _not_ on the first run through here, wait for the min time
                    if (soFar > 0)
                    {
                        await Task.Delay(_minInterval, cancellationToken);
                    }

                    //Check to see if we've ben cancelled
                    cancellationToken.ThrowIfCancellationRequested();

                    if (thisRead > (numberOfPoints - soFar))
                    {
                        thisRead = numberOfPoints - soFar;
                    }

                    //Perform this operation
                    ushort[] registersFromThisRead = await _client.ReadInputRegistersAsync(serverAddress, (ushort)(startAddress + soFar), (ushort)thisRead);

                    //Add these to the result
                    registers.AddRange(registersFromThisRead);

                    //Increment where we're at
                    soFar += thisRead;
                }

                return registers.ToArray();

            }, cancellationToken);
        }

        public Task<ushort[]> ReadHoldingRegistersAsync(byte serverAddress, ushort startAddress, ushort numberOfPoints, ushort blockSize, CancellationToken cancellationToken)
        {
            return PerformFuncAsync(async () =>
            {
                List<ushort> registers = new List<ushort>(numberOfPoints);

                int soFar = 0;
                int thisRead = blockSize;

                while (soFar < numberOfPoints)
                {
                    //If we're _not_ on the first run through here, wait for the min time
                    if (soFar > 0)
                    {
                        await Task.Delay(_minInterval, cancellationToken);
                    }

                    //Check to see if we've ben cancelled
                    cancellationToken.ThrowIfCancellationRequested();

                    if (thisRead > (numberOfPoints - soFar))
                    {
                        thisRead = numberOfPoints - soFar;
                    }

                    //Perform this operation
                    ushort[] registersFromThisRead = await _client.ReadHoldingRegistersAsync(serverAddress, (ushort)(startAddress + soFar), (ushort)thisRead);

                    //Add these to the result
                    registers.AddRange(registersFromThisRead);

                    //Increment where we're at
                    soFar += thisRead;
                }

                return registers.ToArray();

            }, cancellationToken);
        }

        public Task WriteMultipleRegistersAsync(byte serverAddress, ushort startAddress, ushort[] data, ushort blockSize, CancellationToken cancellationToken)
        {
            return PerformAsync(async () =>
            {
                int soFar = 0;
                int thisWrite = blockSize;

                while (soFar < data.Length)
                {
                    //If we're _not_ on the first run through here, wait for the min time
                    if (soFar > 0)
                    {
                        await Task.Delay(_minInterval, cancellationToken);
                    }

                    if (thisWrite > (data.Length - soFar))
                    {
                        thisWrite = data.Length - soFar;
                    }

                    ushort[] registers = data.Skip(soFar).Take(thisWrite).ToArray();

                    await _client.WriteMultipleRegistersAsync(serverAddress, (ushort) (startAddress + soFar), registers);

                    soFar += thisWrite;
                }

            }, cancellationToken);  
        }

        public Task WriteSingleRegisterAsync(byte serverAddress, ushort address, ushort value, CancellationToken cancellationToken)
        {
            return PerformAsync(() => _client.WriteSingleRegisterAsync(serverAddress, address, value), cancellationToken);
        }

        public Task WriteCoilsAsync(byte serverAddress, ushort startAddress, bool[] data, CancellationToken cancellationToken)
        {
            return PerformAsync(() => _client.WriteMultipleCoilsAsync(serverAddress, startAddress, data),  cancellationToken);
        }

        public Task<bool[]> ReadCoilsAsync(byte serverAddress, ushort startAddress, ushort number,
            CancellationToken cancellationToken)
        {
            return PerformFuncAsync(() => _client.ReadCoilsAsync(serverAddress, startAddress, number), cancellationToken);
        }

        public Task<bool[]> ReadDiscretesAsync(byte serverAddress, ushort startAddress, ushort number, CancellationToken cancellationToken)
        {
            return PerformFuncAsync(() => _client.ReadInputsAsync(serverAddress, startAddress, number), cancellationToken);
        }

        public Task WriteSingleCoilAsync(byte serverAddress, ushort coilAddress, bool value, CancellationToken cancellationToken)
        {
            return PerformAsync(() => _client.WriteSingleCoilAsync(serverAddress, coilAddress, value), cancellationToken);
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                _client.Dispose();
            }
        }
    }

}