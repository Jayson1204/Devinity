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
            builder.Services.AddSingleton<ThemeService>();

#if ANDROID
            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
                // This removes the native border/underline on Android
                handler.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
                handler.PlatformView.Background = null;
            });
#endif
            return builder.Build();
        }
    }
}