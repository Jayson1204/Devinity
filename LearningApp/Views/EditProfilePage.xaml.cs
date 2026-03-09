using System.Net.Http.Json;
using LearningApp.Services;
using LearningApp.Constants;

namespace LearningApp.Views;

public partial class EditProfilePage : ContentPage
{
    private readonly HttpClient _httpClient;
    private bool _showCurrentPw = false;
    private bool _showNewPw = false;
    private bool _showConfirmPw = false;
    private string? _pendingImagePath = null;

    public EditProfilePage()
    {
        InitializeComponent();
        _httpClient = ApiClient.Instance;
        LoadCurrentValues();
    }

    private void LoadCurrentValues()
    {
        FullNameEntry.Text = Preferences.Get("UserFullName", "");

        var avatarUrl = Preferences.Get("UserAvatarUrl", "");
        if (!string.IsNullOrEmpty(avatarUrl))
        {
            AvatarImage.Source = ImageSource.FromUri(new Uri(avatarUrl));
            AvatarImage.IsVisible = true;
            AvatarEmoji.IsVisible = false;
        }
    }

    private async void OnChangeAvatarTapped(object sender, EventArgs e)
    {
        try
        {
            var result = await MediaPicker.Default.PickPhotoAsync(
                new MediaPickerOptions { Title = "Choose a profile photo" });

            if (result == null) return;

            var destPath = Path.Combine(
                FileSystem.CacheDirectory,
                $"avatar_pending{Path.GetExtension(result.FileName)}");

            using var src = await result.OpenReadAsync();
            using var dest = File.OpenWrite(destPath);
            await src.CopyToAsync(dest);

            _pendingImagePath = destPath;

            AvatarImage.Source = ImageSource.FromFile(destPath);
            AvatarImage.IsVisible = true;
            AvatarEmoji.IsVisible = false;
        }
        catch
        {
            await DisplayAlert("Error", "Could not open photo picker.", "OK");
        }
    }

    private void OnToggleCurrentPassword(object sender, EventArgs e)
    {
        _showCurrentPw = !_showCurrentPw;
        CurrentPasswordEntry.IsPassword = !_showCurrentPw;
        ShowCurrentPassword.Text = _showCurrentPw ? "🙈" : "👁";
    }

    private void OnToggleNewPassword(object sender, EventArgs e)
    {
        _showNewPw = !_showNewPw;
        NewPasswordEntry.IsPassword = !_showNewPw;
        ShowNewPassword.Text = _showNewPw ? "🙈" : "👁";
    }

    private void OnToggleConfirmPassword(object sender, EventArgs e)
    {
        _showConfirmPw = !_showConfirmPw;
        ConfirmPasswordEntry.IsPassword = !_showConfirmPw;
        ShowConfirmPassword.Text = _showConfirmPw ? "🙈" : "👁";
    }

    private async void OnSaveTapped(object sender, EventArgs e)
    {
        HideAllErrors();

        var fullName = FullNameEntry.Text?.Trim() ?? "";
        var currentPw = CurrentPasswordEntry.Text ?? "";
        var newPw = NewPasswordEntry.Text ?? "";
        var confirmPw = ConfirmPasswordEntry.Text ?? "";
        bool hasError = false;

        if (string.IsNullOrEmpty(fullName))
        {
            FullNameError.Text = "Full name is required.";
            FullNameError.IsVisible = true;
            hasError = true;
        }

        bool changingPassword = !string.IsNullOrEmpty(newPw) || !string.IsNullOrEmpty(confirmPw);

        if (changingPassword)
        {
            if (string.IsNullOrEmpty(currentPw))
            {
                CurrentPasswordError.Text = "Current password is required.";
                CurrentPasswordError.IsVisible = true;
                hasError = true;
            }
            if (newPw.Length < 6)
            {
                NewPasswordError.Text = "New password must be at least 6 characters.";
                NewPasswordError.IsVisible = true;
                hasError = true;
            }
            if (newPw != confirmPw)
            {
                ConfirmPasswordError.Text = "Passwords do not match.";
                ConfirmPasswordError.IsVisible = true;
                hasError = true;
            }
        }

        if (hasError) return;

        SaveBtn.IsEnabled = false;
        SaveBtn.Text = "Saving...";

        try
        {
            var authToken = await SecureStorage.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(authToken))
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            // 1. Upload avatar if picked
            if (!string.IsNullOrEmpty(_pendingImagePath) && File.Exists(_pendingImagePath))
            {
                SaveBtn.Text = "Uploading photo...";

                using var stream = File.OpenRead(_pendingImagePath);
                using var content = new MultipartFormDataContent();
                using var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                content.Add(fileContent, "file", Path.GetFileName(_pendingImagePath));

                var avatarResp = await _httpClient.PostAsync(
                    $"{AppConfig.BaseUrl}/api/auth/avatar", content);

                if (avatarResp.IsSuccessStatusCode)
                {
                    var avatarResult = await avatarResp.Content
                        .ReadFromJsonAsync<UploadAvatarResult>();

                    if (avatarResult?.Success == true && avatarResult.AvatarUrl != null)
                    {
                        Preferences.Set("UserAvatarUrl", avatarResult.AvatarUrl);
                        try { File.Delete(_pendingImagePath); } catch { }
                        _pendingImagePath = null;
                    }
                }
            }

            // 2. Update profile name / password
            SaveBtn.Text = "Saving...";

            var response = await _httpClient.PutAsJsonAsync(
                $"{AppConfig.BaseUrl}/api/auth/profile",
                new
                {
                    FullName = fullName,
                    CurrentPassword = changingPassword ? currentPw : (string?)null,
                    NewPassword = changingPassword ? newPw : (string?)null
                });

            if (!response.IsSuccessStatusCode)
            {
                ShowError("Failed to update profile. Please try again.");
                return;
            }

            Preferences.Set("UserFullName", fullName);

            SuccessBanner.IsVisible = true;
            ErrorBanner.IsVisible = false;

            CurrentPasswordEntry.Text = string.Empty;
            NewPasswordEntry.Text = string.Empty;
            ConfirmPasswordEntry.Text = string.Empty;

            await Task.Delay(1500);
            await Navigation.PopAsync();
        }
        catch
        {
            ShowError("Network error. Please check your connection.");
        }
        finally
        {
            SaveBtn.IsEnabled = true;
            SaveBtn.Text = "Save Changes";
        }
    }

    private void ShowError(string message)
    {
        ErrorBannerText.Text = message;
        ErrorBanner.IsVisible = true;
        SuccessBanner.IsVisible = false;
    }

    private void HideAllErrors()
    {
        FullNameError.IsVisible = false;
        CurrentPasswordError.IsVisible = false;
        NewPasswordError.IsVisible = false;
        ConfirmPasswordError.IsVisible = false;
        ErrorBanner.IsVisible = false;
        SuccessBanner.IsVisible = false;
    }

    private async void OnBackTapped(object sender, EventArgs e)
        => await Navigation.PopAsync();

    private class UploadAvatarResult
    {
        public bool Success { get; set; }
        public string? AvatarUrl { get; set; }
    }
}