using System;
using PassGen.Xamarin.Service;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace PassGen.Xamarin
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            
            DependencyService.Register<ISaltStorage, SaltStorage>();
            DependencyService.Register<IPasswordGenerator, PasswordGeneratorAdapter>();

            MainPage = new MainPage(
                DependencyService.Resolve<ISaltStorage>(DependencyFetchTarget.NewInstance),
                DependencyService.Resolve<IPasswordGenerator>(DependencyFetchTarget.NewInstance),
                DependencyService.Resolve<IToastNotifier>(DependencyFetchTarget.NewInstance));
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}