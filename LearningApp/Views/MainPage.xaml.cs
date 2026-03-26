namespace LearningApp.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var name = Preferences.Get("UserFullName", "");
            if (!string.IsNullOrEmpty(name))
                WelcomeLabel.Text = $"Welcome back, {name.Split(' ')[0]}!";

            SkeletonGrid.IsVisible = true;
            CategoryGrid.IsVisible = false;
            FeaturedSkeleton.IsVisible = true;
            FeaturedCourse.IsVisible = false;

            await Task.Delay(600);

            SkeletonGrid.IsVisible = false;
            CategoryGrid.IsVisible = true;
            FeaturedSkeleton.IsVisible = false;
            FeaturedCourse.IsVisible = true;
        }

        private async void OnCategoryTapped(object sender, TappedEventArgs e)
        {
            var border = (Border)sender;
            var courseName = e.Parameter.ToString();
            _ = border.ScaleTo(0.95, 50).ContinueWith(_ => border.ScaleTo(1.0, 50));
            await Navigation.PushAsync(new CourseDetailPage(courseName));
        }

        private async void OnFeaturedCourseTapped(object sender, TappedEventArgs e)
        {
            var border = (Border)sender;
            var courseName = e.Parameter.ToString(); // ← same as OnCategoryTapped
            _ = border.ScaleTo(0.98, 50).ContinueWith(_ => border.ScaleTo(1.0, 50));
            await Navigation.PushAsync(new CourseDetailPage(courseName));
        }

        private async void OnHomeTapped(object sender, EventArgs e) { }




    }
}