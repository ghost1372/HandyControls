using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace HandyControl.Tools
{
    public static class Logger
    {
        private static readonly LogPublisher LogPublisher;

        private static readonly object Sync = new object();
        private static Level _defaultLevel = Level.Info;
        private static bool _isTurned = true;

        public enum Level
        {
            None,
            Debug,
            Fine,
            Info,
            Warning,
            Error,
            Severe
        }

        static Logger()
        {
            lock (Sync)
            {
                LogPublisher = new LogPublisher();
            }
        }

        public static void DefaultInitialization()
        {
            LoggerHandlerManager
                .AddHandler(new DebugLoggerHandler())
                .AddHandler(new FileLoggerHandler());

            Log(Level.Info, "Default initialization");
        }

        public static Level DefaultLevel
        {
            get { return _defaultLevel; }
            set { _defaultLevel = value; }
        }

        public static ILoggerHandlerManager LoggerHandlerManager
        {
            get { return LogPublisher; }
        }

        public static void Log()
        {
            Log("There is no message");
        }

#if !NET40
        public static void LogAsync()
        {
            LogAsync("There is no message");
        }

        public static void LogAsync(string message)
        {
            LogAsync(_defaultLevel, message);
        }

        public static void LogAsync(Level level, string message)
        {
            var stackFrame = FindStackFrame();
            var methodBase = GetCallingMethodBase(stackFrame);
            var callingMethod = methodBase.Name;
            var callingClass = methodBase.ReflectedType.Name;
            var lineNumber = stackFrame.GetFileLineNumber();

            LogAsync(level, message, callingClass, callingMethod, lineNumber);
        }

        public static void LogAsync(Exception exception)
        {
            LogAsync(Level.Error, exception.Message);
        }

        public static void LogAsync<TClass>(Exception exception) where TClass : class
        {
            var message = string.Format("Log exception -> Message: {0}\nStackTrace: {1}", exception.Message,
                                        exception.StackTrace);
            LogAsync<TClass>(Level.Error, message);
        }

        public static void LogAsync<TClass>(string message) where TClass : class
        {
            LogAsync<TClass>(_defaultLevel, message);
        }

        public static void LogAsync<TClass>(Level level, string message) where TClass : class
        {
            var stackFrame = FindStackFrame();
            var methodBase = GetCallingMethodBase(stackFrame);
            var callingMethod = methodBase.Name;
            var callingClass = typeof(TClass).Name;
            var lineNumber = stackFrame.GetFileLineNumber();

            LogAsync(level, message, callingClass, callingMethod, lineNumber);
        }

        private static void LogAsync(Level level, string message, string callingClass, string callingMethod, int lineNumber)
        {
            var logMessage = GetLogMessage(level, message, callingClass, callingMethod, lineNumber);
            if (logMessage != null)
            {
                LogPublisher.PublishAsync(logMessage);
            }
        }
#endif

        public static void Log(string message)
        {
            Log(_defaultLevel, message);
        }

        public static void Error(string message)
        {
            Log(Level.Error, message);
        }

        public static void Debug(string message)
        {
            Log(Level.Debug, message);
        }

        public static void Fine(string message)
        {
            Log(Level.Fine, message);
        }

        public static void Info(string message)
        {
            Log(Level.Info, message);
        }

        public static void Warning(string message)
        {
            Log(Level.Warning, message);
        }

        public static void Log(Level level, string message)
        {
            var stackFrame = FindStackFrame();
            var methodBase = GetCallingMethodBase(stackFrame);
            var callingMethod = methodBase.Name;
            var callingClass = methodBase.ReflectedType.Name;
            var lineNumber = stackFrame.GetFileLineNumber();

            Log(level, message, callingClass, callingMethod, lineNumber);
        }

        public static void Log(Exception exception)
        {
            Log(Level.Error, exception.Message);
        }

        public static void Log<TClass>(Exception exception) where TClass : class
        {
            var message = string.Format("Log exception -> Message: {0}\nStackTrace: {1}", exception.Message,
                                        exception.StackTrace);
            Log<TClass>(Level.Error, message);
        }

        public static void Log<TClass>(string message) where TClass : class
        {
            Log<TClass>(_defaultLevel, message);
        }

        public static void Log<TClass>(Level level, string message) where TClass : class
        {
            var stackFrame = FindStackFrame();
            var methodBase = GetCallingMethodBase(stackFrame);
            var callingMethod = methodBase.Name;
            var callingClass = typeof(TClass).Name;
            var lineNumber = stackFrame.GetFileLineNumber();

            Log(level, message, callingClass, callingMethod, lineNumber);
        }

        private static void Log(Level level, string message, string callingClass, string callingMethod, int lineNumber)
        {
            var logMessage = GetLogMessage(level, message, callingClass, callingMethod, lineNumber);
            if (logMessage != null)
            {
                LogPublisher.Publish(logMessage);
            }
        }

        private static LogMessage GetLogMessage(Level level, string message, string callingClass, string callingMethod, int lineNumber)
        {
            if (!_isTurned)
                return null;

            var currentDateTime = DateTime.Now;

            return new LogMessage(level, message, currentDateTime, callingClass, callingMethod, lineNumber);
        }

        private static MethodBase GetCallingMethodBase(StackFrame stackFrame)
        {
            return stackFrame == null
                ? MethodBase.GetCurrentMethod() : stackFrame.GetMethod();
        }

        private static StackFrame FindStackFrame()
        {
            var stackTrace = new StackTrace();
            for (var i = 0; i < stackTrace.GetFrames().Count(); i++)
            {
                var methodBase = stackTrace.GetFrame(i).GetMethod();
                var name = MethodBase.GetCurrentMethod().Name;
                if (!methodBase.Name.Equals("Log") && !methodBase.Name.Equals(name))
                    return new StackFrame(i, true);
            }
            return null;
        }

        public static void On()
        {
            _isTurned = true;
        }

        public static void Off()
        {
            _isTurned = false;
        }

        public static IEnumerable<LogMessage> Messages
        {
            get { return LogPublisher.Messages; }
        }

        public static bool StoreLogMessages
        { 
            get { return LogPublisher.StoreLogMessages; }
            set { LogPublisher.StoreLogMessages = value; }
        }

        static class FilterPredicates
        {
            public static bool ByLevelHigher(Level logMessLevel, Level filterLevel)
            {
                return ((int)logMessLevel >= (int)filterLevel);
            }

            public static bool ByLevelLower(Level logMessLevel, Level filterLevel)
            {
                return ((int)logMessLevel <= (int)filterLevel);
            }

            public static bool ByLevelExactly(Level logMessLevel, Level filterLevel)
            {
                return ((int)logMessLevel == (int)filterLevel);
            }

            public static bool ByLevel(LogMessage logMessage, Level filterLevel, Func<Level, Level, bool> filterPred)
            {
                return filterPred(logMessage.Level, filterLevel);
            }
        }

        public class FilterByLevel
        {
            public Level FilteredLevel { get; set; }
            public bool ExactlyLevel { get; set; }
            public bool OnlyHigherLevel { get; set; }

            public FilterByLevel(Level level)
            {
                FilteredLevel = level;
                ExactlyLevel = true;
                OnlyHigherLevel = true;
            }

            public FilterByLevel() 
            {
                ExactlyLevel = false;
                OnlyHigherLevel = true;
            }

            public Predicate<LogMessage> Filter { get { return delegate(LogMessage logMessage) {
                return FilterPredicates.ByLevel(logMessage, FilteredLevel, delegate(Level lm, Level fl) {
                return ExactlyLevel ? 
                    FilterPredicates.ByLevelExactly(lm, fl) : 
                    (OnlyHigherLevel ? 
                        FilterPredicates.ByLevelHigher(lm, fl) : 
                        FilterPredicates.ByLevelLower(lm, fl)
                    );
                    });
                }; 
            }  }
        }
    }
}
