using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LinearAlgebra.App.Models;
using LinearAlgebra.App.Rendering;
using LinearAlgebra.App.Services;
using SkiaSharp;

namespace LinearAlgebra.App.ViewModels.Lessons;

public partial class DeterminantViewModel : LessonViewModelBase
{
    private static readonly SKTypeface ConsolasTypeface = SKTypeface.FromFamilyName("Consolas");
    private readonly TransformAnimator _animator = new();
    private bool _suppressUpdate;
    private double _quizExpectedDeterminant;

    [ObservableProperty] private double _m11 = 2;
    [ObservableProperty] private double _m12 = 1;
    [ObservableProperty] private double _m21 = 0;
    [ObservableProperty] private double _m22 = 2;
    [ObservableProperty] private string _quizAnswer = "";

    public override string Title => "Determinants";

    public double Determinant => new Mat2(M11, M12, M21, M22).Determinant;

    public DeterminantViewModel(GridRenderer grid, SoundService sound) : base(grid, sound)
    {
        _animator.Updated += () => OnPropertyChanged(nameof(Determinant));
        UpdateExplanation();
    }

    partial void OnM11Changed(double value) { AnimateMatrix(); UpdateExplanation(); }
    partial void OnM12Changed(double value) { AnimateMatrix(); UpdateExplanation(); }
    partial void OnM21Changed(double value) { AnimateMatrix(); UpdateExplanation(); }
    partial void OnM22Changed(double value) { AnimateMatrix(); UpdateExplanation(); }

    private void AnimateMatrix()
    {
        if (_suppressUpdate) return;
        _animator.AnimateTo(new Mat2(M11, M12, M21, M22));
        Sound.PlayWhoosh();
    }

    private void UpdateExplanation()
    {
        var det = Determinant;
        var sign = det >= 0 ? "positive (preserves orientation)" : "negative (reverses orientation)";
        ExplanationText = $"Matrix: [{M11:F2} {M12:F2}; {M21:F2} {M22:F2}]\n" +
                         $"Determinant = {M11:F2}*{M22:F2} - {M12:F2}*{M21:F2} = {det:F2}\n" +
                         $"Area scale factor: |{det:F2}| = {Math.Abs(det):F2}, {sign}";
    }

    public override void Render(SKCanvas canvas, SKSize size)
    {
        var mat = _animator.Current;
        var det = mat.Determinant;

        // Draw original unit square
        Shapes.DrawUnitSquareOriginal(canvas, Grid);

        // Draw transformed parallelogram
        var fillColor = det >= 0
            ? ColorPalette.VectorA.WithAlpha(80)
            : ColorPalette.VectorResult.WithAlpha(80); // Red if orientation flipped
        Shapes.DrawUnitSquare(canvas, Grid, mat, fillColor, ColorPalette.ShapeOutline);

        // Draw basis vectors transformed
        var iHat = mat.Transform(Vec2.UnitX);
        var jHat = mat.Transform(Vec2.UnitY);
        Vectors.DrawVector(canvas, Grid, iHat, ColorPalette.BasisI, "i'");
        Vectors.DrawVector(canvas, Grid, jHat, ColorPalette.BasisJ, "j'");

        // Draw area label at center of parallelogram
        var center = (iHat + jHat) * 0.5;
        var screenCenter = Grid.WorldToScreen(center.X, center.Y);

        using var textPaint = new SKPaint
        {
            Color = ColorPalette.TextPrimary,
            TextSize = 18,
            IsAntialias = true,
            Typeface = ConsolasTypeface,
            TextAlign = SKTextAlign.Center
        };
        canvas.DrawText($"Area = {Math.Abs(det):F2}", screenCenter.X, screenCenter.Y, textPaint);

        if (det < 0)
        {
            canvas.DrawText("(flipped)", screenCenter.X, screenCenter.Y + 20, textPaint);
        }
    }

    public override void OnTick() => _animator.Tick();

    [RelayCommand]
    private void SubmitQuizAnswer()
    {
        if (double.TryParse(QuizAnswer, out var answer))
        {
            var isCorrect = Quiz.ValidateNumericAnswer(answer, _quizExpectedDeterminant, 0.1);
            RecordAnswer(isCorrect);
            if (isCorrect) StartQuiz();
        }
    }

    protected override void StartQuiz()
    {
        var q = Quiz.GenerateDeterminantQuestion();
        QuizPrompt = q.Prompt;
        QuizFeedback = "";
        QuizAnswer = "";

        // Use the exact matrix from the quiz question
        var mat = (Mat2)q.CorrectAnswer!;
        _quizExpectedDeterminant = mat.Determinant;
        _suppressUpdate = true;
        M11 = mat.M11; M12 = mat.M12; M21 = mat.M21; M22 = mat.M22;
        _suppressUpdate = false;
        AnimateMatrix();
        UpdateExplanation();
    }
}
