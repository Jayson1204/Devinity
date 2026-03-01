using CommunityToolkit.Maui.Views;

namespace LearningApp.Views
{
    public partial class VideoPlayerPage : ContentPage
    {
        public VideoPlayerPage(string videoUrl, string title, string duration)
        {
            InitializeComponent();

            VideoTitleLabel.Text = title;
            DurationLabel.Text = duration;

            // Set the Firebase direct video URL
            VideoPlayer.Source = MediaSource.FromUri(videoUrl);
        }

        private async void OnCloseClicked(object sender, EventArgs e)
        {
            // Stop video before closing to release resources
            VideoPlayer.Stop();
            await Navigation.PopModalAsync();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            VideoPlayer.Stop();
        }
    }
}