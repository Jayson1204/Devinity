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
                var users = await _context.Users
                    .Where(u => u.IsActive)
                    .ToListAsync();

                var userIds = users.Select(u => u.Id).ToList();

                var allProgress = await _context.UserProgress
                    .Where(p => userIds.Contains(p.UserId))
                    .ToListAsync();

                var allVideos = await _context.VideoProgress
                    .Where(v => userIds.Contains(v.UserId))
                    .ToListAsync();

                var entries = users.Select(user =>
                {
                    var progress = allProgress.Where(p => p.UserId == user.Id).ToList();
                    var videos = allVideos.Where(v => v.UserId == user.Id).ToList();

                    int assessmentsDone = progress.Count(p => p.IsCompleted);
                    int coursesCompleted = progress
                        .Where(p => p.IsCompleted && !string.IsNullOrEmpty(p.Category))
                        .Select(p => p.Category)
                        .Distinct()
                        .Count();
                    int videosWatched = videos.Count(v => v.IsWatched);
                    int hoursWatched = (int)Math.Round(videosWatched * 0.5);
                    int score = (coursesCompleted * 100)
                                      + (hoursWatched * 10)
                                      + (assessmentsDone * 5);

                    return new LeaderboardEntry
                    {
                        UserId = user.Id,
                        FullName = user.FullName,
                        AvatarUrl = user.AvatarUrl,
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