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

        public AuthController(
            IAuthService authService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("register")]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new RegisterResponse { Success = false, Message = "Invalid request data" });

            var result = await _authService.RegisterAsync(request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("login")]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new LoginResponse { Success = false, Message = "Invalid request data" });

            var result = await _authService.LoginAsync(request);
            if (!result.Success) return Unauthorized(result);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest(new LoginResponse { Success = false, Message = "Refresh token is required" });

            var result = await _authService.RefreshTokenAsync(request.RefreshToken);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.RefreshToken))
                await _authService.RevokeTokenAsync(request.RefreshToken);

            return Ok(new { success = true, message = "Logged out successfully" });
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserData), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        [ProducesResponseType(typeof(UpdateProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new UpdateProfileResponse { Success = false, Message = "Invalid request data" });

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _authService.UpdateProfileAsync(userId, request);
            if (!result.Success) return BadRequest(result);
            return Ok(result);
        }

        // ── POST /api/auth/avatar ─────────────────────────────────────────────
        [HttpPost("avatar")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, message = "No file provided" });

            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { success = false, message = "File too large (max 5MB)" });

            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLower();
            if (!allowed.Contains(ext))
                return BadRequest(new { success = false, message = "Only jpg, png, webp allowed" });

            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            using var stream = file.OpenReadStream();
            var result = await _authService.UploadAvatarAsync(userId, stream, file.FileName);

            if (!result.Success) return StatusCode(500, result);
            return Ok(new { avatarUrl = result.AvatarUrl });
        }

        // ── DELETE /api/auth/avatar ───────────────────────────────────────────
        [HttpDelete("avatar")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteAvatar()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            // Delete file from Railway volume
            var uploadDir = "/app/uploads/avatars";
            foreach (var ext in new[] { ".jpg", ".jpeg", ".png", ".webp" })
            {
                var path = Path.Combine(uploadDir, $"{userId}{ext}");
                if (System.IO.File.Exists(path))
                    System.IO.File.Delete(path);
            }

            await _authService.ClearAvatarAsync(userId);
            return Ok(new { success = true });
        }

        [HttpGet("health")]
        [EnableRateLimiting("static")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult HealthCheck()
            => Ok(new { status = "healthy", timestamp = DateTime.UtcNow, version = "1.0.0" });
    }
}