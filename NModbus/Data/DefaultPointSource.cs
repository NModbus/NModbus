using System;
using System.Linq;
using NModbus.Unme.Common;

namespace NModbus.Data
{
    /// <summary>
    /// A simple implementation of the point source. All registers are 
    /// </summary>
    /// <typeparam name="TPoint"></typeparam>
    internal class DefaultPointSource<TPoint> : IPointSource<TPoint>
    {
        //Only create this if referenced.
        private readonly Lazy<TPoint[]> _points;

        private readonly object _syncRoot = new object();

        public DefaultPointSource()
        {
            _points = new Lazy<TPoint[]>(() => new TPoint[ushort.MaxValue+1]);
        }

        public TPoint[] ReadPoints(ushort startAddress, ushort numberOfPoints)
        {
            lock (_syncRoot)
            {
                return _points.Value
                    .Slice(startAddress, numberOfPoints)
                    .ToArray();
            }
        }

        public void WritePoints(ushort startAddress, TPoint[] points)
        {
            lock (_syncRoot)
            {
                for (ushort index = 0; index < points.Length; index++)
                {
                    _points.Value[startAddress + index] = points[index];
                }
            }
        }
    }
}