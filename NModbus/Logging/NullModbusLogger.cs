using System.Threading;

namespace NModbus.Logging
{
    /// <summary>
    /// Empty logger.
    /// </summary>
    public class NullModbusLogger : IModbusLogger
    {
        private static object _lock = new object();
        private static NullModbusLogger _instance = null;
        /// <summary>
        /// Singleton.
        /// </summary>
        public static NullModbusLogger Instance
        {
            get
            {
                if (_instance is null)
                {
                    Monitor.Enter(_lock);
                    if (_instance is null)
                    {
                        _instance = new NullModbusLogger();
                    }
                    Monitor.Exit(_lock);
                }
                return _instance;
            }
        }

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