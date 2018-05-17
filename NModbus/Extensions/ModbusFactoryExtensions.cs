namespace NModbus.Extensions
{
    using System;

    internal static class ModbusFactoryExtensions
    {
        private const int MinRequestFrameLength = 3;

        public static IModbusMessage CreateModbusRequest(this IModbusFactory factory, byte[] frame)
        {
            if (frame.Length < MinRequestFrameLength)
            {
                string msg = $"Argument 'frame' must have a length of at least {MinRequestFrameLength} bytes.";
                throw new FormatException(msg);
            }

            byte functionCode = frame[1];

            var functionService = factory.GetFunctionService(functionCode);

            return functionService.CreateRequest(frame);
        }
    }
}