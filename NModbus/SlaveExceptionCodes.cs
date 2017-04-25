namespace NModbus
{
    internal static class SlaveExceptionCodes
    {
        // modbus slave exception codes
        public const byte IllegalFunction = 1;
        public const byte IllegalDataAddress = 2;
        public const byte Acknowledge = 5;
        public const byte SlaveDeviceBusy = 6;
    }
}