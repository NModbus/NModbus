namespace NModbus
{
    /// <summary>
    /// Represents a memory map.
    /// </summary>
    /// <typeparam name="TPoint"></typeparam>
    public interface IPointSource<TPoint>
    {
        /// <summary>
        /// Read a series of points.
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="numberOfPoints"></param>
        /// <returns></returns>
        TPoint[] ReadPoints(ushort startAddress, ushort numberOfPoints);

        /// <summary>
        /// Write a series of points.
        /// </summary>
        /// <param name="startAddress"></param>
        /// <param name="points"></param>
        void WritePoints(ushort startAddress, TPoint[] points);
    }
}