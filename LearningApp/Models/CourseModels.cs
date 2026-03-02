namespace LearningApp.Models
{
    public class Course
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconColor { get; set; }
        public int TotalVideos { get; set; }
        public List<VideoLesson> Lessons { get; set; } = new();
        public List<Assessment> Assessments { get; set; } = new();
    }

    public class VideoLesson
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string YouTubeVideoId { get; set; }
        public string Duration { get; set; }
        public string Level { get; set; } // Beginner, Intermediate, Advanced
        public string Description { get; set; }
        public int Order { get; set; }
    }

    public class Assessment
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Level { get; set; }
        public List<CodeChallenge> Challenges { get; set; } = new();
    }

    public class CodeChallenge
    {
        public string Id { get; set; }
        public string Question { get; set; }
        public string StarterCode { get; set; }
        public string ExpectedOutput { get; set; }
        public List<TestCase> TestCases { get; set; } = new();
    }

    public class TestCase
    {
        public string Input { get; set; }
        public string ExpectedOutput { get; set; }
    }
}