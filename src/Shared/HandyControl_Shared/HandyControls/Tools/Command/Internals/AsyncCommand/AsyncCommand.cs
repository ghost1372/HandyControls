#if !NET40
// https://github.com/wf-soft/WFsoft.AsyncCommand

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HandyControl.Tools
{
    public class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
    {
        private readonly Func<Task<TResult>> _command1;
        private readonly Func<CancellationToken, Task<TResult>> _command2;
        private readonly Func<CancellationToken, ManualResetEvent, Task<TResult>> _command3;
        private readonly CancelAsyncCommand _cancelCommand;
        private readonly ManualResetAsyncCommand _manualResetAsyncCommand;
        private bool _isCancelUpTask;
        private bool isSuspend;
        public bool IsSuspend
        {
            get => isSuspend;
            private set { isSuspend = value; OnPropertyChanged(); }
        }
        private NotifyTaskCompletion<TResult> _execution;
        public AsyncCommand(Func<Task<TResult>> command)
        {
            _command1 = command;
        }
        public AsyncCommand(Func<CancellationToken, Task<TResult>> command, bool isCancelUpTask)
        {
            _isCancelUpTask = isCancelUpTask;
            _command2 = command;
            _cancelCommand = new CancelAsyncCommand();
        }
        public AsyncCommand(Func<CancellationToken, ManualResetEvent, Task<TResult>> command, bool isCancelUpTask)
        {
            _isCancelUpTask = isCancelUpTask;
            _command3 = command;
            _cancelCommand = new CancelAsyncCommand();
            _manualResetAsyncCommand = new ManualResetAsyncCommand();
        }

        private void _manualResetAsyncCommand_OnStateChang(bool val)
        {
            IsSuspend = val;
        }

        private void _cancelCommand_OnCancel()
        {
            { if (isSuspend) _manualResetAsyncCommand.Execute(null); };
        }

        public override bool CanExecute(object parameter)
        {
            if (_isCancelUpTask) return true;
            else return Execution == null || Execution.IsCompleted;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            if (_command1 != null)
            {
                Execution = new NotifyTaskCompletion<TResult>(_command1());
                RaiseCanExecuteChanged();
                await Execution.TaskCompletion;
            }
            else if (_command2 != null)
            {
                if (_isCancelUpTask) _cancelCommand.Execute(null);
                _cancelCommand.NotifyCommandStarting();
                Execution = new NotifyTaskCompletion<TResult>(_command2(_cancelCommand.Token));
                RaiseCanExecuteChanged();
                await Execution.TaskCompletion;
                if (Execution.IsCompleted)
                {
                    _cancelCommand.NotifyCommandFinished();
                }
            }
            else
            {
                _cancelCommand.OnCancel += _cancelCommand_OnCancel;
                _manualResetAsyncCommand.OnStateChang += _manualResetAsyncCommand_OnStateChang;

                if (_isCancelUpTask) _cancelCommand.Execute(null);
                _cancelCommand.NotifyCommandStarting();
                _manualResetAsyncCommand.NotifyCommandStarting();
                Execution = new NotifyTaskCompletion<TResult>(_command3(_cancelCommand.Token, _manualResetAsyncCommand.manualResetEvent));
                RaiseCanExecuteChanged();
                await Execution.TaskCompletion;
                if (Execution.IsCompleted)
                {
                    _cancelCommand.NotifyCommandFinished();
                    _manualResetAsyncCommand.NotifyCommandFinished();
                    _cancelCommand.OnCancel -= _cancelCommand_OnCancel;
                    _manualResetAsyncCommand.OnStateChang -= _manualResetAsyncCommand_OnStateChang;
                }
            }
            RaiseCanExecuteChanged();
        }

        public ICommand CancelCommand => _cancelCommand;
        public ICommand ManualResetAsyncCommand => _manualResetAsyncCommand;

        public NotifyTaskCompletion<TResult> Execution
        {
            get => _execution;
            private set
            {
                _execution = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class AsyncCommand<T, TResult> : AsyncCommandBase, INotifyPropertyChanged
    {
        private readonly Func<T, Task<TResult>> _command1;
        private readonly Func<T, CancellationToken, Task<TResult>> _command2;
        private readonly Func<T, CancellationToken, ManualResetEvent, Task<TResult>> _command3;
        private readonly CancelAsyncCommand _cancelCommand;
        private readonly ManualResetAsyncCommand _manualResetAsyncCommand;
        private bool _isCancelUpTask;
        private bool isSuspend;
        public bool IsSuspend
        {
            get => isSuspend;
            private set { isSuspend = value; OnPropertyChanged(); }
        }
        private NotifyTaskCompletion<TResult> _execution;

        public AsyncCommand(Func<T, Task<TResult>> command)
        {
            _command1 = command;
        }
        public AsyncCommand(Func<T, CancellationToken, Task<TResult>> command, bool isCancelUpTask)
        {
            _isCancelUpTask = isCancelUpTask;
            _command2 = command;
            _cancelCommand = new CancelAsyncCommand();
        }
        public AsyncCommand(Func<T, CancellationToken, ManualResetEvent, Task<TResult>> command, bool isCancelUpTask)
        {
            _isCancelUpTask = isCancelUpTask;
            _command3 = command;
            _cancelCommand = new CancelAsyncCommand();
            _manualResetAsyncCommand = new ManualResetAsyncCommand();
        }

        private void _manualResetAsyncCommand_OnStateChang(bool val)
        {
            IsSuspend = val;
        }

        private void _cancelCommand_OnCancel()
        {
            if (isSuspend) _manualResetAsyncCommand.Execute(null);
        }

        public override bool CanExecute(object parameter)
        {
            if (_isCancelUpTask) return true;
            else return Execution == null || Execution.IsCompleted;
        }

        public override async Task ExecuteAsync(object parameter)
        {
            if (_command1 != null)
            {
                Execution = new NotifyTaskCompletion<TResult>(_command1((T) parameter));
                RaiseCanExecuteChanged();
                await Execution.TaskCompletion;
            }
            else if (_command2 != null)
            {
                if (_isCancelUpTask) _cancelCommand.Execute(null);
                _cancelCommand.NotifyCommandStarting();
                Execution = new NotifyTaskCompletion<TResult>(_command2((T) parameter, _cancelCommand.Token));
                RaiseCanExecuteChanged();
                await Execution.TaskCompletion;
                if (Execution.IsCompleted)
                {
                    _cancelCommand.NotifyCommandFinished();
                }
            }
            else
            {
                _cancelCommand.OnCancel += _cancelCommand_OnCancel;
                _manualResetAsyncCommand.OnStateChang += _manualResetAsyncCommand_OnStateChang;
                if (_isCancelUpTask)
                {
                    _cancelCommand.Execute(null);
                }
                _cancelCommand.NotifyCommandStarting();
                _manualResetAsyncCommand.NotifyCommandStarting();
                Execution = new NotifyTaskCompletion<TResult>(_command3((T) parameter, _cancelCommand.Token, _manualResetAsyncCommand.manualResetEvent));
                RaiseCanExecuteChanged();
                await Execution.TaskCompletion;
                if (Execution.IsCompleted)
                {
                    _cancelCommand.NotifyCommandFinished();
                    _manualResetAsyncCommand.NotifyCommandFinished();
                    _cancelCommand.OnCancel -= _cancelCommand_OnCancel;
                    _manualResetAsyncCommand.OnStateChang -= _manualResetAsyncCommand_OnStateChang;
                }
            }
            RaiseCanExecuteChanged();
        }

        public ICommand CancelCommand => _cancelCommand;
        public ICommand ManualResetAsyncCommand => _manualResetAsyncCommand;
        public NotifyTaskCompletion<TResult> Execution
        {
            get => _execution;
            private set
            {
                _execution = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
#endif
