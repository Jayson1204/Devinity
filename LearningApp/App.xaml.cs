using LearningApp.Controls;
using LearningApp.Services;

namespace LearningApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
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

        /// <summary>
        /// Call after successful login to show the first quote and start the timer.
        /// </summary>
        public void StartQuoteTimer()
        {
            MotivationalQuoteService.Instance.Start();
            MotivationalQuoteService.Instance.TriggerNow();
        }

        /// <summary>
        /// Call on logout to stop quote popups.
        /// </summary>
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
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($">>> Popup error: {ex.Message}");
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