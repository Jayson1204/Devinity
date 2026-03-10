using Microsoft.Maui.ApplicationModel;

namespace LearningApp.Views.Dialogs
{
    public partial class StoragePermissionDialog : ContentPage
    {
        // Caller awaits this to know the result
        private readonly TaskCompletionSource<bool> _tcs = new();
        public Task<bool> Result => _tcs.Task;

        public StoragePermissionDialog()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Slide-up entrance
            await DialogCard.TranslateTo(0, 0, 380, Easing.CubicOut);
        }

        private async void OnAllowTapped(object sender, EventArgs e)
        {
            // Animate button press
            await DialogCard.ScaleTo(0.98, 80, Easing.CubicOut);
            await DialogCard.ScaleTo(1.0, 80, Easing.CubicIn);

#if ANDROID
            var status = await Permissions.RequestAsync<Permissions.StorageRead>();

            // Android 13+ uses READ_MEDIA_IMAGES instead of READ_EXTERNAL_STORAGE
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.Media>();

            await DismissAsync(status == PermissionStatus.Granted);
#else
            await DismissAsync(true);
#endif
        }

        private async void OnDenyTapped(object sender, EventArgs e)
        {
            await DismissAsync(false);
        }

        private async void OnBackdropTapped(object sender, EventArgs e)
        {
            await DismissAsync(false);
        }

        private async Task DismissAsync(bool granted)
        {
            // Slide back down
            await DialogCard.TranslateTo(0, 500, 300, Easing.CubicIn);
            _tcs.TrySetResult(granted);
            await Navigation.PopModalAsync(false);
        }

        // Intercept Android back button
        protected override bool OnBackButtonPressed()
        {
            DismissAsync(false);
            return true;
        }
    }
}