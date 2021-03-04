#if !(NET40 || NET45)
using System.Windows.Input;

namespace HandyControl.Tools
{
    public interface IDelegateCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
#endif
