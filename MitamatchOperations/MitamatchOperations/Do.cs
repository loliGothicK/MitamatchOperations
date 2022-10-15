using System;
using System.Windows.Input;

namespace mitama;

internal record Defer(Action Action) : IDisposable, ICommand
{
    void IDisposable.Dispose() => Action();

    public event EventHandler? CanExecuteChanged;

    bool ICommand.CanExecute(object? _) => true;

    void ICommand.Execute(object? _) => Action.Invoke();
}
