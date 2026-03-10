using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningApp.Api.Models
{
    [Table("user_progress")]
    public class UserProgress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(36)]
        public string UserId { get; set; }  
        public int AssessmentId { get; set; }
        public string Category { get; set; }
        public string Level { get; set; }
        public bool IsCompleted { get; set; } = false;
        public int Score { get; set; } = 0;
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }
}