using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

namespace PassGen.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiCommunityToolkit(options =>
			{
				options.SetShouldEnableSnackbarOnWindows(true);
			})
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.AddTransient<MainPage>(_ => new MainPage(
			new SaltStorage(), new PasswordGeneratorAdapter())
		);

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
