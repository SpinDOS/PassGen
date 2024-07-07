namespace PassGen.Maui;

public interface IToastNotifier
{
    void ShowToast(string message);
}

public sealed class ToastNotifier : IToastNotifier
{
    public void ShowToast(string message) {}
        // Toast.MakeText(Application.Context, message, ToastLength.Short)?.Show();
}