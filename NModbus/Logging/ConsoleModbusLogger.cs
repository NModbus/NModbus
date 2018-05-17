namespace NModbus.Logging
{
    using System;

    public class ConsoleModbusLogger : ModbusLogger
    {
        private const int LevelColumnSize = 15;
        private static readonly string BlankHeader = Environment.NewLine + new string(' ', LevelColumnSize);

        public ConsoleModbusLogger(LoggingLevel minimumLoggingLevel = LoggingLevel.Debug) 
            : base(minimumLoggingLevel)
        {
        }

        protected override void LogCore(LoggingLevel level, string message)
        {
            message = message?.Replace(Environment.NewLine, BlankHeader);

            Console.WriteLine($"[{level}]".PadRight(LevelColumnSize) + message);
        }
    }
}