// https://github.com/wf-soft/WFsoft.AsyncCommand

using System;
using System.Threading;
using System.Windows.Input;

namespace HandyControl.Tools
{
    internal sealed class ManualResetAsyncCommand : ICommand
    {
        public ManualResetEvent manualResetEvent { get; set; } = new ManualResetEvent(true);
        private bool isSuspend { get; set; }

        private Action<bool> onStateChang;
        public event Action<bool> OnStateChang
        {
            add { if (onStateChang is null) onStateChang += value; }
            remove { onStateChang -= value; }
        }
        private bool _commandExecuting;
        public void NotifyCommandStarting()
        {
            _commandExecuting = true;
            RaiseCanExecuteChanged();
        }

        public void NotifyCommandFinished()
        {
            _commandExecuting = false;
            RaiseCanExecuteChanged();
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public bool CanExecute(object parameter) => _commandExecuting;

        public void Execute(object parameter)
        {
            isSuspend = !isSuspend;
            onStateChang?.Invoke(isSuspend);
            RaiseCanExecuteChanged();
            if (!isSuspend)
                manualResetEvent.Set();
            else
                manualResetEvent.Reset();
        }
        private void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }
}
