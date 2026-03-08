using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using LearningApp.Api.DTOs;
using LearningApp.Api.Services;

namespace LearningApp.Api.Controllers
{
    [ApiController]
    [Route("api/leaderboard")]
    [EnableRateLimiting("api")]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        // GET /api/leaderboard — top 10 ranked users
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(LeaderboardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> GetLeaderboard()
        {
            var result = await _leaderboardService.GetTopTenAsync();
            return Ok(result);
        }
    }
}