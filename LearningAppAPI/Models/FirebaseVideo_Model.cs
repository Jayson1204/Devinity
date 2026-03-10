// LearningApp.Api/Models/FirebaseVideo.cs
namespace LearningApp.Api.Models
{
    public class FirebaseVideo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string FirebaseUrl { get; set; }   
        public string ThumbnailUrl { get; set; }
        public string Level { get; set; }          // beginner, intermediate, advanced
        public string Category { get; set; }       // PHP, Python, JavaScript, etc.
        public string Duration { get; set; }       // e.g. "10:30"
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}