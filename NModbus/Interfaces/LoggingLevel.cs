namespace NModbus
{
    /// <summary>
    /// Following the guidelines from https://github.com/aspnet/Logging/wiki/Guidelines.
    /// </summary>
    public enum LoggingLevel
    {
        /// <summary>
        /// The most detailed log messages, may contain sensitive application data. These messages should be disabled by default and should never be enabled in a production environment.
        /// </summary>
        Trace = 0,

        /// <summary>
        /// Logs that are used for interactive investigation during development should use the Debug level. These logs should primarily contain information useful for debugging and have no long-term value. 
        /// This is the default most verbose level of logging.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Track the general flow of the application using logs at the Information level. These logs should have value in the long term.
        /// </summary>
        Information = 2,

        /// <summary>
        /// Warnings should highlight an abnormal or unexpected event in the application flow. This event does not cause the application execution to stop, but can signify sub-optimal performance or a potential problem for the future. A common place to log a warning is from handled exceptions. 
        /// </summary>
        Warning = 3,

        /// <summary>
        /// An error should be logged when the current flow of execution is stopped due to a failure. These should indicate a failure in the current activity, not an application-wide failure. These will mainly be unhandled exceptions and recoverable failures. 
        /// </summary>
        Error = 4,

        /// <summary>
        /// A critical log should describe an unrecoverable application or system crash, or a catastrophic failure that requires immediate attention. 
        /// </summary>
        Critical = 5
    }
}