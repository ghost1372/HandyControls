using System;

namespace HandyControl.Tools
{
    public class DebugLogger
    {
        private const Logger.Level DebugLevel = Logger.Level.Debug;

        public void Log()
        {
            Log("There is no message");
        }

        public void Log(string message)
        {
            Logger.Log(DebugLevel, message);
        }

        public void Log(Exception exception)
        {
            Logger.Log(DebugLevel, exception.Message);
        }

        public void Log<TClass>(Exception exception) where TClass : class
        {
            var message = string.Format("Log exception -> Message: {0}\nStackTrace: {1}", exception.Message, exception.StackTrace);
            Logger.Log<TClass>(DebugLevel, message);
        }

        public void Log<TClass>(string message) where TClass : class
        {
            Logger.Log<TClass>(DebugLevel, message);
        }
    }
}
