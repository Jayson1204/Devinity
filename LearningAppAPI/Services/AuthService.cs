using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using LearningApp.Api.Data;
using LearningApp.Api.DTOs;
using LearningApp.Api.Models;

namespace LearningApp.Api.Services
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterAsync(RegisterRequest request);
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken);
        Task<UserData> GetUserByIdAsync(string userId);
        Task<UpdateProfileResponse> UpdateProfileAsync(string userId, UpdateProfileRequest request);
        Task<UploadAvatarResponse> UploadAvatarAsync(string userId, Stream imageStream, string fileName);
        Task ClearAvatarAsync(string userId); 
    }

    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IAvatarService _avatarService;

        public AuthService(
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<AuthService> logger,
            IAvatarService avatarService)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _avatarService = avatarService;
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                    return new RegisterResponse { Success = false, Message = "Email already registered" };

                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = request.Email,
                    FullName = request.FullName,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User registered: {Email}", user.Email);

                return new RegisterResponse { Success = true, Message = "Account created successfully!" };
            }
            catch
            {
                return new RegisterResponse { Success = false, Message = "Registration failed. Please try again." };
            }
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    return new LoginResponse { Success = false, Message = "Invalid email or password" };

                if (!user.IsActive)
                    return new LoginResponse { Success = false, Message = "Account is deactivated" };

                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var token = GenerateJwtToken(user);
                var refreshToken = await GenerateRefreshToken(user.Id);

                return new LoginResponse
                {
                    Success = true,
                    Message = "Login successful",
                    Token = token,
                    RefreshToken = refreshToken.Token,
                    User = new UserData
                    {
                        Id = user.Id,
                        Email = user.Email,
                        FullName = user.FullName,
                        AvatarUrl = user.AvatarUrl
                    }
                };
            }
            catch
            {
                return new LoginResponse { Success = false, Message = "Login failed. Please try again." };
            }
        }

        public async Task<LoginResponse> RefreshTokenAsync(string refreshToken)
        {
            try
            {
                var token = await _context.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

                if (token == null || !token.IsActive)
                    return new LoginResponse { Success = false, Message = "Invalid or expired session" };

                token.RevokedAt = DateTime.UtcNow;

                var newJwt = GenerateJwtToken(token.User);
                var newRefreshToken = await GenerateRefreshToken(token.User.Id);
                token.ReplacedByToken = newRefreshToken.Token;

                await _context.SaveChangesAsync();

                return new LoginResponse
                {
                    Success = true,
                    Message = "Token refreshed",
                    Token = newJwt,
                    RefreshToken = newRefreshToken.Token,
                    User = new UserData
                    {
                        Id = token.User.Id,
                        Email = token.User.Email,
                        FullName = token.User.FullName,
                        AvatarUrl = token.User.AvatarUrl
                    }
                };
            }
            catch
            {
                return new LoginResponse { Success = false, Message = "Session refresh failed. Please log in again." };
            }
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            try
            {
                var token = await _context.RefreshTokens
                    .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

                if (token == null || !token.IsActive) return false;

                token.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return true;
            }
            catch { return false; }
        }

        public async Task<UserData> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null) return null;

                return new UserData
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    AvatarUrl = user.AvatarUrl
                };
            }
            catch { return null; }
        }

        public async Task<UpdateProfileResponse> UpdateProfileAsync(string userId, UpdateProfileRequest request)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return new UpdateProfileResponse { Success = false, Message = "User not found" };

                user.FullName = request.FullName;
                user.UpdatedAt = DateTime.UtcNow;

                bool changingPassword = !string.IsNullOrEmpty(request.NewPassword)
                                     && !string.IsNullOrEmpty(request.CurrentPassword);

                if (changingPassword)
                {
                    if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
                        return new UpdateProfileResponse
                        {
                            Success = false,
                            Message = "Current password is incorrect"
                        };

                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
                }

                await _context.SaveChangesAsync();

                return new UpdateProfileResponse
                {
                    Success = true,
                    Message = changingPassword
                        ? "Profile and password updated successfully"
                        : "Profile updated successfully",
                    FullName = user.FullName
                };
            }
            catch
            {
                return new UpdateProfileResponse
                {
                    Success = false,
                    Message = "Failed to update profile. Please try again."
                };
            }
        }

        public async Task<UploadAvatarResponse> UploadAvatarAsync(
            string userId, Stream imageStream, string fileName)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return new UploadAvatarResponse { Success = false, Message = "User not found" };

                var url = await _avatarService.SaveAvatarAsync(imageStream, fileName, userId);
                if (url == null)
                    return new UploadAvatarResponse
                    {
                        Success = false,
                        Message = "Failed to save image. Please try again."
                    };

                user.AvatarUrl = url;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return new UploadAvatarResponse
                {
                    Success = true,
                    Message = "Avatar uploaded successfully",
                    AvatarUrl = url
                };
            }
            catch
            {
                return new UploadAvatarResponse
                {
                    Success = false,
                    Message = "Failed to upload avatar. Please try again."
                };
            }
        }

        // ── ADDED ─────────────────────────────────────────────────────────────
        public async Task ClearAvatarAsync(string userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return;
            user.AvatarUrl = null;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
                new Claim("full_name",                   user.FullName)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<RefreshToken> GenerateRefreshToken(string userId)
        {
            var refreshToken = new RefreshToken
            {
                UserId = userId,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }
    }
}