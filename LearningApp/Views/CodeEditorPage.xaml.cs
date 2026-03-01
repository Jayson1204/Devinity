using LearningApp.Models;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace LearningApp.Views
{
    public partial class CodeEditorPage : ContentPage
    {
        private Assessment _assessment;
        private string _courseName;
        private int _assessmentId;
        private int _currentChallengeIndex = 0;
        private CodeChallenge _currentChallenge;
        private int _score = 0;
        private int _hintsUsed = 0;
        private bool _hintVisible = false;
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "NGROK_URL";

        public CodeEditorPage(Assessment assessment, string courseName, int assessmentId)
        {
            InitializeComponent();
            _assessment = assessment;
            _courseName = courseName;
            _assessmentId = assessmentId;
            _httpClient = new HttpClient();
            //_httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "true");
            LoadChallenge();
        }

        private void LoadChallenge()
        {
            if (_assessment.Challenges == null || _assessment.Challenges.Count == 0)
            {
                AssessmentTitleLabel.Text = _assessment.Title ?? "Assessment";
                QuestionLabel.Text = "No challenges available.";
                return;
            }

            _currentChallenge = _assessment.Challenges[_currentChallengeIndex];
            _hintVisible = false;
            HintCard.IsVisible = false;
            _hintsUsed = 0;

            AssessmentTitleLabel.Text = _assessment.Title ?? "Assessment";
            ChallengeCountLabel.Text = $"{_currentChallengeIndex + 1} of {_assessment.Challenges.Count}";
            ScoreLabel.Text = $"Score: {_score}";
            LanguageLabel.Text = _courseName;
            LevelBadgeLabel.Text = _assessment.Level ?? "";
            ChallengeProgressBar.Progress = (double)_currentChallengeIndex / _assessment.Challenges.Count;
            QuestionLabel.Text = _currentChallenge.Question ?? "Complete the challenge.";
            CodeEditor.Text = _currentChallenge.StarterCode ?? "";
            OutputLabel.Text = "Run your code to see output...";
            OutputLabel.TextColor = Color.FromArgb("#00C9A7");
            OutputStatusLabel.Text = "";
            HintLabel.Text = GenerateHint(_currentChallenge);
        }

        // ── Hint Generator ───────────────────────────────────────────────────
        private string GenerateHint(CodeChallenge challenge)
        {
            if (challenge == null) return "Think through the problem step by step.";
            var q = challenge.Question?.ToLower() ?? "";
            var expected = challenge.ExpectedOutput?.Trim() ?? "";

            if (q.Contains("hello world")) return $"Print the exact text: Hello World";
            if (q.Contains("variable") || q.Contains("create a"))
                return $"Declare the variable and assign the correct value, then print it.";
            if (q.Contains("for loop") || q.Contains("loop"))
                return $"Use a for loop that iterates from 1 to 5 and prints each number.";
            if (q.Contains("function") || q.Contains("method"))
                return $"Define the function first, then call it with the specified arguments.";
            if (q.Contains("array") || q.Contains("list"))
                return $"Create the collection, then calculate and print the sum.";
            if (q.Contains("select")) return "Use SELECT * FROM tablename to retrieve all columns.";
            if (q.Contains("where")) return "Add WHERE clause after the table name to filter results.";
            if (q.Contains("join")) return "Use JOIN ... ON to connect two tables by a matching column.";
            if (!string.IsNullOrEmpty(expected))
                return $"Your output should be exactly: {expected}";
            return "Read the question carefully and check your syntax.";
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
            OutputLabel.Text = "⏳ Executing...";
            OutputLabel.TextColor = Color.FromArgb("#DCDCAA");
            await Task.Delay(600);

            var result = SimulateOutput(code, _currentChallenge);
            OutputLabel.Text = result.output;
            OutputLabel.TextColor = result.isCorrect
                ? Color.FromArgb("#00C9A7")
                : Color.FromArgb("#A8C7FF");
            OutputStatusLabel.Text = "Done";
        }

        // ── Smart Output Simulation ───────────────────────────────────────────
        private (string output, bool isCorrect) SimulateOutput(string code, CodeChallenge challenge)
        {
            if (challenge == null) return ("No challenge loaded.", false);

            var expected = challenge.ExpectedOutput?.Trim() ?? "";
            var q = challenge.Question?.ToLower() ?? "";
            var lang = _courseName.ToLower();

            // Try to extract what the code would print
            var extracted = ExtractOutput(code, lang);

            if (!string.IsNullOrEmpty(extracted))
            {
                bool match = NormalizeOutput(extracted) == NormalizeOutput(expected);
                return match
                    ? ($"Output:\n{extracted}\n\n✓ Correct! Hit Submit to confirm.", true)
                    : ($"Output:\n{extracted}\n\nExpected:\n{expected}", false);
            }

            // Structural checks when we can't extract output
            bool structureOk = CheckStructure(code, q, lang);
            return structureOk
                ? ($"Code looks valid!\n\nExpected output: {expected}\n\nHit Submit if you're confident.", false)
                : ($"Could not determine output.\n\nExpected: {expected}\n\nCheck your syntax and try again.", false);
        }

        // ── Extract Output from Code ─────────────────────────────────────────
        private string ExtractOutput(string code, string lang)
        {
            try
            {
                // ── Python ──
                if (lang == "python")
                {
                    // print("Hello World") or print('Hello World')
                    var printMatches = Regex.Matches(code, @"print\([""'](.+?)[""']\)");
                    if (printMatches.Count == 1) return printMatches[0].Groups[1].Value;

                    // print(variable) where variable = "something"
                    var varMatch = Regex.Match(code, @"(\w+)\s*=\s*[""'](.+?)[""']");
                    var printVar = Regex.Match(code, @"print\((\w+)\)");
                    if (varMatch.Success && printVar.Success && varMatch.Groups[1].Value == printVar.Groups[1].Value)
                        return varMatch.Groups[2].Value;

                    // print(f"Hello {name}") or print("Hello " + name)
                    var fstrMatch = Regex.Match(code, @"print\(f[""'](.+?)[""']\)");
                    if (fstrMatch.Success)
                    {
                        var fstr = fstrMatch.Groups[1].Value;
                        var nameMatch = Regex.Match(code, @"name\s*=\s*[""'](.+?)[""']");
                        if (nameMatch.Success)
                            fstr = fstr.Replace("{name}", nameMatch.Groups[1].Value);
                        return fstr;
                    }

                    // print("Hello " + name)
                    var concatMatch = Regex.Match(code, @"print\([""'](\w+\s*)[""']\s*\+\s*(\w+)\)");
                    if (concatMatch.Success)
                    {
                        var prefix = concatMatch.Groups[1].Value;
                        var varName = concatMatch.Groups[2].Value;
                        var valMatch = Regex.Match(code, varName + @"\s*=\s*[""'](.+?)[""']");
                        if (valMatch.Success) return prefix + valMatch.Groups[1].Value;
                    }

                    // For loop 1 to 5
                    if (Regex.IsMatch(code, @"for\s+\w+\s+in\s+range\s*\(\s*1\s*,\s*6\s*\)") &&
                        code.Contains("print"))
                        return "1 2 3 4 5";

                    // sum / result
                    var sumMatch = Regex.Match(code, @"print\((\w+)\)");
                    var addMatch = Regex.Match(code, @"(\w+)\s*=\s*(\w+)\s*\+\s*(\w+)");
                    if (sumMatch.Success && addMatch.Success)
                    {
                        if (TryEvalSimple(code, out string val)) return val;
                    }

                    // print(sum(list))
                    if (Regex.IsMatch(code, @"print\(\s*sum\s*\(") ||
                        Regex.IsMatch(code, @"print\(.*sum.*\)"))
                        return "60";
                }

                // ── PHP ──
                if (lang == "php")
                {
                    var echoMatch = Regex.Match(code, @"echo\s+[""'](.+?)[""']");
                    if (echoMatch.Success) return echoMatch.Groups[1].Value;

                    var echoVar = Regex.Match(code, @"echo\s+""(.+?)""\s*\.\s*\$(\w+)");
                    if (echoVar.Success)
                    {
                        var prefix = echoVar.Groups[1].Value;
                        var varName = echoVar.Groups[2].Value;
                        var valM = Regex.Match(code, @"\$" + varName + @"\s*=\s*[""'](.+?)[""']");
                        if (valM.Success) return prefix + valM.Groups[1].Value;
                    }

                    if (Regex.IsMatch(code, @"for\s*\(.*\$i\s*=\s*1.*\$i\s*<=\s*5") && code.Contains("echo"))
                        return "1 2 3 4 5";

                    if (code.Contains("array_sum")) return "60";
                }

                // ── JavaScript ──
                if (lang == "javascript")
                {
                    var logMatch = Regex.Match(code, @"console\.log\([""'](.+?)[""']\)");
                    if (logMatch.Success) return logMatch.Groups[1].Value;

                    // console.log("Hello " + name)
                    var concatLog = Regex.Match(code, @"console\.log\([""'](\w+\s*)[""']\s*\+\s*(\w+)\)");
                    if (concatLog.Success)
                    {
                        var prefix = concatLog.Groups[1].Value;
                        var varName = concatLog.Groups[2].Value;
                        var valM = Regex.Match(code, varName + @"\s*=\s*[""'](.+?)[""']");
                        if (valM.Success) return prefix + valM.Groups[1].Value;
                    }

                    // Template literal
                    var templateMatch = Regex.Match(code, @"console\.log\(`(.+?)`\)");
                    if (templateMatch.Success)
                    {
                        var tmpl = templateMatch.Groups[1].Value;
                        var nameM = Regex.Match(code, @"(?:let|const|var)\s+name\s*=\s*[""'](.+?)[""']");
                        if (nameM.Success) tmpl = tmpl.Replace("${name}", nameM.Groups[1].Value);
                        return tmpl;
                    }

                    if (Regex.IsMatch(code, @"for\s*\(.*let\s+\w+\s*=\s*1.*<=\s*5") && code.Contains("console.log"))
                        return "1 2 3 4 5";

                    if (code.Contains("reduce") || Regex.IsMatch(code, @"console\.log\(.*sum.*\)"))
                        return "60";
                }

                // ── Java ──
                if (lang == "java")
                {
                    var printMatch = Regex.Match(code, @"System\.out\.println\([""'](.+?)[""']\)");
                    if (printMatch.Success) return printMatch.Groups[1].Value;

                    var printConcat = Regex.Match(code, @"System\.out\.println\([""'](\w+\s*)[""']\s*\+\s*(\w+)\)");
                    if (printConcat.Success)
                    {
                        var prefix = printConcat.Groups[1].Value;
                        var varName = printConcat.Groups[2].Value;
                        var valM = Regex.Match(code, @"String\s+" + varName + @"\s*=\s*[""'](.+?)[""']");
                        if (valM.Success) return prefix + valM.Groups[1].Value;
                    }

                    if (Regex.IsMatch(code, @"for\s*\(.*int\s+\w+\s*=\s*1.*<=\s*5") && code.Contains("println"))
                        return "1 2 3 4 5";
                }

                // ── C# ──
                if (lang == "c#")
                {
                    var writeMatch = Regex.Match(code, @"Console\.WriteLine\([""'](.+?)[""']\)");
                    if (writeMatch.Success) return writeMatch.Groups[1].Value;

                    var interpMatch = Regex.Match(code, @"Console\.WriteLine\(\$[""'](.+?)[""']\)");
                    if (interpMatch.Success)
                    {
                        var tmpl = interpMatch.Groups[1].Value;
                        var nameM = Regex.Match(code, @"string\s+name\s*=\s*[""'](.+?)[""']");
                        if (nameM.Success) tmpl = tmpl.Replace("{name}", nameM.Groups[1].Value);
                        return tmpl;
                    }

                    if (Regex.IsMatch(code, @"for\s*\(.*int\s+\w+\s*=\s*1.*<=\s*5") && code.Contains("WriteLine"))
                        return "1 2 3 4 5";
                }

                // ── C++ ──
                if (lang == "c++")
                {
                    var coutMatch = Regex.Match(code, @"cout\s*<<\s*[""'](.+?)[""']");
                    if (coutMatch.Success) return coutMatch.Groups[1].Value;

                    if (Regex.IsMatch(code, @"for\s*\(.*int\s+\w+\s*=\s*1.*<=\s*5") && code.Contains("cout"))
                        return "1 2 3 4 5";
                }

                // ── C ──
                if (lang == "c")
                {
                    var printfMatch = Regex.Match(code, @"printf\s*\(\s*[""'](.+?)[""']\s*\)");
                    if (printfMatch.Success)
                    {
                        var val = printfMatch.Groups[1].Value.Replace("\\n", "").Trim();
                        return val;
                    }

                    if (Regex.IsMatch(code, @"for\s*\(.*int\s+\w+\s*=\s*1.*<=\s*5") && code.Contains("printf"))
                        return "1 2 3 4 5";
                }

                // ── MySQL ──
                if (lang == "mysql")
                {
                    var normalized = Regex.Replace(code.Trim().ToUpper(), @"\s+", " ");
                    return normalized;
                }
            }
            catch { }

            return null;
        }

        // ── Structural Check (fallback) ──────────────────────────────────────
        private bool CheckStructure(string code, string question, string lang)
        {
            if (question.Contains("loop"))
                return code.Contains("for") || code.Contains("while");
            if (question.Contains("function") || question.Contains("method"))
                return code.Contains("def ") || code.Contains("function ") ||
                       code.Contains("void ") || code.Contains("int ") || code.Contains("static ");
            if (question.Contains("array") || question.Contains("list"))
                return code.Contains("[") || code.Contains("{") || code.Contains("array");
            return code.Trim().Length > 10;
        }

        // ── Normalize for comparison ─────────────────────────────────────────
        private string NormalizeOutput(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            // Collapse whitespace, trim, lowercase
            return Regex.Replace(s.Trim(), @"\s+", " ").ToLower();
        }

        // ── Simple eval for numeric expressions ─────────────────────────────
        private bool TryEvalSimple(string code, out string result)
        {
            result = "";
            try
            {
                var m = Regex.Match(code, @"(\d+)\s*\+\s*(\d+)");
                if (m.Success)
                {
                    int a = int.Parse(m.Groups[1].Value);
                    int b = int.Parse(m.Groups[2].Value);
                    result = (a + b).ToString();
                    return true;
                }
            }
            catch { }
            return false;
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
                await DisplayAlert("Error", "Please write some code first!", "OK");
                return;
            }

            bool isCorrect = CheckSolution(code, _currentChallenge);

            if (isCorrect)
            {
                int points = _hintsUsed > 0 ? 5 : 10;
                _score += points;
                _hintsUsed = 0;
                ScoreLabel.Text = $"Score: {_score}";

                bool hasMore = _currentChallengeIndex + 1 < _assessment.Challenges.Count;
                double pct = (double)(_currentChallengeIndex + 1) / _assessment.Challenges.Count * 100;

                bool next = await DisplayAlert(
                    "✅ Correct!",
                    $"+{points} points!\n\nProgress: {pct:F0}%" +
                    (hasMore ? $"\n{_assessment.Challenges.Count - _currentChallengeIndex - 1} more challenge(s)." : ""),
                    hasMore ? "Next" : "Finish", "Exit");

                if (next && hasMore) { _currentChallengeIndex++; LoadChallenge(); }
                else await ShowCompletionMessage();
            }
            else
            {
                await DisplayAlert("❌ Not Quite",
                    "Your output doesn't match the expected result.\n\nCheck your code or use 💡 for a hint.",
                    "Try Again");
            }
        }

        // ── Solution Checker ─────────────────────────────────────────────────
        private bool CheckSolution(string code, CodeChallenge challenge)
        {
            if (challenge == null) return true;

            var expected = challenge.ExpectedOutput?.Trim() ?? "";
            var lang = _courseName.ToLower();
            var q = challenge.Question?.ToLower() ?? "";

            // Try extraction first
            var extracted = ExtractOutput(code, lang);
            if (!string.IsNullOrEmpty(extracted))
                return NormalizeOutput(extracted) == NormalizeOutput(expected);

            // MySQL — compare normalized query
            if (lang == "mysql")
            {
                var normCode = Regex.Replace(code.Trim().ToUpper(), @"\s+", " ");
                var normExpected = Regex.Replace(expected.ToUpper(), @"\s+", " ");
                return normCode.Contains(normExpected) || normExpected.Contains(normCode);
            }

            // Structural fallback for loops, functions, arrays
            if (q.Contains("for loop") || q.Contains("loop"))
                return (code.Contains("for") || code.Contains("while")) &&
                       ContainsPrintStatement(code, lang);

            if (q.Contains("function") || q.Contains("method"))
                return ContainsFunctionDef(code, lang) && ContainsPrintStatement(code, lang);

            if (q.Contains("array") || q.Contains("list"))
                return (code.Contains("[") || code.Contains("{")) &&
                       ContainsPrintStatement(code, lang);

            // Last resort — code must be non-trivial
            return code.Length > 20 && ContainsPrintStatement(code, lang);
        }

        private bool ContainsPrintStatement(string code, string lang) =>
            lang switch
            {
                "python" => code.Contains("print("),
                "php" => code.Contains("echo") || code.Contains("print"),
                "javascript" => code.Contains("console.log"),
                "java" => code.Contains("System.out.print"),
                "c#" => code.Contains("Console.Write"),
                "c++" => code.Contains("cout"),
                "c" => code.Contains("printf"),
                _ => true
            };

        private bool ContainsFunctionDef(string code, string lang) =>
            lang switch
            {
                "python" => code.Contains("def "),
                "php" => code.Contains("function "),
                "javascript" => code.Contains("function ") || code.Contains("=>"),
                "java" => Regex.IsMatch(code, @"(static|void|int|String)\s+\w+\s*\("),
                "c#" => Regex.IsMatch(code, @"(static|void|int|string)\s+\w+\s*\("),
                "c++" => Regex.IsMatch(code, @"(int|void|string)\s+\w+\s*\("),
                "c" => Regex.IsMatch(code, @"(int|void)\s+\w+\s*\("),
                _ => true
            };

        // ── Completion ───────────────────────────────────────────────────────
        private async Task ShowCompletionMessage()
        {
            int total = _assessment.Challenges.Count * 10;
            int pct = total > 0 ? _score * 100 / total : 0;
            string grade = pct >= 90 ? "🏆 Excellent!" :
                           pct >= 70 ? "🥈 Good Job!" :
                           pct >= 50 ? "🥉 Keep Practicing!" : "📚 Review & Try Again";

            await SaveProgress();

            await DisplayAlert("Assessment Complete!",
                $"{grade}\n\nScore: {_score} / {total}\nAccuracy: {pct}%\nCourse: {_courseName}",
                "Done!");

            await Navigation.PopAsync();
        }

        private async Task SaveProgress()
        {
            try
            {
                var userId = Preferences.Get("UserId", "");
                if (string.IsNullOrEmpty(userId)) return;
                await _httpClient.PostAsJsonAsync($"{BaseUrl}/api/progress/complete", new
                {
                    UserId = userId,
                    AssessmentId = _assessmentId,
                    Category = _courseName,
                    Level = _assessment.Level ?? "",
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