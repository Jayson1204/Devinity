using LearningApp.Controls;
using LearningApp.Services;

namespace LearningApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Dark;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());

            window.Created += (_, _) =>
            {
                MotivationalQuoteService.Instance.QuoteReady += OnQuoteReady;
                MotivationalQuoteService.Instance.Start();
            };

            return window;
        }

        public void StartQuoteTimer()
        {
            MotivationalQuoteService.Instance.Start();
            MotivationalQuoteService.Instance.TriggerNow();
        }

        public void StopQuoteTimer()
        {
            MotivationalQuoteService.Instance.Stop();
        }

        private async void OnQuoteReady(MotivationalQuoteService.Quote quote)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                if (Shell.Current?.Navigation is null) return;

                try
                {
                    var popupPage = new QuotePopupPage(quote);
                    await Shell.Current.Navigation.PushModalAsync(popupPage, false);
                }
                catch (Exception)
                {
                    System.Diagnostics.Debug.WriteLine(">>> Popup error:");
                }
            });
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            MotivationalQuoteService.Instance.Stop();
        }

        protected override void OnResume()
        {
            base.OnResume();
            // Only restart timer if user is already logged in
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var token = await SecureStorage.GetAsync("auth_token");
                if (!string.IsNullOrEmpty(token))
                    MotivationalQuoteService.Instance.Start();
            });
        }
    }
}