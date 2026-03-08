using LearningApp.Services;
using LearningApp.Constants;

namespace LearningApp.Views;

public partial class LeaderboardPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public LeaderboardPage()
    {
        InitializeComponent();
        _httpClient = ApiClient.Instance;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadLeaderboard();
    }

    private async void LoadLeaderboard()
    {
        SkeletonScroll.IsVisible = true;
        ContentScroll.IsVisible = false;

        try
        {
            var authToken = await SecureStorage.GetAsync("auth_token");
            if (!string.IsNullOrEmpty(authToken))
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            var response = await _httpClient.GetAsync($"{AppConfig.BaseUrl}/api/leaderboard");
            if (!response.IsSuccessStatusCode) return;

            var content = await response.Content.ReadAsStringAsync();
            var result = System.Text.Json.JsonSerializer.Deserialize<LeaderboardResponse>(
                content,
                new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Entries == null || result.Entries.Count == 0) return;

            var entries = result.Entries;
            var currentUserId = Preferences.Get("UserId", "");
            var myAvatarPath = Preferences.Get("UserAvatarPath", "");

            // ── Podium top 3 ──────────────────────────────────────────
            SetPodium(1, entries.ElementAtOrDefault(0), myAvatarPath, currentUserId);
            SetPodium(2, entries.ElementAtOrDefault(1), myAvatarPath, currentUserId);
            SetPodium(3, entries.ElementAtOrDefault(2), myAvatarPath, currentUserId);

            // ── Ranks 4–10 ────────────────────────────────────────────
            RankList.Children.Clear();
            for (int i = 3; i < Math.Min(entries.Count, 10); i++)
            {
                var entry = entries[i];
                bool isMe = entry.UserId == currentUserId;
                RankList.Children.Add(BuildRankRow(i + 1, entry, isMe, myAvatarPath));
            }

            // ── My rank card (if outside top 10) ─────────────────────
            var myEntry = entries.FirstOrDefault(e => e.UserId == currentUserId);
            var myIndex = myEntry != null ? entries.IndexOf(myEntry) : -1;

            if (myEntry != null)
            {
                MyRankNumber.Text = $"#{myIndex + 1}";
                MyRankName.Text = myEntry.FullName;
                MyRankScore.Text = $"{myEntry.Score} pts";

                // Show "You" card only if outside top 10
                MyRankCard.IsVisible = myIndex >= 10;

                // Load avatar for "You" card
                if (!string.IsNullOrEmpty(myAvatarPath) && File.Exists(myAvatarPath))
                {
                    MyAvatarImage.Source = ImageSource.FromFile(myAvatarPath);
                    MyAvatarImage.IsVisible = true;
                    MyAvatarEmoji.IsVisible = false;
                }
            }
        }
        catch { }
        finally
        {
            SkeletonScroll.IsVisible = false;
            ContentScroll.IsVisible = true;
        }
    }

    // ── Podium helper ─────────────────────────────────────────────────

    private void SetPodium(int place, LeaderboardEntry? entry,
                           string myAvatarPath, string currentUserId)
    {
        if (entry == null) return;

        bool isMe = entry.UserId == currentUserId;
        string avatarPath = isMe ? myAvatarPath : "";

        switch (place)
        {
            case 1:
                Name1Label.Text = FirstName(entry.FullName);
                Score1Label.Text = $"{entry.Score} pts";
                SetAvatar(Avatar1Image, Avatar1Emoji, avatarPath);
                break;
            case 2:
                Name2Label.Text = FirstName(entry.FullName);
                Score2Label.Text = $"{entry.Score} pts";
                SetAvatar(Avatar2Image, Avatar2Emoji, avatarPath);
                break;
            case 3:
                Name3Label.Text = FirstName(entry.FullName);
                Score3Label.Text = $"{entry.Score} pts";
                SetAvatar(Avatar3Image, Avatar3Emoji, avatarPath);
                break;
        }
    }

    private static void SetAvatar(Image img, Label emoji, string path)
    {
        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            img.Source = ImageSource.FromFile(path);
            img.IsVisible = true;
            emoji.IsVisible = false;
        }
    }

    // ── Build rank row (4–10) ─────────────────────────────────────────

    private View BuildRankRow(int rank, LeaderboardEntry entry,
                               bool isMe, string myAvatarPath)
    {
        string medalColor = rank switch
        {
            4 => "#3B82F6",
            5 => "#8B5CF6",
            6 => "#EC4899",
            7 => "#F59E0B",
            8 => "#22C55E",
            9 => "#06B6D4",
            10 => "#F97316",
            _ => "#475569"
        };

        var rowBorder = new Border
        {
            BackgroundColor = isMe
                ? Color.FromArgb("#1E40AF20")
                : Color.FromArgb("#1E293B"),
            StrokeThickness = 1,
            Stroke = isMe
                ? Color.FromArgb("#3B82F6")
                : Color.FromArgb("#334155"),
            Padding = new Thickness(0)
        };
        rowBorder.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
        {
            CornerRadius = new CornerRadius(16)
        };

        // Rank number
        var rankLabel = new Label
        {
            Text = $"#{rank}",
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb(medalColor),
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 38
        };

        // Avatar
        var avatarEmoji = new Label
        {
            Text = "👤",
            FontSize = 22,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };
        var avatarImage = new Image
        {
            Aspect = Aspect.AspectFill,
            IsVisible = false,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions = LayoutOptions.Fill
        };

        // Load avatar for current user row
        if (isMe && !string.IsNullOrEmpty(myAvatarPath) && File.Exists(myAvatarPath))
        {
            avatarImage.Source = ImageSource.FromFile(myAvatarPath);
            avatarImage.IsVisible = true;
            avatarEmoji.IsVisible = false;
        }

        var avatarGrid = new Grid();
        avatarGrid.Children.Add(avatarEmoji);
        avatarGrid.Children.Add(avatarImage);

        var avatarBorder = new Border
        {
            BackgroundColor = Color.FromArgb("#0F172A"),
            StrokeThickness = 2,
            Stroke = isMe ? Color.FromArgb("#3B82F6") : Color.FromArgb("#334155"),
            WidthRequest = 44,
            HeightRequest = 44,
            Content = avatarGrid
        };
        avatarBorder.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
        {
            CornerRadius = new CornerRadius(14)
        };

        // Name + score
        var nameLabel = new Label
        {
            Text = entry.FullName,
            FontSize = 14,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#F8FAFC"),
            LineBreakMode = LineBreakMode.TailTruncation,
            MaxLines = 1
        };
        var scoreLabel = new Label
        {
            Text = $"{entry.Score} pts · {entry.CoursesCompleted} courses",
            FontSize = 11,
            TextColor = Color.FromArgb("#64748B")
        };
        var nameStack = new VerticalStackLayout { Spacing = 2, VerticalOptions = LayoutOptions.Center };
        nameStack.Children.Add(nameLabel);
        nameStack.Children.Add(scoreLabel);

        var grid = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Star }
            },
            ColumnSpacing = 12,
            Padding = new Thickness(16, 12)
        };

        Grid.SetColumn(rankLabel, 0);
        Grid.SetColumn(avatarBorder, 1);
        Grid.SetColumn(nameStack, 2);

        grid.Children.Add(rankLabel);
        grid.Children.Add(avatarBorder);
        grid.Children.Add(nameStack);

        rowBorder.Content = grid;
        return rowBorder;
    }

    private static string FirstName(string fullName)
        => fullName?.Split(' ').FirstOrDefault() ?? fullName ?? "—";

    private async void OnBackTapped(object sender, EventArgs e)
        => await Navigation.PopAsync();

    // ── Models ────────────────────────────────────────────────────────

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
        public List<LeaderboardEntry> Entries { get; set; }
    }
}