using LearningApp.Services;

namespace LearningApp.Views;

public partial class CertificatePage : ContentPage
{
    private readonly string _courseName;
    private readonly string _studentName;
    private readonly string _certId;
    private readonly string _issueDate;

    public CertificatePage(string courseName)
    {
        InitializeComponent();
        _courseName = courseName;
        _studentName = Preferences.Get("UserFullName", "Student");
        _issueDate = DateTime.Now.ToString("MMMM dd, yyyy");
        _certId = GenerateCertId(courseName);
        BindData();
        AnimateIn();
    }

    private void BindData()
    {
        StudentNameLabel.Text = _studentName;
        CourseNameLabel.Text = _courseName;
        IssueDateLabel.Text = _issueDate;
        CertIdLabel.Text = _certId;
        CongratsLabel.Text = $"You've mastered {_courseName}. Keep building!";
        Preferences.Set($"cert_{_courseName.Replace(" ", "_")}", _issueDate);
        var count = Preferences.Get("CertificatesCount", 0);
        Preferences.Set("CertificatesCount", count + 1);
    }

    private async void AnimateIn()
    {
        CertificateCard.Opacity = 0;
        CertificateCard.TranslationY = 30;
        await Task.Delay(150);
        await Task.WhenAll(
            CertificateCard.FadeTo(1, 500, Easing.CubicOut),
            CertificateCard.TranslateTo(0, 0, 500, Easing.CubicOut));
    }

    private async void OnBackTapped(object sender, EventArgs e)
        => await Navigation.PopAsync();

    private async void OnShareTapped(object sender, EventArgs e)
    {
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Text = $"🎓 I just completed {_courseName} on Devinity!\n\nCert ID: {_certId}\nIssued: {_issueDate}",
            Title = $"Certificate — {_courseName}"
        });
    }

    private async void OnDownloadPdfTapped(object sender, EventArgs e)
    {
        DownloadBtn.IsEnabled = false;
        DownloadBtn.Text = "Generating...";
        try
        {
            // ← Remove the QuestPDF line, SkiaSharp needs no initialization
            var pdfPath = await Task.Run(() =>
                CertificateGenerator.Generate(
                    _studentName,
                    _courseName,
                    _issueDate,
                    _certId));

            await Share.Default.RequestAsync(new ShareFileRequest
            {
                Title = $"Certificate of Completion — {_courseName}",
                File = new ShareFile(pdfPath, "application/pdf")
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Could not generate PDF: {ex.Message}", "OK");
        }
        finally
        {
            DownloadBtn.IsEnabled = true;
            DownloadBtn.Text = "⬇ PDF";
        }
    }

    private static string GenerateCertId(string courseName)
    {
        var hash = Math.Abs($"{courseName}{DateTime.Now:yyyyMMdd}".GetHashCode()) % 1_000_000;
        return $"CERT-{hash:D6}";
    }
}