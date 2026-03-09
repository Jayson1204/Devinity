namespace LearningApp.Api.Services
{
    public interface IAvatarService
    {
        Task<string?> SaveAvatarAsync(Stream imageStream, string fileName, string userId);
    }

    public class AvatarService : IAvatarService
    {
        private const string UploadDir = "/app/uploads/avatars";
        private const string BaseUrl = "https://devinity-production.up.railway.app";

        private readonly ILogger<AvatarService> _logger;

        public AvatarService(ILogger<AvatarService> logger)
        {
            _logger = logger;
        }

        public async Task<string?> SaveAvatarAsync(Stream imageStream, string fileName, string userId)
        {
            try
            {
                Directory.CreateDirectory(UploadDir);

                var ext = Path.GetExtension(fileName).ToLower();
                var saveName = $"{userId}{ext}";
                var savePath = Path.Combine(UploadDir, saveName);

                using var fileStream = File.Create(savePath);
                await imageStream.CopyToAsync(fileStream);

                return $"{BaseUrl}/uploads/avatars/{saveName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save avatar for user {UserId}", userId);
                return null;
            }
        }
    }
}