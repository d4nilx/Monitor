using Microsoft.Extensions.Logging;
using Monitor.Core.Interfaces;
using Monitor.Maui.ViewModel;

#if MACCATALYST
using Monitor.Maui.Platforms.MacCatalyst;
#endif

namespace Monitor.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        // Crossplatform dependency injection for IProcessService
#if MACCATALYST
        // if it's on macOS
        builder.Services.AddSingleton<Monitor.Core.Interfaces.IProcessService, Monitor.Maui.Platforms.MacCatalyst.MacProcessService>();
#elif WINDOWS
        // if it's on Windows
        builder.Services.AddSingleton<Monitor.Core.Interfaces.IProcessService, Monitor.Maui.Platforms.Windows.WindowsProcessService>();
#endif

        builder.Services.AddTransient<ProcessListViewModel>();
        builder.Services.AddTransient<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
