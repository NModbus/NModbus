namespace NModbus.Logging
{
    /// <summary>
    /// Empty logger.
    /// </summary>
    public class NullModbusLogger : IModbusLogger
    {
        /// <summary>
        /// Singleton.
        /// </summary>
        public static NullModbusLogger Instance = new NullModbusLogger();

        private NullModbusLogger()
        {
        }

        /// <summary>
        /// This won't do anything.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        public void Log(LoggingLevel level, string message)
        {
        }

        /// <summary>
        /// Always returnsa false
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public bool ShouldLog(LoggingLevel level)
        {
            return false;
        }
    }
}