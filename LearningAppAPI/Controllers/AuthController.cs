using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using LearningApp.Api.DTOs;
using LearningApp.Api.Services;

namespace LearningApp.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [EnableRateLimiting("api")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new RegisterResponse { Success = false, Message = "Invalid request data" });

            var result = await _authService.RegisterAsync(request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new LoginResponse { Success = false, Message = "Invalid request data" });

            var result = await _authService.LoginAsync(request);
            return result.Success ? Ok(result) : Unauthorized(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest(new LoginResponse { Success = false, Message = "Refresh token is required" });

            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.RefreshToken))
                await _authService.RevokeTokenAsync(request.RefreshToken);

            return Ok(new { success = true, message = "Logged out successfully" });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _authService.GetUserByIdAsync(userId);
            if (user == null) return NotFound(new { success = false, message = "User not found" });

            return Ok(user);
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new UpdateProfileResponse { Success = false, Message = "Invalid request data" });

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _authService.UpdateProfileAsync(userId, request);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPost("avatar")]
        [Authorize]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new UploadAvatarResponse { Success = false, Message = "No file provided" });

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new UploadAvatarResponse { Success = false, Message = "File size must be under 5MB" });

            var allowed = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowed.Contains(file.ContentType.ToLower()))
                return BadRequest(new UploadAvatarResponse { Success = false, Message = "Only JPEG, PNG, and WebP are allowed" });

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            using var stream = file.OpenReadStream();
            var result = await _authService.UploadAvatarAsync(userId, stream, file.FileName);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("health")]
        [EnableRateLimiting("static")]
        public IActionResult HealthCheck()
            => Ok(new { status = "healthy", timestamp = DateTime.UtcNow, version = "1.0.0" });
    }
}