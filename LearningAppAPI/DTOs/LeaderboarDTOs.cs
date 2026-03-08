namespace LearningApp.Api.DTOs
{
    public class LeaderboardEntry
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public int Score { get; set; }
        public int CoursesCompleted { get; set; }
        public int HoursWatched { get; set; }
        public int AssessmentsCompleted { get; set; }
        public int Rank { get; set; }
    }

    public class LeaderboardResponse
    {
        public List<LeaderboardEntry> Entries { get; set; } = new();
    }
}