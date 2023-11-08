using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls.Compatibility.Hosting;

namespace PassGen.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiCompatibility()
			.UseMauiCommunityToolkit()
			.UseMauiApp<App>();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
