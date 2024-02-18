using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MitamatchOperations.Lib;

internal record Defer(Func<Task> Action) : IDisposable, ICommand
{
    void IDisposable.Dispose() => Action();

#pragma warning disable CS0067 // The event 'Defer.CanExecuteChanged' is never used
    public event EventHandler CanExecuteChanged;
#pragma warning restore CS0067 // The event 'Defer.CanExecuteChanged' is never used
    bool ICommand.CanExecute(object _) => true;
    void ICommand.Execute(object _) => Action.Invoke();
}
