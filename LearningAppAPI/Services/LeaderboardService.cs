using Microsoft.EntityFrameworkCore;
using LearningApp.Api.Data;
using LearningApp.Api.DTOs;

namespace LearningApp.Api.Services
{
    public interface ILeaderboardService
    {
        Task<LeaderboardResponse> GetTopTenAsync();
    }

    public class LeaderboardService : ILeaderboardService
    {
        private readonly ApplicationDbContext _context;

        public LeaderboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<LeaderboardResponse> GetTopTenAsync()
        {
            try
            {
                // Pull all active users with their course progress
                var users = await _context.Users
                    .Where(u => u.IsActive)
                    .ToListAsync();

                var userIds = users.Select(u => u.Id).ToList();

                // Pull progress for all users in one query
                var allProgress = await _context.CourseProgresses
                    .Where(p => userIds.Contains(p.UserId))
                    .ToListAsync();

                var entries = users.Select(user =>
                {
                    var progress = allProgress.Where(p => p.UserId == user.Id).ToList();

                    int coursesCompleted = progress.Count(p => p.Percentage >= 100);
                    int hoursWatched = (int)Math.Round(progress.Sum(p => p.WatchedVideos) * 0.5);
                    int assessmentsDone = progress.Sum(p => p.CompletedAssessments);

                    // Combined score: courses × 100 + hours × 10 + assessments × 5
                    int score = (coursesCompleted * 100)
                              + (hoursWatched * 10)
                              + (assessmentsDone * 5);

                    return new LeaderboardEntry
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        Score = score,
                        CoursesCompleted = coursesCompleted,
                        HoursWatched = hoursWatched,
                        AssessmentsCompleted = assessmentsDone
                    };
                })
                .OrderByDescending(e => e.Score)
                .Take(10)
                .Select((e, i) => { e.Rank = i + 1; return e; })
                .ToList();

                return new LeaderboardResponse { Entries = entries };
            }
            catch
            {
                return new LeaderboardResponse { Entries = new() };
            }
        }
    }
}