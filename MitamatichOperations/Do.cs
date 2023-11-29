using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace mitama;

internal record Defer(Func<Task> Action) : IDisposable, ICommand
{
    void IDisposable.Dispose() => Action();

    public event EventHandler CanExecuteChanged;
    bool ICommand.CanExecute(object _) => true;
    void ICommand.Execute(object _) => Action.Invoke();
}
