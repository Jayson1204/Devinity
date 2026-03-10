using LearningApp.Api.Data;
using LearningApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearningApp.Api.Controllers
{
    [ApiController]
    [Route("api/FirebaseVideos")]
    public class FirebaseVideoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FirebaseVideoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET api/firebasevideos/category/PHP
        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            var videos = await _context.FirebaseVideos
                .Where(v => v.Category.ToLower() == category.ToLower())
                .OrderBy(v => v.OrderIndex)
                .ToListAsync();

            return Ok(videos);
        }

        // GET api/firebasevideos/category/PHP/beginner
        [HttpGet("category/{category}/{level}")]
        public async Task<IActionResult> GetByCategoryAndLevel(string category, string level)
        {
            var videos = await _context.FirebaseVideos
                .Where(v => v.Category.ToLower() == category.ToLower()
                         && v.Level.ToLower() == level.ToLower())
                .OrderBy(v => v.OrderIndex)
                .ToListAsync();

            return Ok(videos);
        }

        // GET api/firebasevideos/all
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var videos = await _context.FirebaseVideos
                .OrderBy(v => v.Category)
                .ThenBy(v => v.OrderIndex)
                .ToListAsync();

            return Ok(videos);
        }
    }
}