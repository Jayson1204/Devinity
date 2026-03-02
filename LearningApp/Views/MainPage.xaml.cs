namespace LearningApp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private async void OnHomeTapped(object sender, EventArgs e)
        {
            
            await DisplayAlert("Navigation", "You are already on Home!", "OK");
        }
        private async void OnCategoryTapped(object sender, TappedEventArgs e)
        {
            var border = (Border)sender;
            var courseName = e.Parameter.ToString();

            
            await border.ScaleTo(0.95, 50);
            await border.ScaleTo(1, 50);

            
           await Navigation.PushAsync(new CourseDetailPage(courseName));
        }

        private async void OnFeaturedCourseTapped(object sender, TappedEventArgs e)
        {
            var border = (Border)sender;

            
            await border.ScaleTo(0.98, 50);
            await border.ScaleTo(1, 50);

            
            await Navigation.PushAsync(new CourseDetailPage("Web Development"));
        }
        private void OnHomeTabTapped(object sender, EventArgs e)
        {
            
            DisplayAlert("Navigation", "Already on Home", "OK");
        }

        private async void OnMyLearningTabTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MyLearningPage ());           
        }
        private async void OnProfileTabTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage ());          
        }
    }
}