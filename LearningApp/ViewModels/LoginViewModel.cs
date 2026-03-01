using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LearningApp.Models;
using LearningApp.Services;

namespace LearningApp.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private string _email;
        private string _password;
        private string _emailError;
        private string _passwordError;
        private string _apiError;
        private bool _isLoading;
        private bool _isPasswordVisible;

        public LoginViewModel()
        {
            _authService = new AuthService();
            LoginCommand = new Command(async () => await LoginAsync(), () => !IsLoading);
            TogglePasswordVisibilityCommand = new Command(TogglePasswordVisibility);
            NavigateToRegisterCommand = new Command(async () => await NavigateToRegister());
        }

        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); EmailError = string.Empty; ((Command)LoginCommand).ChangeCanExecute(); }
        }

        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); PasswordError = string.Empty; ((Command)LoginCommand).ChangeCanExecute(); }
        }

        public string EmailError
        {
            get => _emailError;
            set { _emailError = value; OnPropertyChanged(); }
        }

        public string PasswordError
        {
            get => _passwordError;
            set { _passwordError = value; OnPropertyChanged(); }
        }

        public string ApiError
        {
            get => _apiError;
            set { _apiError = value; OnPropertyChanged(); }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); ((Command)LoginCommand).ChangeCanExecute(); }
        }

        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set { _isPasswordVisible = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }
        public ICommand NavigateToRegisterCommand { get; }

        private bool ValidateForm()
        {
            bool isValid = true;
            if (string.IsNullOrWhiteSpace(Email))
            { EmailError = "Email is required"; isValid = false; }
            else if (!IsValidEmail(Email))
            { EmailError = "Please enter a valid email"; isValid = false; }
            if (string.IsNullOrWhiteSpace(Password))
            { PasswordError = "Password is required"; isValid = false; }
            return isValid;
        }

        private bool IsValidEmail(string email)
        {
            try { var addr = new System.Net.Mail.MailAddress(email); return addr.Address == email; }
            catch { return false; }
        }

        private async Task LoginAsync()
        {
            ApiError = string.Empty;
            if (!ValidateForm()) return;

            IsLoading = true;
            var response = await _authService.LoginAsync(new LoginRequest { Email = Email, Password = Password });
            IsLoading = false;

            if (response.Success)
            {
                // Save access token
                await SecureStorage.SetAsync("auth_token", response.Token ?? "");

                // Save refresh token for proper server-side logout
                await SecureStorage.SetAsync("refresh_token", response.RefreshToken ?? "");

                // Save user info
                if (response.User != null)
                {
                    Preferences.Set("UserId", response.User.Id ?? "");
                    Preferences.Set("UserEmail", response.User.Email ?? "");
                    Preferences.Set("UserFullName", response.User.FullName ?? "");
                }

                await Shell.Current.GoToAsync("///MainPage");
            }
            else
            {
                ApiError = response.Message;
            }
        }

        private void TogglePasswordVisibility() => IsPasswordVisible = !IsPasswordVisible;

        private async Task NavigateToRegister() => await Shell.Current.GoToAsync("RegisterPage");

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}