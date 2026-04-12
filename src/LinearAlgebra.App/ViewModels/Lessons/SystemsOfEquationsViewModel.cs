using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LinearAlgebra.App.Models;
using LinearAlgebra.App.Rendering;
using LinearAlgebra.App.Services;
using SkiaSharp;

namespace LinearAlgebra.App.ViewModels.Lessons;

public partial class SystemsOfEquationsViewModel : LessonViewModelBase
{
    [ObservableProperty] private double _a1 = 1;
    [ObservableProperty] private double _b1 = -1;
    [ObservableProperty] private double _c1 = 0;
    [ObservableProperty] private double _a2 = 1;
    [ObservableProperty] private double _b2 = 1;
    [ObservableProperty] private double _c2 = 4;
    [ObservableProperty] private string _selectedAnswer = "";

    public override string Title => "Systems of Equations";

    private string SolutionType
    {
        get
        {
            var det = A1 * B2 - A2 * B1;
            if (Math.Abs(det) > 1e-10) return "One solution";

            // Parallel or coincident
            var crossC = A1 * C2 - A2 * C1;
            return Math.Abs(crossC) < 1e-10 ? "Infinitely many solutions" : "No solution";
        }
    }

    private Vec2? Solution
    {
        get
        {
            var det = A1 * B2 - A2 * B1;
            if (Math.Abs(det) < 1e-10) return null;
            var x = (C1 * B2 - C2 * B1) / det;
            var y = (A1 * C2 - A2 * C1) / det;
            return new Vec2(x, y);
        }
    }

    public SystemsOfEquationsViewModel(GridRenderer grid, SoundService sound) : base(grid, sound)
    {
        UpdateExplanation();
    }

    partial void OnA1Changed(double value) => UpdateExplanation();
    partial void OnB1Changed(double value) => UpdateExplanation();
    partial void OnC1Changed(double value) => UpdateExplanation();
    partial void OnA2Changed(double value) => UpdateExplanation();
    partial void OnB2Changed(double value) => UpdateExplanation();
    partial void OnC2Changed(double value) => UpdateExplanation();

    private void UpdateExplanation()
    {
        var sol = Solution;
        var solText = sol.HasValue
            ? $"Solution: ({sol.Value.X:F2}, {sol.Value.Y:F2})"
            : SolutionType;

        ExplanationText = $"Equation 1: {A1:F1}x + {B1:F1}y = {C1:F1}\n" +
                         $"Equation 2: {A2:F1}x + {B2:F1}y = {C2:F1}\n" +
                         $"{SolutionType}. {solText}";
    }

    public override void Render(SKCanvas canvas, SKSize size)
    {
        // Draw both lines
        Shapes.DrawLine(canvas, Grid, A1, B1, C1, ColorPalette.VectorA, 3f);
        Shapes.DrawLine(canvas, Grid, A2, B2, C2, ColorPalette.VectorB, 3f);

        // Draw line labels
        DrawLineLabel(canvas, A1, B1, C1, ColorPalette.VectorA, "L1", -50);
        DrawLineLabel(canvas, A2, B2, C2, ColorPalette.VectorB, "L2", 50);

        // Draw intersection point
        var sol = Solution;
        if (sol.HasValue)
        {
            var screenPt = Grid.WorldToScreen(sol.Value.X, sol.Value.Y);

            using var dotPaint = new SKPaint
            {
                Color = ColorPalette.VectorResult,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };
            canvas.DrawCircle(screenPt, 8, dotPaint);

            using var labelPaint = new SKPaint
            {
                Color = ColorPalette.TextPrimary,
                TextSize = 14,
                IsAntialias = true,
                Typeface = SKTypeface.FromFamilyName("Consolas")
            };
            canvas.DrawText($"({sol.Value.X:F1}, {sol.Value.Y:F1})",
                screenPt.X + 12, screenPt.Y - 8, labelPaint);
        }
        else if (SolutionType == "No solution")
        {
            // Draw "parallel" indicator
            using var textPaint = new SKPaint
            {
                Color = ColorPalette.Wrong,
                TextSize = 20,
                IsAntialias = true,
                Typeface = SKTypeface.FromFamilyName("Consolas"),
                TextAlign = SKTextAlign.Center
            };
            canvas.DrawText("Parallel - No Solution", size.Width / 2, 40, textPaint);
        }
        else
        {
            using var textPaint = new SKPaint
            {
                Color = ColorPalette.Correct,
                TextSize = 20,
                IsAntialias = true,
                Typeface = SKTypeface.FromFamilyName("Consolas"),
                TextAlign = SKTextAlign.Center
            };
            canvas.DrawText("Coincident - Infinite Solutions", size.Width / 2, 40, textPaint);
        }
    }

    private void DrawLineLabel(SKCanvas canvas, double a, double b, double c, SKColor color, string label, float offsetX)
    {
        double x = 3, y;
        if (Math.Abs(b) > 1e-10)
            y = (c - a * x) / b;
        else
            return;

        var screenPt = Grid.WorldToScreen(x, y);
        using var paint = new SKPaint
        {
            Color = color,
            TextSize = 16,
            IsAntialias = true,
            Typeface = SKTypeface.FromFamilyName("Consolas", SKFontStyle.Bold)
        };
        canvas.DrawText(label, screenPt.X + offsetX, screenPt.Y - 10, paint);
    }

    [RelayCommand]
    private void SetParallel()
    {
        A1 = 1; B1 = 1; C1 = 2;
        A2 = 1; B2 = 1; C2 = 5;
    }

    [RelayCommand]
    private void SetCoincident()
    {
        A1 = 1; B1 = 1; C1 = 2;
        A2 = 2; B2 = 2; C2 = 4;
    }

    [RelayCommand]
    private void SetIntersecting()
    {
        A1 = 1; B1 = -1; C1 = 0;
        A2 = 1; B2 = 1; C2 = 4;
    }

    [RelayCommand]
    private void SubmitQuizAnswer()
    {
        if (!string.IsNullOrEmpty(SelectedAnswer))
        {
            RecordAnswer(SelectedAnswer == SolutionType);
            if (SelectedAnswer == SolutionType) StartQuiz();
        }
    }

    protected override void StartQuiz()
    {
        var q = Quiz.GenerateSystemsQuestion();
        QuizPrompt = q.Prompt;
        QuizFeedback = "";
        SelectedAnswer = "";

        // Set up a system matching the answer
        var answer = (string)q.CorrectAnswer!;
        switch (answer)
        {
            case "One solution": SetIntersecting(); break;
            case "No solution": SetParallel(); break;
            case "Infinitely many solutions": SetCoincident(); break;
        }
    }
}
