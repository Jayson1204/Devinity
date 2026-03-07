using SkiaSharp;

namespace LearningApp.Services;

public static class CertificateGenerator
{
    private const float W = 1123f;
    private const float H = 794f;

    public static string Generate(
        string studentName, string courseName,
        string issueDate, string certId)
    {
        var path = System.IO.Path.Combine(
            Microsoft.Maui.Storage.FileSystem.CacheDirectory,
            $"certificate_{courseName.Replace(" ", "_")}_{DateTime.Now:yyyyMMdd}.pdf");

        var metadata = new SKDocumentPdfMetadata { Title = $"Certificate - {courseName}" };

        using var stream = new SKFileWStream(path);
        using var doc = SKDocument.CreatePdf(stream, metadata);
        using var canvas = doc.BeginPage(W, H);

        Draw(canvas, studentName, courseName, issueDate, certId);

        doc.EndPage();
        doc.Close();
        return path;
    }

    private static void Draw(SKCanvas cv,
        string studentName, string courseName,
        string issueDate, string certId)
    {
        using var p = new SKPaint { IsAntialias = true, Style = SKPaintStyle.Fill };

        // ── Background ────────────────────────────────────────────────
        p.Color = SKColor.Parse("#EEF1F7");
        cv.DrawRect(0, 0, W, H, p);

        // ── Left navy wedge (top-left to bottom-left) ─────────────────
        p.Color = SKColor.Parse("#1B2B5E");
        var leftWedge = new SKPath();
        leftWedge.MoveTo(0, 0);
        leftWedge.LineTo(160, 0);
        leftWedge.LineTo(220, H);
        leftWedge.LineTo(0, H);
        leftWedge.Close();
        cv.DrawPath(leftWedge, p);

        // ── Right navy wedge ──────────────────────────────────────────
        var rightWedge = new SKPath();
        rightWedge.MoveTo(W, 0);
        rightWedge.LineTo(W - 160, 0);
        rightWedge.LineTo(W - 220, H);
        rightWedge.LineTo(W, H);
        rightWedge.Close();
        cv.DrawPath(rightWedge, p);

        // ── Gold diagonal lines on wedges ─────────────────────────────
        p.Color = SKColor.Parse("#C9A84C");
        p.Style = SKPaintStyle.Stroke;
        p.StrokeWidth = 1f;
        for (int i = 0; i < 5; i++)
        {
            float x = 170 + i * 18;
            cv.DrawLine(x, 0, x - 60, H, p);
            cv.DrawLine(W - x, 0, W - x + 60, H, p);
        }
        p.Style = SKPaintStyle.Fill;

        // ── Gold top/bottom bars ──────────────────────────────────────
        p.Color = SKColor.Parse("#C9A84C");
        cv.DrawRect(0, 0, W, 10, p);
        cv.DrawRect(0, H - 10, W, 10, p);

        // ── Corner ornaments ──────────────────────────────────────────
        p.Color = SKColor.Parse("#C9A84C");
        p.Style = SKPaintStyle.Stroke;
        p.StrokeWidth = 2f;
        cv.DrawLine(10, 10, 10, 60, p);
        cv.DrawLine(10, 10, 60, 10, p);
        cv.DrawLine(W - 10, H - 10, W - 10, H - 60, p);
        cv.DrawLine(W - 10, H - 10, W - 60, H - 10, p);
        p.Style = SKPaintStyle.Fill;

        // ── Badge (top center) ────────────────────────────────────────
        float cx = W / 2f;
        float badgeY = 115f;

        p.Color = SKColor.Parse("#C9A84C");
        cv.DrawCircle(cx, badgeY, 50, p);
        p.Color = SKColor.Parse("#1B2B5E");
        cv.DrawCircle(cx, badgeY, 43, p);
        p.Color = SKColor.Parse("#C9A84C");
        cv.DrawCircle(cx, badgeY, 37, p);
        p.Color = SKColors.White;
        cv.DrawCircle(cx, badgeY, 31, p);

        DrawText(cv, "🎓", cx, badgeY + 13, 26, SKColors.Black, SKTextAlign.Center);

        // ── CERTIFICATE heading ───────────────────────────────────────
        DrawText(cv, "CERTIFICATE", cx, 215, 46,
            SKColor.Parse("#1B2B5E"), SKTextAlign.Center, bold: true);

        DrawText(cv, "OF COMPLETION", cx, 242, 15,
            SKColor.Parse("#1B2B5E"), SKTextAlign.Center);

        // Gold rule
        p.Color = SKColor.Parse("#C9A84C");
        p.Style = SKPaintStyle.Stroke;
        p.StrokeWidth = 1.5f;
        cv.DrawLine(cx - 140, 254, cx + 140, 254, p);
        p.Style = SKPaintStyle.Fill;

        // ── Proudly present to ────────────────────────────────────────
        DrawText(cv, "PROUDLY PRESENT TO:", cx, 284, 11,
            SKColor.Parse("#888888"), SKTextAlign.Center);

        // ── Student name ──────────────────────────────────────────────
        DrawText(cv, studentName, cx, 328, 36,
            SKColor.Parse("#1B2B5E"), SKTextAlign.Center, bold: true, italic: true);

        float nameW = MeasureText(studentName, 36, bold: true);
        p.Color = SKColor.Parse("#C9A84C");
        p.Style = SKPaintStyle.Stroke;
        p.StrokeWidth = 1.5f;
        cv.DrawLine(cx - nameW / 2 - 10, 336,
                    cx + nameW / 2 + 10, 336, p);
        p.Style = SKPaintStyle.Fill;

        // ── Body text ─────────────────────────────────────────────────
        DrawText(cv,
            "Has successfully completed all requirements and assessments for the course",
            cx, 368, 11, SKColor.Parse("#666666"), SKTextAlign.Center);

        DrawText(cv, courseName, cx, 398, 16,
            SKColor.Parse("#1B2B5E"), SKTextAlign.Center, bold: true);

        DrawText(cv, "and is hereby awarded this Certificate of Completion.",
            cx, 424, 11, SKColor.Parse("#666666"), SKTextAlign.Center);

        // Silver divider
        p.Color = SKColor.Parse("#C0C8D8");
        p.Style = SKPaintStyle.Stroke;
        p.StrokeWidth = 0.5f;
        cv.DrawLine(cx - 220, 440, cx + 220, 440, p);
        p.Style = SKPaintStyle.Fill;

        // ── Signature lines ───────────────────────────────────────────
        float sigY = 560f;
        p.Color = SKColor.Parse("#1B2B5E");
        p.Style = SKPaintStyle.Stroke;
        p.StrokeWidth = 1f;
        cv.DrawLine(cx - 220, sigY, cx - 80, sigY, p);
        cv.DrawLine(cx + 80, sigY, cx + 200, sigY, p);
        p.Style = SKPaintStyle.Fill;

        DrawText(cv, "INSTRUCTOR", cx - 150, sigY + 18, 9,
            SKColor.Parse("#1B2B5E"), SKTextAlign.Center, bold: true);
        DrawText(cv, "DIRECTOR", cx + 140, sigY + 18, 9,
            SKColor.Parse("#1B2B5E"), SKTextAlign.Center, bold: true);

        // ── Date + ID ─────────────────────────────────────────────────
        DrawText(cv, $"Date: {issueDate}", cx - 220, sigY + 36, 9,
            SKColor.Parse("#777777"), SKTextAlign.Left);
        DrawText(cv, $"ID: {certId}", cx + 220, sigY + 36, 9,
            SKColor.Parse("#777777"), SKTextAlign.Right);

        // ── Footer ────────────────────────────────────────────────────
        DrawText(cv, "Issued by Devinity Learning Platform",
            cx, H - 18, 9, SKColor.Parse("#AAAAAA"), SKTextAlign.Center);
    }

    private static void DrawText(
        SKCanvas cv, string text, float x, float y,
        float size, SKColor color, SKTextAlign align,
        bool bold = false, bool italic = false)
    {
        using var paint = new SKPaint
        {
            IsAntialias = true,
            Color = color,
            TextSize = size,
            TextAlign = align,
            Typeface = SKTypeface.FromFamilyName(
                "sans-serif",
                bold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal,
                SKFontStyleWidth.Normal,
                italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright)
        };
        cv.DrawText(text, x, y, paint);
    }

    private static float MeasureText(string text, float size, bool bold = false)
    {
        using var paint = new SKPaint
        {
            TextSize = size,
            Typeface = SKTypeface.FromFamilyName(
                "sans-serif",
                bold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal,
                SKFontStyleWidth.Normal,
                SKFontStyleSlant.Upright)
        };
        return paint.MeasureText(text);
    }
}