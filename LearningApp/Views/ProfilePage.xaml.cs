using LearningApp.Services;
using System.Net.Http.Json;
using LearningApp.Constants;

namespace LearningApp.Views
{
    public partial class ProfilePage : ContentPage
    {
        private readonly HttpClient _httpClient;
        private bool _settingsExpanded = false;

        public ProfilePage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            _httpClient = ApiClient.Instance;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadProfile();
        }

        private async void LoadProfile()
        {
            SkeletonScroll.IsVisible = true;
            ContentScroll.IsVisible = false;

            FullNameLabel.Text = Preferences.Get("UserFullName", "User");
            EmailLabel.Text = Preferences.Get("UserEmail", "");

            await LoadStats();

            SkeletonScroll.IsVisible = false;
            ContentScroll.IsVisible = true;
        }

        private async Task LoadStats()
        {
            try
            {
                var userId = Preferences.Get("UserId", "");
                if (string.IsNullOrEmpty(userId)) return;

                var url = $"{AppConfig.BaseUrl}/api/learning/{Uri.EscapeDataString(userId)}/overview";
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                var overview = System.Text.Json.JsonSerializer.Deserialize<OverviewResponse>(
                    content,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (overview == null) return;

                var courses = overview.CourseProgress ?? new();
                int completed = courses.Count(c => c.Percentage >= 100);
                double hours = Math.Round(courses.Sum(c => c.WatchedVideos) * 0.5, 0);

                CoursesCompletedLabel.Text = completed.ToString();
                TotalHoursLabel.Text = hours.ToString();
                CertificatesLabel.Text = completed.ToString();
            }
            catch { }
        }

        // ── Settings Accordion ────────────────────────────────────────

        private void OnSettingsToggled(object sender, EventArgs e)
        {
            _settingsExpanded = !_settingsExpanded;
            SettingsPanel.IsVisible = _settingsExpanded;
            SettingsChevron.Text = _settingsExpanded ? "⌄" : "›";
        }

        private async void OnEditProfileTapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditProfilePage());
        }

        private async void OnPrivacyTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Privacy & Security", "Coming soon.", "OK");
        }

        private async void OnHelpTapped(object sender, EventArgs e)
        {
            await DisplayAlert("Help & Support", "Contact us at support@devinity.com", "OK");
        }

        // ── Logout ────────────────────────────────────────────────────

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Logout", "Are you sure you want to logout?", "Logout", "Cancel");
            if (!confirm) return;

            try
            {
                var refreshToken = await SecureStorage.GetAsync("refresh_token");
                var authToken = await SecureStorage.GetAsync("auth_token");

                if (!string.IsNullOrEmpty(authToken))
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    await _httpClient.PostAsJsonAsync(
                        $"{AppConfig.BaseUrl}/api/auth/logout",
                        new { RefreshToken = refreshToken });
                }
            }
            catch { }
            finally
            {
                Preferences.Remove("UserId");
                Preferences.Remove("UserEmail");
                Preferences.Remove("UserFullName");

                try
                {
                    SecureStorage.Remove("auth_token");
                    SecureStorage.Remove("refresh_token");
                }
                catch { }

                await Shell.Current.GoToAsync("///LoginPage");
            }
        }

        // ── Models ────────────────────────────────────────────────────

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