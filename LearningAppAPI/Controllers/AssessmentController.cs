using LearningApp.Api.Data;
using LearningApp.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearningAppAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AssessmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AssessmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            var assessments = await _context.Assessments
                .Where(a => a.Category.ToLower() == category.ToLower())
                .OrderBy(a => a.OrderIndex)
                .ToListAsync();

            return Ok(assessments);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var assessment = await _context.Assessments.FindAsync(id);
            if (assessment == null)
                return NotFound();
            return Ok(assessment);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var assessments = await _context.Assessments
                .OrderBy(a => a.Category)
                .ThenBy(a => a.OrderIndex)
                .ToListAsync();
            return Ok(assessments);
        }
    }
}