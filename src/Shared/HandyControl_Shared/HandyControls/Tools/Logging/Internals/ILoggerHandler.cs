namespace HandyControl.Tools
{
    public interface ILoggerHandler
    {
        void Publish(LogMessage logMessage);
        void PublishAsync(LogMessage logMessage);
    }
}
