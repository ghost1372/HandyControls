// https://github.com/meziantou/Meziantou.Framework

#if !(NET40 || NET45 || NET451 || NET452)
using System;
using System.Windows.Threading;

namespace HandyControl.Tools
{
    internal sealed class SyncDelegateCommand : IDelegateCommand
    {
        private readonly Action<object?> _execute;
        private readonly Func<object?, bool> _canExecute;
        private readonly Dispatcher _dispatcher;

        public event EventHandler? CanExecuteChanged;

        public SyncDelegateCommand(Action<object?> execute, Func<object?, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute.Invoke(parameter);
        }

        public void Execute(object? parameter)
        {
            _execute.Invoke(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            if (_dispatcher != null)
            {
                _dispatcher.Invoke(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
            }
            else
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

    }

}
#endif
