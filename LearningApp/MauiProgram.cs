using CommunityToolkit.Maui;
using LearningApp.Services;
using Microsoft.Extensions.Logging;          // ← new
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace LearningApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // ← new: must run before any PdfSharpCore type is touched

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif
            builder.Services.AddSingleton<AuthService>();
            return builder.Build();
        }
    }
}