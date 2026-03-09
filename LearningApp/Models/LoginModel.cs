namespace LearningApp.Models
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public string AvatarUrl { get; set; }
        public UserData User { get; set; }
    }

    public class UserData
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }

        public string AvatarUrl { get; set; }
    }
}