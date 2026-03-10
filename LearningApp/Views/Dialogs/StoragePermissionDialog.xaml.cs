namespace LearningApp.Views.Dialogs
{
    public partial class StoragePermissionDialog : ContentPage
    {
        private readonly TaskCompletionSource<bool> _tcs = new();
        public Task<bool> Result => _tcs.Task;

        public StoragePermissionDialog()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await DialogCard.TranslateTo(0, 0, 380, Easing.CubicOut);
        }

        private async void OnAllowTapped(object sender, EventArgs e)
        {
            await DialogCard.ScaleTo(0.98, 80, Easing.CubicOut);
            await DialogCard.ScaleTo(1.0, 80, Easing.CubicIn);
            await DismissAsync(true);
        }

        private async void OnDenyTapped(object sender, EventArgs e)
            => await DismissAsync(false);

        private async void OnBackdropTapped(object sender, EventArgs e)
            => await DismissAsync(false);

        private async Task DismissAsync(bool confirmed)
        {
            await DialogCard.TranslateTo(0, 500, 300, Easing.CubicIn);
            _tcs.TrySetResult(confirmed);
            await Shell.Current.Navigation.PopModalAsync(false);
        }

        protected override bool OnBackButtonPressed()
        {
            DismissAsync(false);
            return true;
        }
    }
}