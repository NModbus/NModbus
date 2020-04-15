#if NETSTANDARD1_3
#else

using System;
using System.Diagnostics;

namespace NModbus.Logging
{
    public class TraceModbusLogger : ModbusLogger
    {
        private const int LevelColumnSize = 15;
        private static readonly string BlankHeader = Environment.NewLine + new string(' ', LevelColumnSize);

        public TraceModbusLogger(LoggingLevel minimumLoggingLevel = LoggingLevel.Debug)
            : base(minimumLoggingLevel)
        {
        }

        protected override void LogCore(LoggingLevel level, string message)
        {
            message = message?.Replace(Environment.NewLine, BlankHeader);

            Trace.WriteLine($"[{level}]".PadRight(LevelColumnSize) + message);
        }
    }
}

#endif