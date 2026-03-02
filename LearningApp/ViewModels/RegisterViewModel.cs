using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using LearningApp.Models;
using LearningApp.Services;

namespace LearningApp.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _authService;
        private string _fullName;
        private string _email;
        private string _password;
        private string _confirmPassword;
        private string _fullNameError;
        private string _emailError;
        private string _passwordError;
        private string _confirmPasswordError;
        private string _successMessage;
        private string _errorMessage;
        private bool _isLoading;
        private bool _isPasswordVisible;
        private bool _isConfirmPasswordVisible;

        public RegisterViewModel()
        {
            _authService = new AuthService();
            RegisterCommand = new Command(async () => await RegisterAsync(), () => !IsLoading);
            TogglePasswordVisibilityCommand = new Command(TogglePasswordVisibility);
            ToggleConfirmPasswordVisibilityCommand = new Command(ToggleConfirmPasswordVisibility);
            NavigateToLoginCommand = new Command(async () => await NavigateToLogin());
        }

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged();
                FullNameError = string.Empty;
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
                EmailError = string.Empty;
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
                PasswordError = string.Empty;
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged();
                ConfirmPasswordError = string.Empty;
            }
        }

        public string FullNameError
        {
            get => _fullNameError;
            set
            {
                _fullNameError = value;
                OnPropertyChanged();
            }
        }

        public string EmailError
        {
            get => _emailError;
            set
            {
                _emailError = value;
                OnPropertyChanged();
            }
        }

        public string PasswordError
        {
            get => _passwordError;
            set
            {
                _passwordError = value;
                OnPropertyChanged();
            }
        }

        public string ConfirmPasswordError
        {
            get => _confirmPasswordError;
            set
            {
                _confirmPasswordError = value;
                OnPropertyChanged();
            }
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                _successMessage = value;
                OnPropertyChanged();
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                System.Diagnostics.Debug.WriteLine($"[ViewModel] ErrorMessage set to: {value}");
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                ((Command)RegisterCommand).ChangeCanExecute();
            }
        }

        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set
            {
                _isPasswordVisible = value;
                OnPropertyChanged();
            }
        }

        public bool IsConfirmPasswordVisible
        {
            get => _isConfirmPasswordVisible;
            set
            {
                _isConfirmPasswordVisible = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegisterCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }
        public ICommand ToggleConfirmPasswordVisibilityCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        private bool ValidateForm()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(FullName))
            {
                FullNameError = "Full name is required";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                EmailError = "Email is required";
                isValid = false;
            }
            else if (!IsValidEmail(Email))
            {
                EmailError = "Please enter a valid email";
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                PasswordError = "Password is required";
                isValid = false;
            }
            else if (Password.Length < 6)
            {
                PasswordError = "Password must be at least 6 characters";
                isValid = false;
            }

            if (Password != ConfirmPassword)
            {
                ConfirmPasswordError = "Passwords do not match";
                isValid = false;
            }

            return isValid;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async Task RegisterAsync()
        {
            System.Diagnostics.Debug.WriteLine("[ViewModel] RegisterAsync started");

            if (!ValidateForm())
            {
                System.Diagnostics.Debug.WriteLine("[ViewModel] Form validation failed");
                return;
            }

            System.Diagnostics.Debug.WriteLine("[ViewModel] Form validation passed");

            // Clear previous messages
            ErrorMessage = string.Empty;
            SuccessMessage = string.Empty;

            IsLoading = true;

            System.Diagnostics.Debug.WriteLine($"[ViewModel] Creating RegisterRequest with:");
            System.Diagnostics.Debug.WriteLine($"[ViewModel] - FullName: {FullName}");
            System.Diagnostics.Debug.WriteLine($"[ViewModel] - Email: {Email}");

            var request = new RegisterRequest
            {
                FullName = FullName,
                Email = Email,
                Password = Password
            };

            System.Diagnostics.Debug.WriteLine("[ViewModel] Calling AuthService.RegisterAsync");

            var response = await _authService.RegisterAsync(request);

            System.Diagnostics.Debug.WriteLine($"[ViewModel] Response received - Success: {response.Success}");
            System.Diagnostics.Debug.WriteLine($"[ViewModel] Response Message: {response.Message}");

            IsLoading = false;

            if (response.Success)
            {
                System.Diagnostics.Debug.WriteLine("[ViewModel] Registration successful");
                SuccessMessage = "Account created successfully!";

                // Wait 2 seconds then navigate to login
                await Task.Delay(2000);
                await NavigateToLogin();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[ViewModel] Registration failed");

                // Show error in the UI debug panel
                ErrorMessage = response.Message;

                // Also show in dialog
                await Application.Current.MainPage.DisplayAlert("Error", "", "OK");
            }
        }

        private void TogglePasswordVisibility()
        {
            IsPasswordVisible = !IsPasswordVisible;
        }

        private void ToggleConfirmPasswordVisibility()
        {
            IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
        }

        private async Task NavigateToLogin()
        {
            await Shell.Current.GoToAsync("..");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}