namespace LearningApp.Api.Models
{
    public class Assessment
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Question { get; set; }
        public string Category { get; set; }  // PHP, Java, JavaScript, etc.
        public string ExpectedOutput { get; set; }
        public string StarterCode { get; set; }  // Pre-filled code for students
        public string Level { get; set; }  // Beginner, Intermediate, Advanced
        public int OrderIndex { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}