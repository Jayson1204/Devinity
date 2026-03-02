using LearningApp.Api.Data;
using LearningApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearningApp.Api.Controllers
{
    [ApiController]
    [Route("api/learning")]
    public class LearningProgressController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string[] _categories = { "PHP", "Python", "JavaScript", "Java", "C#", "C++", "C", "MySQL" };

        public LearningProgressController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST api/learning/video/watched
        [HttpPost("video/watched")]
        public async Task<IActionResult> MarkVideoWatched([FromBody] VideoWatchRequest request)
        {
            if (string.IsNullOrEmpty(request.UserId))
                return BadRequest("UserId is required.");

            var existing = await _context.VideoProgress
                .FirstOrDefaultAsync(v => v.UserId == request.UserId && v.VideoId == request.VideoId);

            if (existing != null)
            {
                existing.IsWatched = true;
                existing.WatchedAt = DateTime.UtcNow;
            }
            else
            {
                _context.VideoProgress.Add(new VideoProgress
                {
                    UserId = request.UserId,
                    VideoId = request.VideoId,
                    Category = request.Category,
                    IsWatched = true,
                    WatchedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Video marked as watched." });
        }

        // GET api/learning/{userId}/overview
        // Returns stats + per-course progress for My Learning page
        [HttpGet("{userId}/overview")]
        public async Task<IActionResult> GetOverview(string userId)
        {
            // Total videos watched
            var totalVideosWatched = await _context.VideoProgress
                .CountAsync(v => v.UserId == userId && v.IsWatched);

            // Total assessments completed
            var totalAssessmentsCompleted = await _context.UserProgress
                .CountAsync(p => p.UserId == userId && p.IsCompleted);

            // Per-course progress
            var courseProgress = new List<object>();

            foreach (var category in _categories)
            {
                var totalVideos = await _context.FirebaseVideos
                    .CountAsync(v => v.Category.ToLower() == category.ToLower());

                var watchedVideos = await _context.VideoProgress
                    .CountAsync(v => v.UserId == userId
                                  && v.Category.ToLower() == category.ToLower()
                                  && v.IsWatched);

                var totalAssessments = await _context.Assessments
                    .CountAsync(a => a.Category.ToLower() == category.ToLower());

                var completedAssessments = await _context.UserProgress
                    .CountAsync(p => p.UserId == userId
                                  && p.Category.ToLower() == category.ToLower()
                                  && p.IsCompleted);

                // Skip courses with no content
                if (totalVideos == 0 && totalAssessments == 0) continue;

                var totalItems = totalVideos + totalAssessments;
                var completedItems = watchedVideos + completedAssessments;
                var percentage = totalItems > 0
                    ? Math.Round((double)completedItems / totalItems * 100, 1)
                    : 0;

                courseProgress.Add(new
                {
                    Category = category,
                    TotalVideos = totalVideos,
                    WatchedVideos = watchedVideos,
                    TotalAssessments = totalAssessments,
                    CompletedAssessments = completedAssessments,
                    Percentage = percentage
                });
            }

            return Ok(new
            {
                TotalVideosWatched = totalVideosWatched,
                TotalAssessmentsCompleted = totalAssessmentsCompleted,
                CourseProgress = courseProgress
            });
        }

        public class VideoWatchRequest
        {
            public string UserId { get; set; }
            public int VideoId { get; set; }
            public string Category { get; set; }
        }
    }
}