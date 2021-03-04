// https://github.com/meziantou/Meziantou.Framework

#if !(NET40 || NET45 || NET451 || NET452)
using System.Windows.Input;

namespace HandyControl.Tools
{
    public interface IDelegateCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }
}
#endif
