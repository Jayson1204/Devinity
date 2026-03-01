using Android.App;
using Android.Content.PM;
using Android.OS;
// We keep this using, but we must be specific below
using Android.Webkit;

namespace LearningApp; // Make sure your namespace is here!

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.LayoutDirection | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        // Fix: Use the fully qualified name to avoid ambiguity with Microsoft.Maui.Controls.WebView
        Android.Webkit.WebView.SetWebContentsDebuggingEnabled(true);
    }
}