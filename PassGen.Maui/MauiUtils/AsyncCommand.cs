using System.Windows.Input;

namespace PassGen.Maui;

public interface IAsyncCommand : ICommand {
    Task ExecuteAsync(object parameter);
}

public static class CommandExtensions {
    public static async Task ExecuteAsyncOrSync(this ICommand command, object parameter) 
    {
        if (command is IAsyncCommand asyncCommand)
            await asyncCommand.ExecuteAsync(parameter);
        else
            command.Execute(parameter);
    }
}

public class AsyncCommand : IAsyncCommand
{
    private readonly WeakEventManager _weakEventManager = new WeakEventManager();
    private readonly Func<object, Task> execute_;
    private readonly Func<object, bool> canExecute_;

    public AsyncCommand(Func<object, Task> execute, Func<object, bool> canExecute) {
        execute_ = execute ?? throw new ArgumentNullException(nameof(execute));
        canExecute_ = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
    }

    public AsyncCommand(Func<object, Task> execute) : this(execute, parameter => true) {}

    public AsyncCommand(Func<Task> execute, Func<bool> canExecute) 
        : this(execute != null ? parameter => execute() : null, canExecute != null ? parameter => canExecute() : null) {}

    public AsyncCommand(Func<Task> execute) : this(execute, () => true) {}

    public event EventHandler CanExecuteChanged {
        add => _weakEventManager.AddEventHandler(value, nameof(CanExecuteChanged));
        remove => _weakEventManager.RemoveEventHandler(value, nameof(CanExecuteChanged));
    }

    public bool CanExecute(object parameter) => canExecute_.Invoke(parameter);
    public void Execute(object parameter) => ExecuteAsync(parameter);
    public Task ExecuteAsync(object parameter) => execute_.Invoke(parameter);
    public void ChangeCanExecute() => _weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
}