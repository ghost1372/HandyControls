namespace HandyControl.Tools
{
    public interface ILoggerHandler
    {
        void Publish(LogMessage logMessage);

#if !NET40
        void PublishAsync(LogMessage logMessage);
#endif
    }
}
