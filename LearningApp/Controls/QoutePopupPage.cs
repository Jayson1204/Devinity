using LearningApp.Services;
using Microsoft.Maui.Controls.Shapes;

namespace LearningApp.Controls;

public class QuotePopupPage : ContentPage
{
    private Border _card = null!;

    public QuotePopupPage(MotivationalQuoteService.Quote quote)
    {
        BackgroundColor = Color.FromArgb("#CC0F172A");
        Shell.SetNavBarIsVisible(this, false);

        _card = new Border
        {
            BackgroundColor = Color.FromArgb("#1E293B"),
            StrokeThickness = 1,
            Stroke = Color.FromArgb("#334155"),
            Padding = new Thickness(28, 32),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(32, 0),
            Opacity = 0,
            Scale = 0.85,
            StrokeShape = new RoundRectangle { CornerRadius = 28 }
        };

        // ── Top row ───
        var iconBadge = new Border
        {
            BackgroundColor = Color.FromArgb("#3B82F620"),
            StrokeThickness = 0,
            WidthRequest = 40,
            HeightRequest = 40,
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Content = new Label
            {
                Text = "⬇️",
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            }
        };

        var titleLabel = new Label
        {
            Text = "WORDS OF THE DAY",
            TextColor = Color.FromArgb("#94A3B8"),
            FontSize = 11,
            FontAttributes = FontAttributes.Bold,
            CharacterSpacing = 1.5,
            VerticalOptions = LayoutOptions.Center,
            Margin = new Thickness(12, 0, 0, 0)
        };

        var closeLabel = new Label
        {
            Text = "✕",
            FontSize = 13,
            TextColor = Color.FromArgb("#64748B"),
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        var closeBadge = new Border
        {
            BackgroundColor = Color.FromArgb("#334155"),
            StrokeThickness = 0,
            WidthRequest = 32,
            HeightRequest = 32,
            StrokeShape = new RoundRectangle { CornerRadius = 10 },
            Content = closeLabel
        };
        closeBadge.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () => await DismissAsync())
        });

        var topRow = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection(
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Auto))
        };
        topRow.Add(iconBadge, 0, 0);
        topRow.Add(titleLabel, 1, 0);
        topRow.Add(closeBadge, 2, 0);

        // ── Divider ───
        var divider = new BoxView
        {
            HeightRequest = 1,
            Color = Color.FromArgb("#334155")
        };

        // ── Quote text ──
        var quoteLabel = new Label
        {
            Text = $"\u201c{quote.Text}\u201d",
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#F8FAFC"),
            LineBreakMode = LineBreakMode.WordWrap,
            HorizontalTextAlignment = TextAlignment.Center,
            LineHeight = 1.4
        };

        // ── Author row ───
        var authorRow = new HorizontalStackLayout
        {
            HorizontalOptions = LayoutOptions.Center,
            Spacing = 6,
            Children =
            {
                new BoxView
                {
                    WidthRequest = 24, HeightRequest = 1.5,
                    Color = Color.FromArgb("#3B82F6"),
                    VerticalOptions = LayoutOptions.Center
                },
                new Label
                {
                    Text = $"— {quote.Author}",
                    FontSize = 13,
                    TextColor = Color.FromArgb("#3B82F6"),
                    FontAttributes = FontAttributes.Italic
                },
                new BoxView
                {
                    WidthRequest = 24, HeightRequest = 1.5,
                    Color = Color.FromArgb("#3B82F6"),
                    VerticalOptions = LayoutOptions.Center
                }
            }
        };

        // ── Keep Going button ───
        var btnLabel = new Label
        {
            Text = "Keep Going ",
            FontSize = 15,
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.White,
            HorizontalOptions = LayoutOptions.Center,
            Padding = new Thickness(0, 14)
        };

        var btnBorder = new Border
        {
            BackgroundColor = Color.FromArgb("#3B82F6"),
            StrokeThickness = 0,
            Margin = new Thickness(0, 4, 0, 0),
            StrokeShape = new RoundRectangle { CornerRadius = 14 },
            Content = btnLabel
        };
        btnBorder.GestureRecognizers.Add(new TapGestureRecognizer
        {
            Command = new Command(async () => await DismissAsync())
        });

        // ── Assemble card ───
        _card.Content = new VerticalStackLayout
        {
            Spacing = 20,
            Children = { topRow, divider, quoteLabel, authorRow, btnBorder }
        };

        
        var rootGrid = new Grid();
        var dimTap = new TapGestureRecognizer
        {
            Command = new Command(async () => await DismissAsync())
        };
        rootGrid.GestureRecognizers.Add(dimTap);
        rootGrid.Add(_card);

        Content = rootGrid;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Animate card in
        await Task.WhenAll(
            _card.FadeTo(1, 280, Easing.CubicOut),
            _card.ScaleTo(1, 280, Easing.CubicOut));
    }

    private async Task DismissAsync()
    {
        await Task.WhenAll(
            _card.FadeTo(0, 180, Easing.CubicIn),
            _card.ScaleTo(0.88, 180, Easing.CubicIn));

        await Navigation.PopModalAsync(false);
    }
}