using System;
using System.Collections.Generic;

namespace HandyControl.Tools
{
    internal class FilteredHandler : ILoggerHandler
    {
        public Predicate<LogMessage> Filter { get; set; }
        public ILoggerHandler Handler { get; set; }

        public void Publish(LogMessage logMessage) 
        {
            if (Filter(logMessage))
                Handler.Publish (logMessage);
        }

#if !NET40
        public void PublishAsync(LogMessage logMessage)
        {
            if (Filter(logMessage))
                Handler.PublishAsync(logMessage);
        }
#endif
    }

    internal class LogPublisher : ILoggerHandlerManager
    {
        private readonly IList<ILoggerHandler> _loggerHandlers;
        private readonly IList<LogMessage> _messages;

        public LogPublisher()
        {
            _loggerHandlers = new List<ILoggerHandler>();
            _messages = new List<LogMessage>();
            StoreLogMessages = false;
        }

        public LogPublisher(bool storeLogMessages)
        {
            _loggerHandlers = new List<ILoggerHandler>();
            _messages = new List<LogMessage>();
            StoreLogMessages = storeLogMessages;
        }

        public void Publish(LogMessage logMessage)
        {
            if (StoreLogMessages)
                _messages.Add(logMessage);
            foreach (var loggerHandler in _loggerHandlers)
                loggerHandler.Publish(logMessage);
        }

#if !NET40
        public void PublishAsync(LogMessage logMessage)
        {
            if (StoreLogMessages)
                _messages.Add(logMessage);
            foreach (var loggerHandler in _loggerHandlers)
                loggerHandler.PublishAsync(logMessage);
        }
#endif

        public ILoggerHandlerManager AddHandler(ILoggerHandler loggerHandler)
        {
            if (loggerHandler != null)
                _loggerHandlers.Add(loggerHandler);
            return this;
        }

        public ILoggerHandlerManager AddHandler(ILoggerHandler loggerHandler, Predicate<LogMessage> filter)
        {
            if ((filter == null) || loggerHandler == null)
                return this;

            return AddHandler(new FilteredHandler() {
                Filter = filter,
                Handler = loggerHandler
            });
        }

        public bool RemoveHandler(ILoggerHandler loggerHandler)
        {
            return _loggerHandlers.Remove(loggerHandler);
        }

        public IEnumerable<LogMessage> Messages
        {
            get { return _messages; }
        }

        public bool StoreLogMessages { get; set; }
    }
}
