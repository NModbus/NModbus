namespace NModbus
{
    /// <summary>
    /// Supported function codes
    /// </summary>
    public static class ModbusFunctionCodes
    {
        public const byte ReadCoils = 1;

        public const byte ReadInputs = 2;

        public const byte ReadHoldingRegisters = 3;

        public const byte ReadInputRegisters = 4;

        public const byte WriteSingleCoil = 5;

        public const byte WriteSingleRegister = 6;

        public const byte Diagnostics = 8;

        public const ushort DiagnosticsReturnQueryData = 0;

        public const byte WriteMultipleCoils = 15;

        public const byte WriteMultipleRegisters = 16;
        
        public const byte WriteFileRecord = 21;

        public const byte ReadWriteMultipleRegisters = 23;
    }
}