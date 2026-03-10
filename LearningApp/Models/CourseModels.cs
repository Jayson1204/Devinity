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
        public string Level { get; set; }
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

    public abstract class CourseItem
    {
        public string Level { get; set; }
        public string AccentHex { get; set; }
    }

    // Section header e.g. "🟢 Beginner"
    public class SectionHeaderItem : CourseItem
    {
        public string LevelLabel { get; set; }
    }

    // Sub-section label e.g. "🎬 Videos"
    public class SubSectionItem : CourseItem
    {
        public string Label { get; set; }
    }

    // Video row
    public class VideoItem : CourseItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Duration { get; set; }
        public string FirebaseUrl { get; set; }
        public string Category { get; set; }
        public string CategoryIcon => Category?.ToLower() switch
        {
            "c#" or "csharp" => "csharp.png",
            "python" => "python.png",
            "javascript" or "js" => "javascript.png",
            "php" => "php.png",
            "java" => "java.png",
            "c" => "cprog.png",
            "c++" => "cplusplus.png",
            _ => "video_placeholder.png"
        };
    }

    // Assessment row
    public class AssessmentItem : CourseItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Question { get; set; }
        public string StarterCode { get; set; }
        public string ExpectedOutput { get; set; }
        public bool IsCompleted { get; set; }
    }

    // Empty note row
    public class EmptyNoteItem : CourseItem
    {
        public string Message { get; set; }
    }
}