namespace NModbus
{
    /// <summary>
    /// Simple logging target. Designed to be easily integrated into other logging frameworks.
    /// </summary>
    public interface IModbusLogger
    {
        /// <summary>
        /// Conditionally log a message
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        void Log(LoggingLevel level, string message);

        /// <summary>
        /// True if this level should be logged, false otherwise.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        bool ShouldLog(LoggingLevel level);
    }
}