using System;
using ABI.System.Windows.Input;
using mitama.Pages.OrderConsole;
using static System.Collections.Specialized.BitVector32;

namespace mitama;

internal record Defer(Action Action) : IDisposable, ICommand
{
    void IDisposable.Dispose() => Action();

    public event EventHandler? CanExecuteChanged;

    bool System.Windows.Input.ICommand.CanExecute(object? _) => true;

    void System.Windows.Input.ICommand.Execute(object? _) => Action.Invoke();
}
