using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LinearAlgebra.App.Models;
using LinearAlgebra.App.Rendering;
using LinearAlgebra.App.Services;
using SkiaSharp;

namespace LinearAlgebra.App.ViewModels.Lessons;

public partial class LinearCombinationViewModel : LessonViewModelBase
{
    [ObservableProperty] private double _v1X = 2;
    [ObservableProperty] private double _v1Y = 1;
    [ObservableProperty] private double _v2X = -1;
    [ObservableProperty] private double _v2Y = 2;
    [ObservableProperty] private double _scalar1 = 1.0;
    [ObservableProperty] private double _scalar2 = 1.0;

    private bool _draggingV1, _draggingV2;
    private Vec2? _quizTargetScalars;
    private Vec2 _quizTargetVector;

    public override string Title => "Linear Combinations";

    private Vec2 V1 => new(V1X, V1Y);
    private Vec2 V2 => new(V2X, V2Y);
    private Vec2 Result => V1 * Scalar1 + V2 * Scalar2;

    private bool IsCollinear => Math.Abs(Vec2.Cross(V1, V2)) < 0.1;

    public LinearCombinationViewModel(GridRenderer grid, SoundService sound) : base(grid, sound)
    {
        UpdateExplanation();
    }

    partial void OnV1XChanged(double value) => UpdateExplanation();
    partial void OnV1YChanged(double value) => UpdateExplanation();
    partial void OnV2XChanged(double value) => UpdateExplanation();
    partial void OnV2YChanged(double value) => UpdateExplanation();
    partial void OnScalar1Changed(double value) => UpdateExplanation();
    partial void OnScalar2Changed(double value) => UpdateExplanation();

    private void UpdateExplanation()
    {
        var r = Result;
        var collinearWarning = IsCollinear ? "\nWarning: Vectors are collinear! Span is a line, not a plane." : "";
        ExplanationText = $"Linear Combination: {Scalar1:F1} * v1 + {Scalar2:F1} * v2\n" +
                         $"v1 = ({V1X:F1}, {V1Y:F1}), v2 = ({V2X:F1}, {V2Y:F1})\n" +
                         $"Result = ({r.X:F1}, {r.Y:F1}){collinearWarning}";
    }

    public override void Render(SKCanvas canvas, SKSize size)
    {
        var v1 = V1;
        var v2 = V2;
        var sv1 = v1 * Scalar1;
        var sv2 = v2 * Scalar2;
        var result = Result;

        // Draw span region
        if (!IsCollinear)
        {
            Shapes.DrawParallelogram(canvas, Grid, sv1, sv2,
                ColorPalette.Highlight, SKColors.Transparent);
        }
        else
        {
            // Draw line indicating collinear span
            using var linePaint = new SKPaint
            {
                Color = ColorPalette.VectorEigen.WithAlpha(60),
                StrokeWidth = 4,
                IsAntialias = true,
                Style = SKPaintStyle.Stroke
            };
            var dir = v1.Normalized;
            var p1 = Grid.WorldToScreen(dir.X * -20, dir.Y * -20);
            var p2 = Grid.WorldToScreen(dir.X * 20, dir.Y * 20);
            canvas.DrawLine(p1, p2, linePaint);
        }

        // Draw basis vectors
        Vectors.DrawVector(canvas, Grid, v1, ColorPalette.VectorA, "v1");
        Vectors.DrawVector(canvas, Grid, v2, ColorPalette.VectorB, "v2");

        // Draw scaled vectors
        Vectors.DrawVector(canvas, Grid, sv1, ColorPalette.VectorA, $"{Scalar1:F1}v1", 2f, dashed: true);
        Vectors.DrawVector(canvas, Grid, sv2, ColorPalette.VectorB, $"{Scalar2:F1}v2", 2f, dashed: true);

        // Draw result
        Vectors.DrawVector(canvas, Grid, result, ColorPalette.VectorResult, "result");

        // Draw quiz target
        if (IsQuizMode && _quizTargetScalars.HasValue)
        {
            Vectors.DrawVector(canvas, Grid, _quizTargetVector, ColorPalette.VectorEigen, "target", 3f, dashed: true);
        }
    }

    public override void OnMouseDown(double worldX, double worldY)
    {
        var sx = Grid.WorldToScreenX(worldX);
        var sy = Grid.WorldToScreenY(worldY);

        if (Vectors.HitTestArrowHead(Grid, V1, sx, sy)) { _draggingV1 = true; Sound.PlayClick(); }
        else if (Vectors.HitTestArrowHead(Grid, V2, sx, sy)) { _draggingV2 = true; Sound.PlayClick(); }
    }

    public override void OnMouseMove(double worldX, double worldY)
    {
        var snappedX = Math.Round(worldX * 2) / 2;
        var snappedY = Math.Round(worldY * 2) / 2;

        if (_draggingV1) { V1X = snappedX; V1Y = snappedY; }
        else if (_draggingV2) { V2X = snappedX; V2Y = snappedY; }
    }

    public override void OnMouseUp()
    {
        if (_draggingV1 || _draggingV2) Sound.PlaySnap();
        _draggingV1 = _draggingV2 = false;
    }

    [RelayCommand]
    private void SubmitQuizAnswer()
    {
        if (!IsQuizMode || !_quizTargetScalars.HasValue) return;

        var target = _quizTargetScalars.Value;
        var s1Match = Math.Abs(Scalar1 - target.X) < 0.3;
        var s2Match = Math.Abs(Scalar2 - target.Y) < 0.3;
        var correct = s1Match && s2Match;
        RecordAnswer(correct);
        if (correct) StartQuiz();
    }

    protected override void StartQuiz()
    {
        var q = Quiz.GenerateLinearCombinationQuestion();
        _quizTargetScalars = (Vec2)q.CorrectAnswer!;
        _quizTargetVector = V1 * _quizTargetScalars.Value.X + V2 * _quizTargetScalars.Value.Y;
        QuizPrompt = $"Use the sliders to find s1 and s2 so that s1*v1 + s2*v2 reaches the gold target vector.";
        QuizFeedback = "";
        Scalar1 = 0;
        Scalar2 = 0;
    }

    protected override void EndQuiz()
    {
        base.EndQuiz();
        _quizTargetScalars = null;
    }
}
