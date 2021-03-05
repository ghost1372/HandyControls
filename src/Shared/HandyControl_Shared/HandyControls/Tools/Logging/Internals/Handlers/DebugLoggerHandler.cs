namespace HandyControl.Tools
{
    public class DebugLoggerHandler : ILoggerHandler
    {
        private readonly ILoggerFormatter _loggerFormatter;

        public DebugLoggerHandler() : this(new DefaultLoggerFormatter()) { }

        public DebugLoggerHandler(ILoggerFormatter loggerFormatter)
        {
            _loggerFormatter = loggerFormatter;
        }

        public void Publish(LogMessage logMessage)
        {
            System.Diagnostics.Debug.WriteLine(_loggerFormatter.ApplyFormat(logMessage));
        }

#if !NET40
        public void PublishAsync(LogMessage logMessage)
        {
            System.Diagnostics.Debug.WriteLine(_loggerFormatter.ApplyFormat(logMessage));
        }
#endif
    }
}
