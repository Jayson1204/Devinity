using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using LearningApp.Api.Services;

namespace LearningApp.Api.Controllers
{
    [ApiController]
    [Route("api/leaderboard")]
    [Authorize]
    [EnableRateLimiting("api")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTopTen()
        {
            var result = await _leaderboardService.GetTopTenAsync();
            return Ok(result);
        }
    }
}