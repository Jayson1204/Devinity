using LearningApp.Services;
using LearningApp.Constants;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace LearningApp.Views
{
    // ── Display model ──
    public class CourseProgressItem
    {
        public string Category { get; set; }
        public int TotalVideos { get; set; }
        public int WatchedVideos { get; set; }
        public int TotalAssessments { get; set; }
        public int CompletedAssessments { get; set; }
        public double Percentage { get; set; }

        public string Emoji => Category switch
        {
            "PHP" => "🐘",
            "Python" => "🐍",
            "JavaScript" => "⚡",
            "Java" => "☕",
            "C#" => "C#",
            "C++" => "⚙️",
            "C" => "🔧",
            "MySQL" => "🗄️",
            _ => "📘"
        };

        public string BgColor => Category switch
        {
            "PHP" => "#7B68EE",
            "Python" => "#3776AB",
            "JavaScript" => "#F7DF1E",
            "Java" => "#ED8B00",
            "C#" => "#020202",
            "C++" => "#004482",
            "C" => "#555555",
            "MySQL" => "#4479A1",
            _ => "#6C5CE7"
        };

        public string ProgressDetail =>
            $"{WatchedVideos}/{TotalVideos} videos · {CompletedAssessments}/{TotalAssessments} assessments";
        public string PercentageText => $"{Percentage:F0} %";
        public double ProgressValue => Percentage / 100;
        public string HoursEstimate => $"⏱ {Math.Round(TotalVideos * 0.5, 0)}h estimated";
    }

    // ── ViewModel ──
    public class MyLearningViewModel : Microsoft.Maui.Controls.BindableObject
    {
        private ObservableCollection<CourseProgressItem> _filteredCourses = new();
        public ObservableCollection<CourseProgressItem> FilteredCourses
        {
            get => _filteredCourses;
            set { _filteredCourses = value; OnPropertyChanged(); }
        }

        public ICommand CourseTappedCommand { get; }

        public MyLearningViewModel(Action<CourseProgressItem> onCourseTapped)
        {
            CourseTappedCommand = new Command<CourseProgressItem>(onCourseTapped);
        }

        public void SetCourses(IEnumerable<CourseProgressItem> courses)
            => FilteredCourses = new ObservableCollection<CourseProgressItem>(courses);
    }

    // ── Page ──
    public partial class MyLearningPage : ContentPage
    {
        private readonly HttpClient _httpClient;
        private readonly MyLearningViewModel _viewModel;
        private List<CourseProgressItem> _allCourses = new();
        private bool _showingInProgress = true;

        public class OverviewResponse
        {
            public int TotalVideosWatched { get; set; }
            public int TotalAssessmentsCompleted { get; set; }
            public List<CourseProgressItem> CourseProgress { get; set; }
        }

        public MyLearningPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            _httpClient = ApiClient.Instance;
            _viewModel = new MyLearningViewModel(OnCourseTapped);
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadProgress();
        }

        private async void LoadProgress()
        {
            SkeletonScroll.IsVisible = true;
            CourseCollectionView.IsVisible = false;

            try
            {
                var userId = Preferences.Get("UserId", "");
                if (string.IsNullOrEmpty(userId))
                {
                    _allCourses = new();
                    RenderCourses();
                    return;
                }

                var url = $"{AppConfig.BaseUrl}/api/learning/{Uri.EscapeDataString(userId)}/overview";
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                var overview = System.Text.Json.JsonSerializer.Deserialize<OverviewResponse>(
                    content,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (overview == null) { _allCourses = new(); RenderCourses(); return; }

                _allCourses = overview.CourseProgress?
                    .Where(c => c.TotalVideos > 0 || c.TotalAssessments > 0)
                    .ToList() ?? new();

                EnrolledLabel.Text = $"{_allCourses.Count} courses";
                TotalHoursLabel.Text = $"{Math.Round(_allCourses.Sum(c => c.TotalVideos) * 0.5, 0)}h";

                RenderCourses();
            }
            catch
            {
                await DisplayAlert("Error", "Failed to load progress.", "OK");
            }
            finally
            {
                SkeletonScroll.IsVisible = false;
                CourseCollectionView.IsVisible = true;
            }
        }

        private void RenderCourses()
        {
            var filtered = _showingInProgress
                ? _allCourses.Where(c => c.Percentage < 100 && c.Percentage > 0)
                             .OrderByDescending(c => c.Percentage)
                : _allCourses.Where(c => c.Percentage >= 100)
                             .OrderByDescending(c => c.Percentage);

            _viewModel.SetCourses(filtered);
        }

        private async void OnCourseTapped(CourseProgressItem course)
            => await Navigation.PushAsync(new CourseDetailPage(course.Category));

        private void OnInProgressTapped(object sender, EventArgs e)
        {
            _showingInProgress = true;
            InProgressTab.BackgroundColor = Colors.White;
            ((Label)InProgressTab.Content).FontAttributes = FontAttributes.Bold;
            ((Label)InProgressTab.Content).TextColor = Color.FromArgb("#1A1A1A");
            CompletedTab.BackgroundColor = Colors.Transparent;
            ((Label)CompletedTab.Content).FontAttributes = FontAttributes.None;
            ((Label)CompletedTab.Content).TextColor = Color.FromArgb("#888888");
            RenderCourses();
        }

        private void OnCompletedTapped(object sender, EventArgs e)
        {
            _showingInProgress = false;
            CompletedTab.BackgroundColor = Colors.White;
            ((Label)CompletedTab.Content).FontAttributes = FontAttributes.Bold;
            ((Label)CompletedTab.Content).TextColor = Color.FromArgb("#1A1A1A");
            InProgressTab.BackgroundColor = Colors.Transparent;
            ((Label)InProgressTab.Content).FontAttributes = FontAttributes.None;
            ((Label)InProgressTab.Content).TextColor = Color.FromArgb("#888888");
            RenderCourses();
        }


       
    }
}