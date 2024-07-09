using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace PassGen.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiCommunityToolkit()
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		var mainPageViewModel = new MainPageViewModel(new SaltStorage(), new PasswordGeneratorAdapter());
		Task.Run(() => mainPageViewModel.LoadDataAsync()).Wait();

		builder.Services.AddTransient<MainPage>(_ => new MainPage(mainPageViewModel));

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
