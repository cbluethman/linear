using CommunityToolkit.Mvvm.ComponentModel;
using LinearAlgebra.App.Models;
using LinearAlgebra.App.Rendering;
using LinearAlgebra.App.Services;
using SkiaSharp;

namespace LinearAlgebra.App.ViewModels.Lessons;

public partial class VectorBasicsViewModel : LessonViewModelBase
{
    [ObservableProperty] private double _vectorX = 3;
    [ObservableProperty] private double _vectorY = 2;
    [ObservableProperty] private bool _showComponents = true;

    private bool _dragging;
    private Vec2? _quizTarget;

    public override string Title => "Vector Basics";

    public double Magnitude => new Vec2(VectorX, VectorY).Magnitude;
    public double Angle => new Vec2(VectorX, VectorY).AngleDegrees;

    public VectorBasicsViewModel(GridRenderer grid, SoundService sound) : base(grid, sound)
    {
        UpdateExplanation();
    }

    partial void OnVectorXChanged(double value) => UpdateExplanation();
    partial void OnVectorYChanged(double value) => UpdateExplanation();

    private void UpdateExplanation()
    {
        OnPropertyChanged(nameof(Magnitude));
        OnPropertyChanged(nameof(Angle));
        ExplanationText = $"Vector v = ({VectorX:F1}, {VectorY:F1})\n" +
                         $"Magnitude |v| = {Magnitude:F2}\n" +
                         $"Angle = {Angle:F1} degrees";
    }

    public override void Render(SKCanvas canvas, SKSize size)
    {
        var v = new Vec2(VectorX, VectorY);

        if (ShowComponents)
            Vectors.DrawComponentLines(canvas, Grid, v, ColorPalette.VectorA);

        Vectors.DrawVector(canvas, Grid, v, ColorPalette.VectorA, "v");

        if (IsQuizMode && _quizTarget.HasValue)
        {
            Vectors.DrawVector(canvas, Grid, _quizTarget.Value, ColorPalette.VectorResult, "target", dashed: true);
        }

        // Draw magnitude arc indicator
        DrawAngleArc(canvas, v);
    }

    private void DrawAngleArc(SKCanvas canvas, Vec2 v)
    {
        if (v.Magnitude < 0.1) return;

        var radius = 25f;
        var center = Grid.WorldToScreen(0, 0);
        var angleDeg = (float)v.AngleDegrees;

        using var paint = new SKPaint
        {
            Color = ColorPalette.VectorA.WithAlpha(100),
            StrokeWidth = 2,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        var rect = new SKRect(center.X - radius, center.Y - radius, center.X + radius, center.Y + radius);
        canvas.DrawArc(rect, 0, -angleDeg, false, paint);
    }

    public override void OnMouseDown(double worldX, double worldY)
    {
        var v = new Vec2(VectorX, VectorY);
        if (Vectors.HitTestArrowHead(Grid, v, Grid.WorldToScreenX(worldX), Grid.WorldToScreenY(worldY)))
        {
            _dragging = true;
            Sound.PlayClick();
        }
    }

    public override void OnMouseMove(double worldX, double worldY)
    {
        if (!_dragging) return;
        VectorX = Math.Round(worldX * 2) / 2; // Snap to 0.5
        VectorY = Math.Round(worldY * 2) / 2;
    }

    public override void OnMouseUp()
    {
        if (_dragging)
        {
            _dragging = false;
            Sound.PlaySnap();

            if (IsQuizMode && _quizTarget.HasValue)
            {
                var answer = new Vec2(VectorX, VectorY);
                var correct = Quiz.ValidateDragAnswer(answer, _quizTarget.Value);
                RecordAnswer(correct);
                if (correct) StartQuiz(); // Next question
            }
        }
    }

    protected override void StartQuiz()
    {
        var q = Quiz.GenerateVectorBasicsQuestion();
        _quizTarget = (Vec2)q.CorrectAnswer!;
        QuizPrompt = q.Prompt;
        QuizFeedback = "";
        VectorX = 0;
        VectorY = 0;
    }

    protected override void EndQuiz()
    {
        base.EndQuiz();
        _quizTarget = null;
    }
}
