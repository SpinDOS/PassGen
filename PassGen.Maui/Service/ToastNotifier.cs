using CommunityToolkit.Maui.Alerts;

namespace PassGen.Maui.Service;

public interface IToastNotifier
{
    Task ShowToast(string message);
}

public sealed class ToastNotifier : IToastNotifier
{
    public Task ShowToast(string message) => Toast.Make(message).Show();
}