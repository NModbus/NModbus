using System;
using System.Diagnostics;

namespace NModbus.Logging
{
    /// <summary>
    /// Writes using Debug.WriteLine().
    /// </summary>
    public class DebugModbusLogger : ModbusLogger
    {
        private const int LevelColumnSize = 15;
        private static readonly string BlankHeader = Environment.NewLine + new string(' ', LevelColumnSize);

        public DebugModbusLogger(LoggingLevel minimumLoggingLevel = LoggingLevel.Debug) 
            : base(minimumLoggingLevel)
        {
        }

        protected override void LogCore(LoggingLevel level, string message)
        {
            message = message?.Replace(Environment.NewLine, BlankHeader);

            Debug.WriteLine($"[{level}]".PadRight(LevelColumnSize) + message);
        }
    }
}