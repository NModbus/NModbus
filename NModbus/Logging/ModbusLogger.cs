namespace NModbus.Logging
{
    /// <summary>
    /// Base class for Modbus loggers.
    /// </summary>
    public abstract class ModbusLogger : IModbusLogger
    {
        protected ModbusLogger(LoggingLevel minimumLoggingLevel)
        {
            MinimumLoggingLevel = minimumLoggingLevel;
        }

        protected LoggingLevel MinimumLoggingLevel { get; }

        /// <summary>
        /// Returns true if the level should be loggged, false otherwise.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool ShouldLog(LoggingLevel level)
        {
            return level >= MinimumLoggingLevel;
        }

        /// <summary>
        /// Log the specified message at the specified level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Log(LoggingLevel level, string message)
        {
            if (ShouldLog(level))
            {
                LogCore(level, message);
            }
        }

        /// <summary>
        /// Override this method to implement logging behavior. This function will only be called if ShouldLog(level) is true.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        protected abstract void LogCore(LoggingLevel level, string message);
    }
}