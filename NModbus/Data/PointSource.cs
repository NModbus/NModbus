using NModbus.Device;
using NModbus.Unme.Common;
using System;
using System.Linq;

namespace NModbus.Data
{
    /// <summary>
    /// A simple implementation of the point source. All registers are.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PointSource<T> : IPointSource<T> where T : struct
    {
        public event EventHandler<PointEventArgs> BeforeRead;

        public event EventHandler<PointEventArgs<T>> BeforeWrite;

        public event EventHandler<PointEventArgs> AfterWrite;

        //Only create this if referenced.
        private readonly Lazy<T[]> _points;

        private readonly object _syncRoot = new object();

        public PointSource()
        {
            _points = new Lazy<T[]>(() => new T[ushort.MaxValue]);
        }

        public T[] ReadPoints(ushort startAddress, ushort numberOfPoints)
        {
            lock (_syncRoot)
            {
                return _points.Value
                    .Slice(startAddress, numberOfPoints)
                    .ToArray();
            }
        }

        T[] IPointSource<T>.ReadPoints(ushort startAddress, ushort numberOfPoints)
        {
            BeforeRead?.Invoke(this, new PointEventArgs(startAddress, numberOfPoints));
            return ReadPoints(startAddress, numberOfPoints);
        }

        public void WritePoints(ushort startAddress, T[] points)
        {
            lock (_syncRoot)
            {
                for (ushort index = 0; index < points.Length; index++)
                {
                    _points.Value[startAddress + index] = points[index];
                }
            }
        }

        void IPointSource<T>.WritePoints(ushort startAddress, T[] points)
        {
            BeforeWrite?.Invoke(this, new PointEventArgs<T>(startAddress, points));
            WritePoints(startAddress, points);
            AfterWrite?.Invoke(this, new PointEventArgs(startAddress, (ushort)points.Length));
        }
    }
}
