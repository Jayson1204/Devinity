using LearningApp.Models;
using LearningApp.Services;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using LearningApp.Constants;

namespace LearningApp.Views
{
    public partial class CodeEditorPage : ContentPage
    {
        private Assessment _Assessment;
        private string _courseName;
        private int _assessmentId;
        private int _currentChallengeIndex = 0;
        private CodeChallenge _currentChallenge;
        private int _score = 0;
        private int _hintsUsed = 0;
        private bool _hintVisible = false;
        private bool _lastSubmitCorrect = false;
        private string _lastOutput = "";
        private readonly HttpClient _httpClient;
        private readonly HttpClient _judgeClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        public CodeEditorPage(Assessment Assessment, string courseName, int assessmentId)
        {
            InitializeComponent();
            _Assessment = Assessment;
            _courseName = courseName;
            _assessmentId = assessmentId;
            _httpClient = ApiClient.Instance;
            LoadChallenge();
        }

        private void LoadChallenge()
        {
            if (_Assessment.Challenges == null || _Assessment.Challenges.Count == 0)
            {
                AssessmentTitleLabel.Text = _Assessment.Title ?? "Assessment";
                QuestionLabel.Text = "No challenges available.";
                return;
            }

            _currentChallenge = _Assessment.Challenges[_currentChallengeIndex];
            _hintVisible = false;
            HintCard.IsVisible = false;
            _hintsUsed = 0;

            AssessmentTitleLabel.Text = _Assessment.Title ?? "Assessment";
            ChallengeCountLabel.Text = $"{_currentChallengeIndex + 1} of {_Assessment.Challenges.Count}";
            ScoreLabel.Text = $"Score: {_score}";
            LanguageLabel.Text = _courseName;
            LevelBadgeLabel.Text = _Assessment.Level ?? "";
            ChallengeProgressBar.Progress = (double)_currentChallengeIndex / _Assessment.Challenges.Count;
            QuestionLabel.Text = _currentChallenge.Question ?? "Complete the challenge.";
            CodeEditor.Text = _currentChallenge.StarterCode ?? "";
            OutputLabel.Text = "Run your code to see output...";
            OutputLabel.TextColor = Color.FromArgb("#00C9A7");
            OutputStatusLabel.Text = "";
            HintLabel.Text = GenerateHint(_currentChallenge);
        }

        // ── Hint Generator ──
        private string GenerateHint(CodeChallenge challenge)
        {
            if (challenge == null) return "Think through the problem step by step.";
            var q = challenge.Question?.ToLower() ?? "";
            var expected = challenge.ExpectedOutput?.Trim() ?? "";

            if (q.Contains("hello world")) return "Print the exact text: Hello World";
            if (q.Contains("variable") || q.Contains("create a"))
                return "Declare the variable and assign the correct value, then print it.";
            if (q.Contains("for loop") || q.Contains("loop"))
                return "Use a for loop that iterates from 1 to 5 and prints each number.";
            if (q.Contains("function") || q.Contains("method"))
                return "Define the function first, then call it with the specified arguments.";
            if (q.Contains("array") || q.Contains("list"))
                return "Create the collection, then calculate and print the sum.";
            if (q.Contains("select")) return "Use SELECT * FROM tablename to retrieve all columns.";
            if (q.Contains("where")) return "Add WHERE clause after the table name to filter results.";
            if (q.Contains("join")) return "Use JOIN ... ON to connect two tables by a matching column.";
            if (!string.IsNullOrEmpty(expected))
                return $"Your output should be exactly: {expected}";
            return "Read the question carefully and check your syntax.";
        }

        // ── Code Wrapper ───
        private string WrapCode(string code, string language)
        {
            switch (language.ToLower())
            {
                case "java":
                    if (!code.Contains("class "))
                        return $"public class Main {{\n    public static void main(String[] args) {{\n        {code}\n    }}\n}}";
                    return code;

                case "c#":
                    if (!code.Contains("class "))
                        return $"using System;\nclass Program {{\n    static void Main() {{\n        {code}\n    }}\n}}";
                    return code;

                case "c++":
                    if (!code.Contains("#include"))
                        return $"#include <iostream>\nusing namespace std;\nint main() {{\n    {code}\n    return 0;\n}}";
                    return code;

                case "c":
                    if (!code.Contains("#include"))
                        return $"#include <stdio.h>\nint main() {{\n    {code}\n    return 0;\n}}";
                    return code;

                case "php":
                    if (!code.Contains("<?php"))
                        return $"<?php\n{code}\n?>";
                    return code;

                default:
                    return code;
            }
        }

