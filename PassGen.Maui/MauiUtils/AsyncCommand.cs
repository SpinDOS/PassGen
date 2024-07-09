using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace PassGen.Maui;

public interface IAsyncCommand : ICommand
{
    Task ExecuteAsync(object parameter);
}

public static class CommandExtensions
{
    public static async Task ExecuteAsyncOrSync(this ICommand command, object parameter)
    {
        if (command is IAsyncCommand asyncCommand)
            await asyncCommand.ExecuteAsync(parameter);
        else if (command is IAsyncRelayCommand asyncRelayCommand)
            await asyncRelayCommand.ExecuteAsync(parameter);
        else
            command.Execute(parameter);
    }
}