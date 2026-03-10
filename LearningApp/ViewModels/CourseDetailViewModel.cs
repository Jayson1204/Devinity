using LearningApp.Constants;
using LearningApp.Models;
using LearningApp.Services;
using System.Collections.ObjectModel;
using System.Net.Http.Json;
using System.Windows.Input;

namespace LearningApp.ViewModels
{
    public class CourseDetailViewModel : BindableObject
    {
        private readonly HttpClient _httpClient;
        private readonly string _courseName;
        private readonly Action<VideoItem> _onVideoTapped;
        private readonly Action<AssessmentItem> _onAssessmentTapped;

        public ObservableCollection<CourseItem> CourseItems { get; } = new();

        public ICommand VideoTappedCommand { get; }
        public ICommand AssessmentTappedCommand { get; }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        // API response models
        private class FirebaseVideoDto
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string FirebaseUrl { get; set; }
            public string ThumbnailUrl { get; set; }
            public string Level { get; set; }
            public string Category { get; set; }
            public string Duration { get; set; }
            public int OrderIndex { get; set; }
        }

        private class AssessmentDto
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Level { get; set; }
            public string Category { get; set; }
            public string Question { get; set; }
            public string StarterCode { get; set; }
            public string ExpectedOutput { get; set; }
            public int OrderIndex { get; set; }
        }

        private class UserProgressDto
        {
            public int AssessmentId { get; set; }
            public bool IsCompleted { get; set; }
        }

        public CourseDetailViewModel(
            string courseName,
            Action<VideoItem> onVideoTapped,
            Action<AssessmentItem> onAssessmentTapped)
        {
            _courseName = courseName;
            _onVideoTapped = onVideoTapped;
            _onAssessmentTapped = onAssessmentTapped;
            _httpClient = ApiClient.Instance;

            VideoTappedCommand = new Command<VideoItem>(v => _onVideoTapped?.Invoke(v));
            AssessmentTappedCommand = new Command<AssessmentItem>(a => _onAssessmentTapped?.Invoke(a));
        }

        // Static cache shared across all instances
        private static readonly Dictionary<string, List<CourseItem>> _cache = new();

        public async Task LoadAsync(bool forceRefresh = false)
        {
            // Return cached data instantly if available
            if (!forceRefresh && _cache.TryGetValue(_courseName, out var cached))
            {
                CourseItems.Clear();
                foreach (var item in cached)
                    CourseItems.Add(item);
                return;
            }

            IsLoading = true;
            CourseItems.Clear();

            try
            {
                var userId = Preferences.Get("UserId", "");

                // Parallel fetch
                var videosTask = FetchVideos();
                var assessTask = FetchAssessments();
                var progressTask = FetchProgress(userId);

                await Task.WhenAll(videosTask, assessTask, progressTask);

                BuildItems(videosTask.Result, assessTask.Result, progressTask.Result);

                // Save to cache
                _cache[_courseName] = CourseItems.ToList();
            }
            catch { }
            finally
            {
                IsLoading = false;
            }
        }

       
        public void InvalidateCache() => _cache.Remove(_courseName);

        private async Task<List<FirebaseVideoDto>> FetchVideos()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<FirebaseVideoDto>>(
                    $"{AppConfig.BaseUrl}/api/firebasevideos/category/{Uri.EscapeDataString(_courseName)}")
                    ?? new();
            }
            catch { return new(); }
        }

        private async Task<List<AssessmentDto>> FetchAssessments()
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{AppConfig.BaseUrl}/api/assessments/category/{Uri.EscapeDataString(_courseName)}");
                var content = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<AssessmentDto>>(content,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new();
            }
            catch { return new(); }
        }

        private async Task<HashSet<int>> FetchProgress(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) return new();
                var response = await _httpClient.GetAsync(
                    $"{AppConfig.BaseUrl}/api/progress/{Uri.EscapeDataString(userId)}/category/{Uri.EscapeDataString(_courseName)}");
                var content = await response.Content.ReadAsStringAsync();
                var items = System.Text.Json.JsonSerializer.Deserialize<List<UserProgressDto>>(content,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return items != null
                    ? new HashSet<int>(items.Where(p => p.IsCompleted).Select(p => p.AssessmentId))
                    : new();
            }
            catch { return new(); }
        }

        private void BuildItems(
            List<FirebaseVideoDto> videos,
            List<AssessmentDto> assessments,
            HashSet<int> completedIds)
        {
            var levels = new[]
            {
                ("beginner",     "🟢  Beginner",    "#00C9A7"),
                ("intermediate", "🟡  Intermediate", "#F5A623"),
                ("advanced",     "🔴  Advanced",     "#E85D75")
            };

            foreach (var (levelKey, levelLabel, accentHex) in levels)
            {
                var levelVideos = videos
                    .Where(v => (v.Level ?? "beginner").ToLower() == levelKey)
                    .OrderBy(v => v.OrderIndex)
                    .ToList();

                var levelAssessments = assessments
                    .Where(a => (a.Level ?? "beginner").ToLower() == levelKey)
                    .OrderBy(a => a.OrderIndex)
                    .ToList();

                if (levelVideos.Count == 0 && levelAssessments.Count == 0) continue;

                // Section header
                CourseItems.Add(new SectionHeaderItem
                {
                    Level = levelKey,
                    AccentHex = accentHex,
                    LevelLabel = levelLabel
                });

                // Videos
                if (levelVideos.Count > 0)
                {
                    CourseItems.Add(new SubSectionItem
                    {
                        Level = levelKey,
                        AccentHex = accentHex,
                        Label = "🎬  Videos"
                    });

                    foreach (var v in levelVideos)
                        CourseItems.Add(new VideoItem
                        {
                            Id = v.Id,
                            Title = v.Title,
                            Duration = v.Duration,
                            FirebaseUrl = v.FirebaseUrl,
                            Category = v.Category ?? _courseName,
                            Level = levelKey,
                            AccentHex = accentHex
                        });
                }
                else
                {
                    CourseItems.Add(new EmptyNoteItem
                    {
                        Level = levelKey,
                        AccentHex = accentHex,
                        Message = "No videos for this level yet."
                    });
                }

                // Assessments
                if (levelAssessments.Count > 0)
                {
                    CourseItems.Add(new SubSectionItem
                    {
                        Level = levelKey,
                        AccentHex = accentHex,
                        Label = "📝  Assessments"
                    });

                    foreach (var a in levelAssessments)
                        CourseItems.Add(new AssessmentItem
                        {
                            Id = a.Id,
                            Title = a.Title,
                            Question = a.Question,
                            StarterCode = a.StarterCode,
                            ExpectedOutput = a.ExpectedOutput,
                            IsCompleted = completedIds.Contains(a.Id),
                            Level = levelKey,
                            AccentHex = accentHex
                        });
                }
            }
        }

        public async Task MarkVideoWatched(int videoId, string category)
        {
            try
            {
                var userId = Preferences.Get("UserId", "");
                if (string.IsNullOrEmpty(userId)) return;
                await _httpClient.PostAsJsonAsync(
                    $"{AppConfig.BaseUrl}/api/learning/video/watched",
                    new { UserId = userId, VideoId = videoId, Category = category });
            }
            catch { }
        }
    }
}