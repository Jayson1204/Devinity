using System.Timers;
using Timer = System.Timers.Timer;

namespace LearningApp.Services;

public class MotivationalQuoteService
{
    // ── Singleton ──────────────────────────────────────────────────────────────
    public static readonly MotivationalQuoteService Instance = new();

    // ── Event raised on UI thread ──────────────────────────────────────────────
    public event Action<Quote>? QuoteReady;

    // ── Quote model ────────────────────────────────────────────────────────────
    public record Quote(string Text, string Author);

    // ── Quote bank ────────────────────────────────────────────────────────────
    private readonly List<Quote> _quotes =
    [
        new("I can do all things through Christ who strengthens me.", "Philippians 4:13"),
        new("For I know the plans I have for you… plans to give you hope and a future.", "Jeremiah 29:11"),
        new("Be strong and courageous. Do not be afraid; for the Lord your God is with you wherever you go", "Joshua 1:9"),
        new("But those who hope in the Lord will renew their strength..", "Isaiah 40:31"),
        new("Trust in the Lord with all your heart and lean not on your own understanding.", "Proverbs 3:5"),
        new("Believe you can and you're halfway there.", "Theodore Roosevelt"),
        new("The only way to do great work is to love what you do.", "Steve Jobs"),
        new("Success is not final, failure is not fatal: it is the courage to continue that counts.", "Winston Churchill"),
        new("Do what you can, with what you have, where you are.", "Theodore Roosevelt"),
        new("It always seems impossible until it’s done.", "Nelson Mandela"),
        new("God is within her, she will not fall.", " Psalm 46:5"),
        new("And we know that in all things God works for the good of those who love Him.", " Romans 8:28"),
        new("With God all things are possible.", "Matthew 19:26"),
        new("Delight yourself in the Lord, and He will give you the desires of your heart.", "Psalm 37:4"),
        new("Do not fear, for I am with you.Insert", "Isaiah 41:10"),
        new("Commit to the Lord whatever you do, and He will establish your plans.", " Proverbs 16:3"),
        new("Let us not become weary in doing good.", "Galatians 6:9"),
        new("For God gave us a spirit not of fear but of power.", "Timothy 1:7"),
        new("Cast your cares on the Lord and He will sustain you.", "Psalm 55:22"),
        new("Take heart! I have overcome the world.", "John 16:33")
    ];

    private readonly Timer _timer;
    private readonly Random _random = new();
    private int _lastIndex = -1;

    private MotivationalQuoteService()
    {
        _timer = new Timer(TimeSpan.FromMinutes(10).TotalMilliseconds);
        _timer.Elapsed += (_, _) => RaiseQuote();
        _timer.AutoReset = true;
    }

    // ── Public API ─────────────────────────────────────────────────────────────

    /// <summary>Call once from App.xaml.cs after shell loads.</summary>
    public void Start() => _timer.Start();

    public void Stop() => _timer.Stop();

    /// <summary>Fires immediately — handy for testing without waiting 2 hours.</summary>
    public void TriggerNow() => RaiseQuote();

    // ── Internals ──────────────────────────────────────────────────────────────

    private void RaiseQuote()
    {
        var quote = PickRandom();
        // Always deliver on the main thread so UI can respond directly
        MainThread.BeginInvokeOnMainThread(() => QuoteReady?.Invoke(quote));
    }

    private Quote PickRandom()
    {
        int index;
        do { index = _random.Next(_quotes.Count); }
        while (index == _lastIndex && _quotes.Count > 1);
        _lastIndex = index;
        return _quotes[index];
    }
}