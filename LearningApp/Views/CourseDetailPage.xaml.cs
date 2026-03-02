using LearningApp.Models;
using System.Net.Http.Json;

namespace LearningApp.Views
{
    public partial class CourseDetailPage : ContentPage
    {
        private string _courseName;
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "NGROK_URL";

        // Color palette
        private static readonly Color NavyDark = Color.FromArgb("#0A0F2E");
        private static readonly Color NavyMid = Color.FromArgb("#1B2250");
        private static readonly Color NavyLight = Color.FromArgb("#2A3470");
        private static readonly Color AccentBlue = Color.FromArgb("#4A90D9");
        private static readonly Color AccentTeal = Color.FromArgb("#00C9A7");
        private static readonly Color TextPrimary = Colors.White;
        private static readonly Color TextSecond = Color.FromArgb("#8A9BC8");
        private static readonly Color GreenDone = Color.FromArgb("#00C9A7");

        public class FirebaseVideoItem
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string FirebaseUrl { get; set; }
            public string ThumbnailUrl { get; set; }
            public string Level { get; set; }
            public string Category { get; set; }
            public string Duration { get; set; }
            public int OrderIndex { get; set; }
        }

        public class AssessmentItem
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

        public class UserProgressItem
        {
            public int AssessmentId { get; set; }
            public bool IsCompleted { get; set; }
        }

        public CourseDetailPage(string courseName)
        {
            InitializeComponent();
            _courseName = courseName;
            _httpClient = new HttpClient();
            //_httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "true");
            LoadCourse();
        }

        private async void LoadCourse()
        {
            CourseTitleLabel.Text = _courseName;
            await LoadLearningPath();
        }

        private async Task LoadLearningPath()
        {
            try
            {
                LoadingIndicator.IsVisible = true;
                LoadingIndicator.IsRunning = true;

                var userId = Preferences.Get("UserId", "");
                var videosTask = FetchFirebaseVideos();
                var assessmentsTask = FetchAssessments();
                var progressTask = FetchUserProgress(userId);

                await Task.WhenAll(videosTask, assessmentsTask, progressTask);

                BuildLearningPathUI(videosTask.Result, assessmentsTask.Result, progressTask.Result);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load", "OK");
            }
            finally
            {
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
            }
        }

        private async Task<Dictionary<string, List<FirebaseVideoItem>>> FetchFirebaseVideos()
        {
            var result = new Dictionary<string, List<FirebaseVideoItem>>
            {
                ["beginner"] = new(),
                ["intermediate"] = new(),
                ["advanced"] = new()
            };
            try
            {
                var videos = await _httpClient.GetFromJsonAsync<List<FirebaseVideoItem>>(
                    $"{BaseUrl}/api/firebasevideos/category/{Uri.EscapeDataString(_courseName)}");
                if (videos != null)
                    foreach (var v in videos)
                    {
                        var lvl = v.Level?.ToLower() ?? "beginner";
                        if (result.ContainsKey(lvl)) result[lvl].Add(v);
                        else result["beginner"].Add(v);
                    }
            }
            catch { }
            return result;
        }