        // ── Judge0 API Code Execution ──
        private async Task<string> ExecuteCode(string code, string language)
        {
            if (language.ToLower() == "mysql")
                return "MySQL live execution is not supported. Check your query structure.";

            var languageId = language.ToLower() switch
            {
                "python" => 71,
                "javascript" => 63,
                "java" => 62,
                "c#" => 51,
                "c++" => 54,
                "c" => 50,
                "php" => 68,
                _ => 71
            };

            try
            {
                var payload = new
                {
                    source_code = WrapCode(code, language),
                    language_id = languageId,
                    stdin = ""
                };

                var response = await _judgeClient.PostAsJsonAsync(
                    "https://ce.judge0.com/submissions?base64_encoded=false&wait=true",
                    payload);

                if (!response.IsSuccessStatusCode)
                    return "Could not connect to code runner. Please try again.";

                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = await response.Content.ReadFromJsonAsync<Judge0Response>(options);

                var output = result?.Stdout?.Trim();
                var compile = result?.CompileOutput?.Trim();
                var stderr = result?.Stderr?.Trim();

                if (!string.IsNullOrEmpty(output)) return output;
                if (!string.IsNullOrEmpty(compile)) return $"Compile Error:\n{compile}";
                if (!string.IsNullOrEmpty(stderr)) return $"Error:\n{stderr}";
                return "No output";
            }
            catch
            {
                return "Could not run code. Please check your connection and try again.";
            }
        }

        private class Judge0Response
        {
            [JsonPropertyName("stdout")]
            public string Stdout { get; set; }

            [JsonPropertyName("stderr")]
            public string Stderr { get; set; }

            [JsonPropertyName("compile_output")]
            public string CompileOutput { get; set; }

            [JsonPropertyName("status")]
            public Judge0Status Status { get; set; }
        }

