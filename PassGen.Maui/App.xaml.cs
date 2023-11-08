using PassGen.Maui.Service;

namespace PassGen.Maui;

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

		// MainPage = new AppShell();
	}
}
