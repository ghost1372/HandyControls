#if !NET40
// https://github.com/wf-soft/WFsoft.AsyncCommand

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HandyControl.Tools
{
    public abstract class AsyncCommandBase : IAsyncCommand
    {
        public abstract bool CanExecute(object parameter);

        public abstract Task ExecuteAsync(object parameter);

        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        protected void RaiseCanExecuteChanged() => CommandManager.InvalidateRequerySuggested();
    }

}
#endif
