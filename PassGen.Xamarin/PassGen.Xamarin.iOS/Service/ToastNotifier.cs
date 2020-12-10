using System;
using GlobalToast;
using PassGen.Xamarin.Service;

namespace PassGen.Xamarin.iOS.Service
{
    public sealed class ToastNotifier : IToastNotifier
    {
        private static readonly TimeSpan Duration = TimeSpan.FromSeconds(2);
        
        public void ShowToast(string message)
        {
            Toast.MakeToast(message)
                .SetDuration(Duration.TotalMilliseconds)
                .Show();
        }
    }
}