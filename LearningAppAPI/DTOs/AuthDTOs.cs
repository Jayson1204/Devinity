using System.ComponentModel.DataAnnotations;

namespace LearningApp.Api.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }

    public class RegisterRequest
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(255, MinimumLength = 2)]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
    }

    public class RefreshTokenRequest
    {
        [Required]
        public string RefreshToken { get; set; }
    }

    public class UpdateProfileRequest
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(255, MinimumLength = 2)]
        public string FullName { get; set; }

        public string? CurrentPassword { get; set; }

        [StringLength(100, MinimumLength = 6)]
        public string? NewPassword { get; set; }
    }

    public class UpdateProfileResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? FullName { get; set; }
    }

    public class UploadAvatarResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public UserData User { get; set; }
    }

    public class RegisterResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }

    public class UserData
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string? AvatarUrl { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Success")
            => new() { Success = true, Message = message, Data = data };

        public static ApiResponse<T> ErrorResponse(string message)
            => new() { Success = false, Message = message, Data = default };
    }
}