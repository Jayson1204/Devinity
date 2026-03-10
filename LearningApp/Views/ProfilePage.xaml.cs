using System.Net.Http.Json;
using LearningApp.Services;
using LearningApp.Constants;

namespace LearningApp.Views
{
    public partial class ProfilePage : ContentPage
    {
        private readonly HttpClient _httpClient;
        private bool _settingsExpanded = false;
        private bool _isTogglingTheme = false;

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
            ThemeSwitch.IsToggled = Application.Current.UserAppTheme == AppTheme.Dark;
        }

        private async void LoadProfile()
        {
            SkeletonScroll.IsVisible = true;
            ContentScroll.IsVisible = false;

            FullNameLabel.Text = Preferences.Get("UserFullName", "User");
            EmailLabel.Text = Preferences.Get("UserEmail", "");

            LoadAvatar();
            await LoadStats();

            SkeletonScroll.IsVisible = false;
            ContentScroll.IsVisible = true;
        }

        // ── Avatar ─────────────────────────────────────────────────────────────

        private void LoadAvatar()
        {
            var avatarUrl = Preferences.Get("UserAvatarUrl", "");
            if (!string.IsNullOrEmpty(avatarUrl))
            {
                AvatarImage.Source = ImageSource.FromUri(new Uri(avatarUrl));
                AvatarImage.IsVisible = true;
                AvatarLabel.IsVisible = false;
            }
            else
            {
                AvatarImage.IsVisible = false;
                AvatarLabel.IsVisible = true;
            }
        }

        private async void OnAvatarClicked(object sender, EventArgs e)
        {
            try
            {
                var action = await DisplayActionSheet(
                    "Profile Photo", "Cancel", "Remove Photo",
                    "Choose from Gallery");

                if (action == "Choose from Gallery")
                    await PickAndUploadAvatar();
                else if (action == "Remove Photo")
                    await RemoveAvatar();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Avatar tap error: {ex.Message}");
            }
        }

        private async Task PickAndUploadAvatar()
        {
            try
            {
                var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Pick a profile photo"
                });

                if (result == null) return;

                // Show uploading state
                AvatarLabel.Text = "⏳";
                AvatarLabel.IsVisible = true;
                AvatarImage.IsVisible = false;

                var userId = Preferences.Get("UserId", "");
                if (string.IsNullOrEmpty(userId))
                {
                    await DisplayAlert("Error", "User not found. Please log in again.", "OK");
                    return;
                }
             
                using var stream = await result.OpenReadAsync();
                using var content = new MultipartFormDataContent();
                using var fileContent = new StreamContent(stream);

                fileContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(result.ContentType ?? "image/jpeg");

                content.Add(fileContent, "file", result.FileName);

                // Attach auth token
                var authToken = await SecureStorage.GetAsync("auth_token");
                if (!string.IsNullOrEmpty(authToken))
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

                var response = await _httpClient.PostAsync(
                    $"{AppConfig.BaseUrl}/api/auth/avatar", content);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"Avatar upload failed: {error}");
                    await DisplayAlert("Upload Failed", "Could not upload photo. Try again.", "OK");
                    LoadAvatar(); 
                    return;
                }

                var uploadResult = await response.Content
                    .ReadFromJsonAsync<AvatarUploadResponse>();

                if (uploadResult?.AvatarUrl == null)
                {
                    await DisplayAlert("Error", "Invalid response from server.", "OK");
                    LoadAvatar();
                    return;
                }

                // Save new URL and update UI
                Preferences.Set("UserAvatarUrl", uploadResult.AvatarUrl);
         
                var bustUrl = $"{uploadResult.AvatarUrl}?t={DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
                AvatarImage.Source = ImageSource.FromUri(new Uri(bustUrl));
                AvatarImage.IsVisible = true;
                AvatarLabel.IsVisible = false;

                
                await AvatarImage.ScaleTo(1.08, 120, Easing.CubicOut);
                await AvatarImage.ScaleTo(1.0, 120, Easing.CubicIn);
            }
            catch (PermissionException)
            {
                await DisplayAlert("Permission Required",
                    "Please allow photo access in your device settings.", "OK");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Pick/upload photo error: {ex.Message}");
                await DisplayAlert("Error", "Something went wrong. Please try again.", "OK");
            }
            finally
            {
                // Reset label text in case it was set to ⏳
                AvatarLabel.Text = "👤";
            }
        }

        private async Task RemoveAvatar()
        {
            try
            {
                var authToken = await SecureStorage.GetAsync("auth_token");
                if (!string.IsNullOrEmpty(authToken))
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

                
                await _httpClient.DeleteAsync($"{AppConfig.BaseUrl}/api/auth/avatar");
            }
            catch { }
            finally
            {
                Preferences.Remove("UserAvatarUrl");
                AvatarImage.IsVisible = false;
                AvatarLabel.Text = "👤";
                AvatarLabel.IsVisible = true;
            }
        }

        // ── Stats ──────────────────────────────────────────────────────────────

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

        // ── Settings ───────────────────────────────────────────────────────────

        private void OnSettingsToggled(object sender, EventArgs e)
        {
            _settingsExpanded = !_settingsExpanded;
            SettingsPanel.IsVisible = _settingsExpanded;
            SettingsChevron.Text = _settingsExpanded ? "⌄" : "›";
        }

        private async void OnEditProfileTapped(object sender, EventArgs e)
            => await Navigation.PushAsync(new EditProfilePage());

        private async void OnHelpTapped(object sender, EventArgs e)
            => await DisplayAlert("Help & Support", "Contact us at support@devinity.com", "OK");

        private async void OnLeaderboardTapped(object sender, EventArgs e)
            => await Navigation.PushAsync(new LeaderboardPage());

        private async void OnMeetingsTapped(object sender, EventArgs e)
            => await DisplayAlert("Meetings", "GMEET LINK COMING SOON", "OK");

        // ── Theme toggle ───────────────────────────────────────────────────────

        private void OnThemeToggled(object sender, ToggledEventArgs e)
        {
            if (_isTogglingTheme) return;
            _isTogglingTheme = true;
            Application.Current.UserAppTheme = e.Value ? AppTheme.Dark : AppTheme.Light;
            _isTogglingTheme = false;
        }

        // ── Logout ─────────────────────────────────────────────────────────────

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert(
                "Logout", "Are you sure you want to logout?", "Logout", "Cancel");
            if (!confirm) return;

            // Show loading overlay
            LoadingOverlay.IsVisible = true;

            try
            {
                var refreshToken = await SecureStorage.GetAsync("refresh_token");
                var authToken = await SecureStorage.GetAsync("auth_token");

                if (!string.IsNullOrEmpty(authToken))
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

                if (!string.IsNullOrEmpty(refreshToken))
                    await _httpClient.PostAsJsonAsync(
                        $"{AppConfig.BaseUrl}/api/auth/logout",
                        new { RefreshToken = refreshToken });
            }
            catch { }
            finally
            {
                if (Application.Current is App app)
                    app.StopQuoteTimer();

                Preferences.Remove("UserId");
                Preferences.Remove("UserEmail");
                Preferences.Remove("UserFullName");
                Preferences.Remove("UserAvatarUrl");

                try
                {
                    SecureStorage.Remove("auth_token");
                    SecureStorage.Remove("refresh_token");
                }
                catch { }

                LoadingOverlay.IsVisible = false;
                await Shell.Current.GoToAsync("///LoginPage");
            }
        }

        // ── DTOs ───────────────────────────────────────────────────────────────

        private class AvatarUploadResponse
        {
            public string? AvatarUrl { get; set; }
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