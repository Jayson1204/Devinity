using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningApp.Api.Models
{
    [Table("video_progress")]
    public class VideoProgress
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(36)]
        public string UserId { get; set; }

        public int VideoId { get; set; }
        public string Category { get; set; }
        public bool IsWatched { get; set; } = false;
        public DateTime WatchedAt { get; set; } = DateTime.UtcNow;
    }
}