        private class Judge0Status
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }
        }

        // ── Normalize for comparison ─────────────────────────────────────────
        private string NormalizeOutput(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return Regex.Replace(s.Trim(), @"\s+", " ").ToLower();
        }

        // ── Run Button ───────────────────────────────────────────────────────
        private async void OnRunCodeClicked(object sender, EventArgs e)
        {
            var code = CodeEditor.Text?.Trim();
            if (string.IsNullOrWhiteSpace(code))
            {
                OutputLabel.Text = "⚠️ Please write some code first!";
                OutputLabel.TextColor = Color.FromArgb("#F48771");
                return;
            }

            OutputStatusLabel.Text = "Running...";
            OutputLabel.Text = "⏳ Executing your code...";
            OutputLabel.TextColor = Color.FromArgb("#DCDCAA");

            var output = await ExecuteCode(code, _courseName);
            var expected = _currentChallenge?.ExpectedOutput?.Trim() ?? "";
            bool isCorrect = NormalizeOutput(output) == NormalizeOutput(expected);

            OutputLabel.Text = $"Output:\n{output}";
            OutputLabel.TextColor = isCorrect
                ? Color.FromArgb("#00C9A7")
                : Color.FromArgb("#A8C7FF");
            OutputStatusLabel.Text = isCorrect ? "✓ Correct!" : "Done";
        }

        // ── Hint Button ──────────────────────────────────────────────────────
        private void OnHintClicked(object sender, EventArgs e)
        {
            _hintVisible = !_hintVisible;
            HintCard.IsVisible = _hintVisible;
            if (_hintVisible) _hintsUsed++;
        }

        // ── Submit Button ────────────────────────────────────────────────────
        private async void OnSubmitClicked(object sender, EventArgs e)
        {
            var code = CodeEditor.Text?.Trim();
            if (string.IsNullOrWhiteSpace(code))
            {
                OutputLabel.Text = "⚠️ Please write some code first!";
                OutputLabel.TextColor = Color.FromArgb("#F48771");
                return;
            }

            OutputStatusLabel.Text = "Checking...";
            OutputLabel.Text = "⏳ Verifying your solution...";
            OutputLabel.TextColor = Color.FromArgb("#DCDCAA");

            var output = await ExecuteCode(code, _courseName);
            var expected = _currentChallenge?.ExpectedOutput?.Trim() ?? "";
            bool isCorrect = NormalizeOutput(output) == NormalizeOutput(expected);

            _lastOutput = output;
            _lastSubmitCorrect = isCorrect;

            OutputLabel.Text = $"Output:\n{output}";
            OutputLabel.TextColor = isCorrect
                ? Color.FromArgb("#00C9A7")
                : Color.FromArgb("#A8C7FF");
            OutputStatusLabel.Text = isCorrect ? "✓ Correct!" : "Done";

            ShowResultPopup(isCorrect, output, expected);
        }

        // ── Show Result Popup ────────────────────────────────────────────────
        private void ShowResultPopup(bool isCorrect, string output, string expected)
        {
            bool hasMore = _currentChallengeIndex + 1 < _Assessment.Challenges.Count;

            if (isCorrect)
            {
                int points = _hintsUsed > 0 ? 5 : 10;
                double pct = (double)(_currentChallengeIndex + 1) / _Assessment.Challenges.Count * 100;

                PopupEmoji.Text = "✅";
                PopupTitle.Text = "Correct!";
                PopupOutputLabel.Text = output;
                PopupExpectedSection.IsVisible = false;
                PopupPointsBadge.IsVisible = true;
                PopupPointsLabel.Text = $"+{points} points  •  Progress: {pct:F0}%";
                PopupMessage.Text = hasMore
                    ? $"{_Assessment.Challenges.Count - _currentChallengeIndex - 1} more challenge(s) remaining."
                    : "You've completed all challenges!";
                PopupSecondaryBtn.Text = "Exit";
                PopupPrimaryBtn.Text = hasMore ? "Next →" : "Finish 🎉";
                PopupPrimaryBorder.BackgroundColor = Color.FromArgb("#00C9A7");
                PopupPrimaryBtn.TextColor = Color.FromArgb("#0A0F2E");
            }
            else
            {
                PopupEmoji.Text = "❌";
                PopupTitle.Text = "Not Quite";
                PopupOutputLabel.Text = string.IsNullOrEmpty(output) ? "No output" : output;
                PopupExpectedSection.IsVisible = true;
                PopupExpectedLabel.Text = expected;
                PopupPointsBadge.IsVisible = false;
                PopupMessage.Text = "Review your code and try again. Use 💡 for a hint.";
                PopupSecondaryBtn.Text = "Exit";
                PopupPrimaryBtn.Text = "Try Again";
                PopupPrimaryBorder.BackgroundColor = Color.FromArgb("#4A90D9");
                PopupPrimaryBtn.TextColor = Color.FromArgb("#FFFFFF");
            }

            DimOverlay.IsVisible = true;
            ResultPopup.IsVisible = true;
            ResultPopup.Scale = 0.85;
            ResultPopup.Opacity = 0;
            ResultPopup.ScaleTo(1.0, 250, Easing.CubicOut);
            ResultPopup.FadeTo(1.0, 250);
        }

        private void HideResultPopup()
        {
            ResultPopup.IsVisible = false;
            DimOverlay.IsVisible = false;
        }

        // ── Popup Primary Button ─────────────────────────────────────────────
        private async void OnPopupPrimaryClicked(object sender, EventArgs e)
        {
            HideResultPopup();

            if (_lastSubmitCorrect)
            {
                int points = _hintsUsed > 0 ? 5 : 10;
                _score += points;
                _hintsUsed = 0;
                ScoreLabel.Text = $"Score: {_score}";

                bool hasMore = _currentChallengeIndex + 1 < _Assessment.Challenges.Count;
                if (hasMore)
                {
                    _currentChallengeIndex++;
                    LoadChallenge();
                }
                else
                {
                    await ShowCompletionPopup();
                }
            }
            // If wrong, "Try Again" just closes popup so user can edit code
        }

        // ── Popup Secondary Button (Exit) ────────────────────────────────────
        private async void OnPopupSecondaryClicked(object sender, EventArgs e)
        {
            HideResultPopup();
            bool leave = await DisplayAlert("Leave?",
                $"Current score: {_score}. Progress won't be saved.",
                "Leave", "Stay");
            if (leave) await Navigation.PopAsync();
        }

        // ── Completion Popup ─────────────────────────────────────────────────
        private async Task ShowCompletionPopup()
        {
            int total = _Assessment.Challenges.Count * 10;
            int pct = total > 0 ? _score * 100 / total : 0;
            string grade = pct >= 90 ? "🏆 Excellent!" :
                           pct >= 70 ? "🥈 Good Job!" :
                           pct >= 50 ? "🥉 Keep Practicing!" : "📚 Review & Try Again";
            string emoji = pct >= 90 ? "🏆" : pct >= 70 ? "🥈" : pct >= 50 ? "🥉" : "📚";

            await SaveProgress();

            CompletionEmoji.Text = emoji;
            CompletionGradeLabel.Text = grade;
            CompletionScoreLabel.Text = $"{_score}/{total}";
            CompletionAccuracyLabel.Text = $"{pct}%";
            CompletionCourseLabel.Text = _courseName;

            DimOverlay.IsVisible = true;
            CompletionPopup.IsVisible = true;
            CompletionPopup.Scale = 0.85;
            CompletionPopup.Opacity = 0;
            CompletionPopup.ScaleTo(1.0, 250, Easing.CubicOut);
            CompletionPopup.FadeTo(1.0, 250);
        }

        // ── Completion Done Button ───────────────────────────────────────────
        private async void OnCompletionDoneClicked(object sender, EventArgs e)
        {
            DimOverlay.IsVisible = false;
            CompletionPopup.IsVisible = false;
            await Navigation.PopAsync();
        }

        private async Task SaveProgress()
        {
            try
            {
                var userId = Preferences.Get("UserId", "");
                if (string.IsNullOrEmpty(userId)) return;
                await _httpClient.PostAsJsonAsync($"{AppConfig.BaseUrl}/api/progress/complete", new
                {
                    UserId = userId,
                    AssessmentId = _assessmentId,
                    Category = _courseName,
                    Level = _Assessment.Level ?? "",
                    Score = _score
                });
            }
            catch { }
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            bool leave = await DisplayAlert("Leave?",
                $"Current score: {_score}. Progress won't be saved.",
                "Leave", "Stay");
            if (leave) await Navigation.PopAsync();
        }
    }
}