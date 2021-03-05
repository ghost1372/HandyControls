// https://github.com/wf-soft/WFsoft.AsyncCommand

using System.Threading.Tasks;
using System.Windows.Input;

namespace HandyControl.Tools
{
    internal interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }
}
