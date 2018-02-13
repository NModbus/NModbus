namespace NModbus.Device
{
    public class PointEventArgs<T> : PointEventArgs where T : struct
    {
        private readonly T[] _points;

        public PointEventArgs(ushort startAddress, T[] points) : base(startAddress, (ushort)points.Length)
        {
            _points = points;
        }

        public T[] Points => _points;
    }
}
