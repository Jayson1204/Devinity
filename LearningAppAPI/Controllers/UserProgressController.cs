using LearningApp.Api.Data;
using LearningApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearningApp.Api.Controllers
{
    [ApiController]
    [Route("api/progress")]
    public class UserProgressController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserProgressController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST api/progress/complete
        [HttpPost("complete")]
        public async Task<IActionResult> MarkComplete([FromBody] CompleteRequest request)
        {
            if (string.IsNullOrEmpty(request.UserId))
                return BadRequest("UserId is required.");

            var existing = await _context.UserProgress
                .FirstOrDefaultAsync(p => p.UserId == request.UserId
                                       && p.AssessmentId == request.AssessmentId);

            if (existing != null)
            {
                existing.IsCompleted = true;
                existing.Score = request.Score;
                existing.CompletedAt = DateTime.UtcNow;
            }
            else
            {
                _context.UserProgress.Add(new UserProgress
                {
                    UserId = request.UserId,
                    AssessmentId = request.AssessmentId,
                    Category = request.Category,
                    Level = request.Level,
                    IsCompleted = true,
                    Score = request.Score,
                    CompletedAt = DateTime.UtcNow
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "Progress saved successfully." });
        }

        // GET api/progress/{userId}/category/{category}
        [HttpGet("{userId}/category/{category}")]
        public async Task<IActionResult> GetProgressByCategory(string userId, string category)
        {
            var progress = await _context.UserProgress
                .Where(p => p.UserId == userId
                         && p.Category.ToLower() == category.ToLower()
                         && p.IsCompleted)
                .ToListAsync();

            return Ok(progress);
        }

        // GET api/progress/{userId}/summary/{category}
        [HttpGet("{userId}/summary/{category}")]
        public async Task<IActionResult> GetSummary(string userId, string category)
        {
            var total = await _context.Assessments
                .CountAsync(a => a.Category.ToLower() == category.ToLower());

            var completed = await _context.UserProgress
                .CountAsync(p => p.UserId == userId
                              && p.Category.ToLower() == category.ToLower()
                              && p.IsCompleted);

            var percentage = total > 0 ? (double)completed / total * 100 : 0;

            return Ok(new
            {
                Category = category,
                TotalAssessments = total,
                CompletedAssessments = completed,
                Percentage = Math.Round(percentage, 1)
            });
        }

        public class CompleteRequest
        {
            public string UserId { get; set; }  // string GUID
            public int AssessmentId { get; set; }
            public string Category { get; set; }
            public string Level { get; set; }
            public int Score { get; set; }
        }
    }
}