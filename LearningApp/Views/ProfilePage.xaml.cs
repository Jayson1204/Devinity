using System.Net.Http.Json;

namespace LearningApp.Views
{
    public partial class ProfilePage : ContentPage
    {
        private const string BaseUrl = "NGROK_URL";
        private readonly HttpClient _httpClient;

        public ProfilePage()
        {
            InitializeComponent();
            _httpClient = new HttpClient();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadProfile();
        }

        private async void LoadProfile()
        {
            FullNameLabel.Text = Preferences.Get("UserFullName", "User");
            EmailLabel.Text = Preferences.Get("UserEmail", "");
            await LoadStats();
        }

        private async Task LoadStats()
        {
            try
            {
                var userId = Preferences.Get("UserId", "");
                if (string.IsNullOrEmpty(userId)) return;

                var url = $"{BaseUrl}/api/learning/{Uri.EscapeDataString(userId)}/overview";
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                var overview = System.Text.Json.JsonSerializer.Deserialize<OverviewResponse>(
                    content,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (overview == null) return;

                var courses = overview.CourseProgress ?? new();
                int completed = courses.Count(c => c.Percentage >= 100);
                double hours = Math.Round(courses.Sum(c => c.TotalVideos) * 0.5, 0);

                CoursesCompletedLabel.Text = completed.ToString();
                TotalHoursLabel.Text = hours.ToString();
                CertificatesLabel.Text = completed.ToString();
            }
            catch { }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Logout", "Are you sure you want to logout?", "Logout", "Cancel");
            if (!confirm) return;

            try
            {
                // Step 1 — Revoke refresh token on server
                var refreshToken = await SecureStorage.GetAsync("refresh_token");
                var authToken = await SecureStorage.GetAsync("auth_token");

                if (!string.IsNullOrEmpty(authToken))
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    await _httpClient.PostAsJsonAsync(
                        $"{BaseUrl}/api/auth/logout",
                        new { RefreshToken = refreshToken });
                }
            }
            catch { /* still proceed with local logout even if server call fails */ }
            finally
            {
                // Step 2 — Clear all local data regardless of server response
                Preferences.Remove("UserId");
                Preferences.Remove("UserEmail");
                Preferences.Remove("UserFullName");

                try
                {
                    SecureStorage.Remove("auth_token");
                    SecureStorage.Remove("refresh_token");
                }
                catch { }

                // Step 3 — Navigate to login
                await Shell.Current.GoToAsync("///LoginPage");
            }
        }

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
    }
}