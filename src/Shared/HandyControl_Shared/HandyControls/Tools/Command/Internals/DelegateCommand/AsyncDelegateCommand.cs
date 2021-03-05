// https://github.com/meziantou/Meziantou.Framework

#if !(NET40 || NET45 || NET451 || NET452)
using System;
using System.Threading.Tasks;
using System.Windows.Threading;
namespace HandyControl.Tools
{
    internal sealed class AsyncDelegateCommand : IDelegateCommand
    {
        private readonly Func<object?, Task> _execute;
        private readonly Func<object?, bool> _canExecute;
        private readonly Dispatcher _dispatcher;
        private bool _isExecuting;

        public event EventHandler? CanExecuteChanged;

        public AsyncDelegateCommand(Func<object?, Task> execute, Func<object?, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
            _dispatcher = Dispatcher.CurrentDispatcher;
        }

        public bool CanExecute(object? parameter)
        {
            return !_isExecuting && _canExecute.Invoke(parameter);
        }

        public async void Execute(object? parameter)
        {
            if (_isExecuting)
                return;

            try
            {
                _isExecuting = true;
                RaiseCanExecuteChanged();
                await _execute.Invoke(parameter);
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }

        }

        public void RaiseCanExecuteChanged()
        {
            var canExecuteChanged = CanExecuteChanged;
            if (canExecuteChanged != null)
            {
                if (_dispatcher != null)
                {
                    _dispatcher.Invoke(() => canExecuteChanged.Invoke(this, EventArgs.Empty));
                }
                else
                {
                    canExecuteChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
}
#endif
