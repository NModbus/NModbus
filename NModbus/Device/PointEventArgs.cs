using System;


namespace NModbus.Device
{
    /// <summary>
    /// Modbus Slave request event args containing information on the message.
    /// </summary>
    public class PointEventArgs : EventArgs
    {
        private ushort _numberOfPoints;

        private ushort _startAddress;

        public PointEventArgs(ushort startAddress, ushort numberOfPoints)
        {
            _startAddress = startAddress;
            _numberOfPoints = numberOfPoints;
        }

        public ushort NumberOfPoints => _numberOfPoints;

        public ushort StartAddress => _startAddress;
    }
}
