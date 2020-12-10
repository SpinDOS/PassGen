using Android.App;
using Android.Widget;
using PassGen.Xamarin.Service;

namespace PassGen.Xamarin.Android.Service
{
    public sealed class ToastNotifier : IToastNotifier
    {
        public void ShowToast(string message) => 
            Toast.MakeText(Application.Context, message, ToastLength.Short)?.Show();
    }
}