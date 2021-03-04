#if !NET40

using System.Windows.Input;

namespace HandyControl.Tools
{
    public interface IDelegateCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
#endif
