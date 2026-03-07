using LearningApp.Services;
using LearningApp.Constants;
using System.Net.Http.Json;

namespace LearningApp.Views;

public partial class EditProfilePage : ContentPage
{
    private readonly HttpClient _httpClient;
    private bool _showCurrentPw = false;
    private bool _showNewPw = false;
    private bool _showConfirmPw = false;
    private string? _selectedImagePath = null;

    public EditProfilePage()
    {
        InitializeComponent();
        _httpClient = ApiClient.Instance;
        LoadCurrentValues();
    }

    private void LoadCurrentValues()
    {
        FullNameEntry.Text = Preferences.Get("UserFullName", "");

        // Load saved avatar if any
        var savedAvatar = Preferences.Get("UserAvatarPath", "");
        if (!string.IsNullOrEmpty(savedAvatar) && File.Exists(savedAvatar))
        {
            AvatarImage.Source = ImageSource.FromFile(savedAvatar);
            AvatarImage.IsVisible = true;
            AvatarEmoji.IsVisible = false;
            _selectedImagePath = savedAvatar;
        }
    }

    // ── Avatar ────────────────────────────────────────────────────────

    private async void OnChangeAvatarTapped(object sender, EventArgs e)
    {
        try
        {
            var result = await MediaPicker.Default.PickPhotoAsync(new MediaPickerOptions
            {
                Title = "Choose a profile photo"
            });

            if (result == null) return;

            // Copy to app cache so it persists
            var destPath = Path.Combine(
                FileSystem.CacheDirectory,
                $"avatar_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(result.FileName)}");

            using var sourceStream = await result.OpenReadAsync();
            using var destStream = File.OpenWrite(destPath);
            await sourceStream.CopyToAsync(destStream);

            _selectedImagePath = destPath;

            AvatarImage.Source = ImageSource.FromFile(destPath);
            AvatarImage.IsVisible = true;
            AvatarEmoji.IsVisible = false;
        }
        catch
        {
            await DisplayAlert("Error", "Could not open photo picker.", "OK");
        }
    }

    // ── Password Toggles ──────────────────────────────────────────────

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

    // ── Save ──────────────────────────────────────────────────────────

    private async void OnSaveTapped(object sender, EventArgs e)
    {
        // Reset errors
        HideAllErrors();

        var fullName = FullNameEntry.Text?.Trim() ?? "";
        var currentPw = CurrentPasswordEntry.Text ?? "";
        var newPw = NewPasswordEntry.Text ?? "";
        var confirmPw = ConfirmPasswordEntry.Text ?? "";

        bool hasError = false;

        // Validate full name
        if (string.IsNullOrEmpty(fullName))
        {
            FullNameError.Text = "Full name is required.";
            FullNameError.IsVisible = true;
            hasError = true;
        }

        // Validate password fields only if user wants to change password
        bool changingPassword = !string.IsNullOrEmpty(newPw) || !string.IsNullOrEmpty(confirmPw);

        if (changingPassword)
        {
            if (string.IsNullOrEmpty(currentPw))
            {
                CurrentPasswordError.Text = "Current password is required to set a new one.";
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
            // CHANGED: attach Bearer token from SecureStorage
            var authToken = await SecureStorage.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(authToken))
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            // CHANGED: use /api/auth/profile — userId is read from JWT on the server
            var response = await _httpClient.PutAsJsonAsync(
                $"{AppConfig.BaseUrl}/api/auth/profile",
                new
                {
                    FullName = fullName,
                    CurrentPassword = changingPassword ? currentPw : null,
                    NewPassword = changingPassword ? newPw : null
                });

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ShowError(string.IsNullOrEmpty(errorContent)
                    ? "Failed to update profile. Please try again."
                    : errorContent);
                return;
            }

            // Save locally
            Preferences.Set("UserFullName", fullName);

            if (!string.IsNullOrEmpty(_selectedImagePath))
                Preferences.Set("UserAvatarPath", _selectedImagePath);

            // Show success
            SuccessBanner.IsVisible = true;
            ErrorBanner.IsVisible = false;

            // Clear password fields
            CurrentPasswordEntry.Text = string.Empty;
            NewPasswordEntry.Text = string.Empty;
            ConfirmPasswordEntry.Text = string.Empty;

            await Task.Delay(1500);
            await Navigation.PopAsync();
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
}