        private async Task<List<AssessmentItem>> FetchAssessments()
        {
            try
            {
                var response = await _httpClient.GetAsync(
                    $"{BaseUrl}/api/assessments/category/{Uri.EscapeDataString(_courseName)}");
                var content = await response.Content.ReadAsStringAsync();
                return System.Text.Json.JsonSerializer.Deserialize<List<AssessmentItem>>(content,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                    ?? new();
            }
            catch { return new(); }
        }

        private async Task<HashSet<int>> FetchUserProgress(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId)) return new();
                var response = await _httpClient.GetAsync(
                    $"{BaseUrl}/api/progress/{Uri.EscapeDataString(userId)}/category/{Uri.EscapeDataString(_courseName)}");
                var content = await response.Content.ReadAsStringAsync();
                var items = System.Text.Json.JsonSerializer.Deserialize<List<UserProgressItem>>(content,
                    new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return items != null
                    ? new HashSet<int>(items.Where(p => p.IsCompleted).Select(p => p.AssessmentId))
                    : new();
            }
            catch { return new(); }
        }

        private void BuildLearningPathUI(
            Dictionary<string, List<FirebaseVideoItem>> videosByLevel,
            List<AssessmentItem> assessments,
            HashSet<int> completedIds)
        {
            var levels = new[]
            {
                ("beginner",     "🟢  Beginner",     "#00C9A7"),
                ("intermediate", "🟡  Intermediate",  "#F5A623"),
                ("advanced",     "🔴  Advanced",      "#E85D75")
            };

            foreach (var (levelKey, levelLabel, accentHex) in levels)
            {
                var accent = Color.FromArgb(accentHex);
                var videos = videosByLevel.TryGetValue(levelKey, out var v) ? v : new();
                var levelAssessments = assessments.Where(a => a.Level?.ToLower() == levelKey).ToList();

                // Skip empty levels
                if (videos.Count == 0 && levelAssessments.Count == 0) continue;

                // ── Level Section Header ─────────────────────────
                LearningPathContainer.Children.Add(new Border
                {
                    Margin = new Thickness(16, 20, 16, 10),
                    BackgroundColor = Colors.Transparent,
                    StrokeThickness = 0,
                    Content = new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition { Width = GridLength.Auto },
                            new ColumnDefinition { Width = GridLength.Star }
                        },
                        ColumnSpacing = 10,
                        Children =
                        {
                            new Label
                            {
                                Text = levelLabel,
                                FontSize = 16,
                                FontAttributes = FontAttributes.Bold,
                                TextColor = accent,
                                VerticalOptions = LayoutOptions.Center
                            },
                            new BoxView
                            {
                                Color = Color.FromArgb(accentHex + "33"),
                                HeightRequest = 1,
                                VerticalOptions = LayoutOptions.Center,
                                Margin = new Thickness(0,0,0,0)
                            }
                            .WithColumn(1)
                        }
                    }
                });

                // ── Videos ───────────────────────────────────────
                if (videos.Count > 0)
                {
                    LearningPathContainer.Children.Add(SectionLabel("🎬  Videos"));
                    foreach (var video in videos)
                        LearningPathContainer.Children.Add(CreateVideoCard(video, accent));
                }
                else
                {
                    LearningPathContainer.Children.Add(EmptyNote("No videos for this level yet."));
                }

                // ── Assessments ──────────────────────────────────
                if (levelAssessments.Count > 0)
                {
                    LearningPathContainer.Children.Add(SectionLabel("📝  Assessments"));
                    foreach (var a in levelAssessments)
                        LearningPathContainer.Children.Add(
                            CreateAssessmentCard(a, completedIds.Contains(a.Id), accent));
                }
            }
        }

        private Label SectionLabel(string text) => new Label
        {
            Text = text,
            FontSize = 13,
            TextColor = TextSecond,
            FontAttributes = FontAttributes.Bold,
            Margin = new Thickness(20, 10, 20, 6)
        };

        private Label EmptyNote(string text) => new Label
        {
            Text = text,
            FontSize = 13,
            TextColor = TextSecond,
            Margin = new Thickness(20, 0, 20, 8)
        };

        private View CreateVideoCard(FirebaseVideoItem video, Color accent)
        {
            var card = new Border
            {
                BackgroundColor = NavyMid,
                StrokeThickness = 0,
                Margin = new Thickness(16, 4),
                Padding = new Thickness(14, 12),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 14 }
            };

            var row = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(48) },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                ColumnSpacing = 12
            };

            // Play icon circle
            var iconBg = new Border
            {
                BackgroundColor = accent.WithAlpha(0.15f),
                StrokeThickness = 0,
                WidthRequest = 48,
                HeightRequest = 48,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 14 },
                Content = new Label
                {
                    Text = "▶",
                    FontSize = 20,
                    TextColor = accent,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };
            row.Children.Add(iconBg);

            // Title + duration
            var info = new VerticalStackLayout { Spacing = 4, VerticalOptions = LayoutOptions.Center };
            info.Children.Add(new Label
            {
                Text = video.Title,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = TextPrimary,
                LineBreakMode = LineBreakMode.TailTruncation,
                MaxLines = 2
            });
            if (!string.IsNullOrEmpty(video.Duration))
                info.Children.Add(new Label
                {
                    Text = $"⏱ {video.Duration}",
                    FontSize = 12,
                    TextColor = TextSecond
                });
            Grid.SetColumn(info, 1);
            row.Children.Add(info);

            card.Content = row;

            var tap = new TapGestureRecognizer();
            tap.Tapped += async (s, e) =>
            {
                if (!string.IsNullOrEmpty(video.FirebaseUrl))
                {
                    await MarkVideoWatched(video.Id, video.Category);
                    await Navigation.PushModalAsync(
                        new VideoPlayerPage(video.FirebaseUrl, video.Title, video.Duration ?? ""));
                }
                else
                    await DisplayAlert("Error", "Video URL is missing.", "OK");
            };
            card.GestureRecognizers.Add(tap);

            return card;
        }

        private View CreateAssessmentCard(AssessmentItem assessment, bool isCompleted, Color accent)
        {
            var statusColor = isCompleted ? GreenDone : accent;
            var statusIcon = isCompleted ? "✅" : "📝";
            var statusText = isCompleted ? "Completed" : "Start";

            var card = new Border
            {
                BackgroundColor = NavyMid,
                StrokeThickness = 1,
                Stroke = isCompleted ? GreenDone.WithAlpha(0.4f) : NavyLight,
                Margin = new Thickness(16, 4),
                Padding = new Thickness(14, 12),
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 14 }
            };

            var row = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(48) },
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                },
                ColumnSpacing = 12
            };

            var iconBg = new Border
            {
                BackgroundColor = statusColor.WithAlpha(0.15f),
                StrokeThickness = 0,
                WidthRequest = 48,
                HeightRequest = 48,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 14 },
                Content = new Label
                {
                    Text = statusIcon,
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                }
            };
            row.Children.Add(iconBg);

            var info = new VerticalStackLayout { Spacing = 4, VerticalOptions = LayoutOptions.Center };
            info.Children.Add(new Label
            {
                Text = assessment.Title,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                TextColor = TextPrimary,
                LineBreakMode = LineBreakMode.TailTruncation,
                MaxLines = 2
            });
            info.Children.Add(new Label
            {
                Text = assessment.Level ?? "",
                FontSize = 11,
                TextColor = TextSecond
            });
            Grid.SetColumn(info, 1);
            row.Children.Add(info);

            // Action badge
            var badge = new Border
            {
                BackgroundColor = isCompleted ? GreenDone.WithAlpha(0.15f) : AccentBlue.WithAlpha(0.15f),
                StrokeThickness = 0,
                Padding = new Thickness(10, 6),
                VerticalOptions = LayoutOptions.Center,
                StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle { CornerRadius = 8 },
                Content = new Label
                {
                    Text = statusText,
                    FontSize = 11,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = statusColor
                }
            };
            Grid.SetColumn(badge, 2);
            row.Children.Add(badge);

            card.Content = row;

            card.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(async () =>
                {
                    var a = new Assessment
                    {
                        Id = assessment.Id.ToString(),
                        Title = assessment.Title,
                        Level = assessment.Level,
                        Challenges = new List<CodeChallenge>
                        {
                            new CodeChallenge
                            {
                                Id = assessment.Id.ToString(),
                                Question = assessment.Question,
                                StarterCode = assessment.StarterCode,
                                ExpectedOutput = assessment.ExpectedOutput
                            }
                        }
                    };
                    await Navigation.PushAsync(new CodeEditorPage(a, _courseName, assessment.Id));
                })
            });

            return card;
        }

        private async Task MarkVideoWatched(int videoId, string category)
        {
            try
            {
                var userId = Preferences.Get("UserId", "");
                if (string.IsNullOrEmpty(userId)) return;
                var payload = new { UserId = userId, VideoId = videoId, Category = category };
                await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/learning/video/watched", payload);
            }
            catch { }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }

    // Helper extension to set Grid column inline
    internal static class ViewExtensions
    {
        public static T WithColumn<T>(this T view, int column) where T : View
        {
            Grid.SetColumn(view, column);
            return view;
        }
    }
}