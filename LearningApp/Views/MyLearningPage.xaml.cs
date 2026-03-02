using System.Net.Http.Json;

namespace LearningApp.Views
{
    public partial class MyLearningPage : ContentPage
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "NGROK_URL";
        private List<CourseProgressItem> _allCourses = new();
        private bool _showingInProgress = true;

        public class CourseProgressItem
        {
            public string Category { get; set; }
            public int TotalVideos { get; set; }
            public int WatchedVideos { get; set; }
            public int TotalAssessments { get; set; }
            public int CompletedAssessments { get; set; }
            public double Percentage { get; set; }
        }

        public class OverviewResponse
        {
            public int TotalVideosWatched { get; set; }
            public int TotalAssessmentsCompleted { get; set; }
            public List<CourseProgressItem> CourseProgress { get; set; }
        }

        public MyLearningPage()
        {
            InitializeComponent();
            _httpClient = new HttpClient();           
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadProgress();
        }

        private async void LoadProgress()
        {
            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;
                CourseListContainer.Children.Clear();
                EmptyState.IsVisible = false;

                var userId = Preferences.Get("UserId", "");
                if (string.IsNullOrEmpty(userId))
                {
                    EmptyState.IsVisible = true;
                    return;
                }

                var url = $"{BaseUrl}/api/learning/{Uri.EscapeDataString(userId)}/overview";
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                var overview = System.Text.Json.JsonSerializer.Deserialize<OverviewResponse>(
                    content,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (overview == null) { ShowEmpty(); return; }

                // Header stats
                _allCourses = overview.CourseProgress?
                    .Where(c => c.TotalVideos > 0 || c.TotalAssessments > 0)
                    .ToList() ?? new();

                EnrolledLabel.Text = $"{_allCourses.Count} courses";

                // Estimate total hours (avg 30 min per video)
                var totalVideos = _allCourses.Sum(c => c.TotalVideos);
                var totalHours = Math.Round(totalVideos * 0.5, 0);
                TotalHoursLabel.Text = $"{totalHours}h";

                RenderCourses();
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Failed to load", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private void RenderCourses()
        {
            CourseListContainer.Children.Clear();
            EmptyState.IsVisible = false;

            var filtered = _showingInProgress
                ? _allCourses.Where(c => c.Percentage < 100 && c.Percentage > 0).ToList()
                : _allCourses.Where(c => c.Percentage >= 100).ToList();

            if (filtered.Count == 0)
            {
                ShowEmpty();
                return;
            }

            foreach (var course in filtered.OrderByDescending(c => c.Percentage))
                CourseListContainer.Children.Add(CreateCourseCard(course));
        }

        private void ShowEmpty()
        {
            CourseListContainer.Children.Clear();
            EmptyState.IsVisible = true;
            CourseListContainer.Children.Add(EmptyState);
        }

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

        private View CreateCourseCard(CourseProgressItem course)
        {
            var emoji = course.Category switch
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

            var bgColor = course.Category switch
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

            var card = new Border
            {
                BackgroundColor = Colors.White,
                StrokeThickness = 0,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 }
            };

            var cardContent = new VerticalStackLayout { Spacing = 0 };

            // Course banner (colored with emoji)
            var banner = new Border
            {
                BackgroundColor = Color.FromArgb(bgColor),
                StrokeThickness = 0,
                HeightRequest = 140,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 16 }
            };

            var bannerContent = new Grid();
            bannerContent.Children.Add(new Label
            {
                Text = emoji,
                FontSize = 64,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            });
            banner.Content = bannerContent;
            cardContent.Children.Add(banner);

            // Card info
            var info = new VerticalStackLayout
            {
                Padding = new Thickness(16, 12),
                Spacing = 8
            };

            info.Children.Add(new Label
            {
                Text = course.Category,
                FontSize = 17,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#1A1A1A")
            });

            info.Children.Add(new Label
            {
                Text = $"{course.WatchedVideos}/{course.TotalVideos} videos · {course.CompletedAssessments}/{course.TotalAssessments} assessments",
                FontSize = 12,
                TextColor = Colors.Gray
            });

            // Progress row
            var progressRow = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                RowDefinitions =
                {
                    new RowDefinition { Height = GridLength.Auto },
                    new RowDefinition { Height = GridLength.Auto }
                },
                RowSpacing = 6
            };

            progressRow.Children.Add(new Label
            {
                Text = "Progress",
                FontSize = 12,
                TextColor = Colors.Gray,
                VerticalOptions = LayoutOptions.Center
            });

            var pctLabel = new Label
            {
                Text = $"{course.Percentage:F0} %",
                FontSize = 13,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#1A1A1A"),
                HorizontalOptions = LayoutOptions.End
            };
            Grid.SetColumn(pctLabel, 1);
            progressRow.Children.Add(pctLabel);

            var progressBar = new ProgressBar
            {
                Progress = course.Percentage / 100,
                ProgressColor = Color.FromArgb("#1A1A1A"),
                BackgroundColor = Color.FromArgb("#E8E8E8"),
                HeightRequest = 6
            };
            Grid.SetRow(progressBar, 1);
            Grid.SetColumnSpan(progressBar, 2);
            progressRow.Children.Add(progressBar);

            info.Children.Add(progressRow);

            // Hours estimate
            var hours = Math.Round(course.TotalVideos * 0.5, 0);
            info.Children.Add(new Label
            {
                Text = $"⏱ {hours}h estimated",
                FontSize = 12,
                TextColor = Colors.Gray
            });

            cardContent.Children.Add(info);
            card.Content = cardContent;

            // Tap to navigate to course
            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
                await Navigation.PushAsync(new CourseDetailPage(course.Category));
            card.GestureRecognizers.Add(tap);

            return card;
        }
    }
}