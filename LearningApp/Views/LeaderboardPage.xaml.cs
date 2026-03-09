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
            var myAvatarUrl = Preferences.Get("UserAvatarUrl", "");

            // Podium top 3
            SetPodium(1, entries.ElementAtOrDefault(0), currentUserId, myAvatarUrl);
            SetPodium(2, entries.ElementAtOrDefault(1), currentUserId, myAvatarUrl);
            SetPodium(3, entries.ElementAtOrDefault(2), currentUserId, myAvatarUrl);

            // Ranks 4-10
            RankList.Children.Clear();
            for (int i = 3; i < Math.Min(entries.Count, 10); i++)
            {
                var entry = entries[i];
                bool isMe = entry.UserId == currentUserId;
                RankList.Children.Add(BuildRankRow(i + 1, entry, isMe, myAvatarUrl));
            }

            // My rank card if outside top 10
            var myEntry = entries.FirstOrDefault(e => e.UserId == currentUserId);
            var myIndex = myEntry != null ? entries.IndexOf(myEntry) : -1;

            if (myEntry != null)
            {
                MyRankNumber.Text = $"#{myIndex + 1}";
                MyRankName.Text = myEntry.FullName;
                MyRankScore.Text = $"{myEntry.Score} pts";
                MyRankCard.IsVisible = myIndex >= 10;

                var myUrl = myEntry.UserId == currentUserId
                    ? myAvatarUrl
                    : myEntry.AvatarUrl ?? "";
                SetAvatarFromUrl(MyAvatarImage, MyAvatarEmoji, myUrl);
            }
        }
        catch { }
        finally
        {
            SkeletonScroll.IsVisible = false;
            ContentScroll.IsVisible = true;
        }
    }

    private void SetPodium(int place, LeaderboardEntry? entry,
                            string currentUserId, string myAvatarUrl)
    {
        if (entry == null) return;

        var avatarUrl = entry.UserId == currentUserId
            ? myAvatarUrl
            : entry.AvatarUrl ?? "";

        switch (place)
        {
            case 1:
                Name1Label.Text = FirstName(entry.FullName);
                Score1Label.Text = $"{entry.Score} pts";
                SetAvatarFromUrl(Avatar1Image, Avatar1Emoji, avatarUrl);
                break;
            case 2:
                Name2Label.Text = FirstName(entry.FullName);
                Score2Label.Text = $"{entry.Score} pts";
                SetAvatarFromUrl(Avatar2Image, Avatar2Emoji, avatarUrl);
                break;
            case 3:
                Name3Label.Text = FirstName(entry.FullName);
                Score3Label.Text = $"{entry.Score} pts";
                SetAvatarFromUrl(Avatar3Image, Avatar3Emoji, avatarUrl);
                break;
        }
    }

    private static void SetAvatarFromUrl(Image img, Label emoji, string? url)
    {
        if (!string.IsNullOrEmpty(url))
        {
            img.Source = ImageSource.FromUri(new Uri(url));
            img.IsVisible = true;
            emoji.IsVisible = false;
        }
    }

    private View BuildRankRow(int rank, LeaderboardEntry entry,
                               bool isMe, string myAvatarUrl)
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
            BackgroundColor = isMe ? Color.FromArgb("#1E40AF20") : Color.FromArgb("#1E293B"),
            StrokeThickness = 1,
            Stroke = isMe ? Color.FromArgb("#3B82F6") : Color.FromArgb("#334155"),
            Padding = new Thickness(0)
        };
        rowBorder.StrokeShape = new Microsoft.Maui.Controls.Shapes.RoundRectangle
        {
            CornerRadius = new CornerRadius(16)
        };

        var rankLabel = new Label
        {
            Text = $"#{rank}",
            FontSize = 13,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb(medalColor),
            VerticalOptions = LayoutOptions.Center,
            WidthRequest = 38
        };

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

        var avatarUrl = isMe ? myAvatarUrl : entry.AvatarUrl ?? "";
        if (!string.IsNullOrEmpty(avatarUrl))
        {
            avatarImage.Source = ImageSource.FromUri(new Uri(avatarUrl));
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

        var nameStack = new VerticalStackLayout
        {
            Spacing = 2,
            VerticalOptions = LayoutOptions.Center
        };
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

    public class LeaderboardEntry
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string? AvatarUrl { get; set; }
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