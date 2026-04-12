using CommunityToolkit.Mvvm.ComponentModel;
using LinearAlgebra.App.Models;
using LinearAlgebra.App.Rendering;
using LinearAlgebra.App.Services;
using SkiaSharp;

namespace LinearAlgebra.App.ViewModels.Lessons;

public partial class VectorOperationsViewModel : LessonViewModelBase
{
    [ObservableProperty] private double _aX = 3;
    [ObservableProperty] private double _aY = 1;
    [ObservableProperty] private double _bX = 1;
    [ObservableProperty] private double _bY = 3;
    [ObservableProperty] private double _scalar = 1.0;
    [ObservableProperty] private int _operationIndex; // 0=Add, 1=Sub, 2=Scale

    private bool _draggingA, _draggingB, _draggingResult;
    private Vec2? _quizTarget;

    public override string Title => "Vector Operations";

    public VectorOperationsViewModel(GridRenderer grid, SoundService sound) : base(grid, sound)
    {
        UpdateExplanation();
    }

    partial void OnAXChanged(double value) => UpdateExplanation();
    partial void OnAYChanged(double value) => UpdateExplanation();
    partial void OnBXChanged(double value) => UpdateExplanation();
    partial void OnBYChanged(double value) => UpdateExplanation();
    partial void OnScalarChanged(double value) => UpdateExplanation();
    partial void OnOperationIndexChanged(int value) => UpdateExplanation();

    private Vec2 A => new(AX, AY);
    private Vec2 B => new(BX, BY);

    private Vec2 Result => OperationIndex switch
    {
        0 => A + B,
        1 => A - B,
        2 => A * Scalar,
        _ => A + B
    };

    private void UpdateExplanation()
    {
        var opName = OperationIndex switch { 0 => "Addition", 1 => "Subtraction", 2 => "Scalar Multiplication", _ => "" };
        var r = Result;
        ExplanationText = $"Operation: {opName}\n" +
                         $"A = ({AX:F1}, {AY:F1}), B = ({BX:F1}, {BY:F1})\n" +
                         (OperationIndex == 2
                             ? $"Result = {Scalar:F1} * A = ({r.X:F1}, {r.Y:F1})"
                             : $"Result = ({r.X:F1}, {r.Y:F1})");
    }

    public override void Render(SKCanvas canvas, SKSize size)
    {
        var a = A;
        var b = B;
        var result = Result;

        Vectors.DrawVector(canvas, Grid, a, ColorPalette.VectorA, "A");
        if (OperationIndex != 2)
            Vectors.DrawVector(canvas, Grid, b, ColorPalette.VectorB, "B");

        // Show parallelogram rule for addition
        if (OperationIndex == 0)
        {
            Vectors.DrawVector(canvas, Grid, a, a + b, ColorPalette.VectorB, null, 2f, dashed: true);
            Vectors.DrawVector(canvas, Grid, b, a + b, ColorPalette.VectorA, null, 2f, dashed: true);
        }

        Vectors.DrawVector(canvas, Grid, result, ColorPalette.VectorResult, "R");

        if (IsQuizMode && _quizTarget.HasValue)
        {
            Vectors.DrawVector(canvas, Grid, _quizTarget.Value, ColorPalette.VectorEigen, "?", dashed: true);
        }
    }

    public override void OnMouseDown(double worldX, double worldY)
    {
        var sx = Grid.WorldToScreenX(worldX);
        var sy = Grid.WorldToScreenY(worldY);

        if (Vectors.HitTestArrowHead(Grid, A, sx, sy)) { _draggingA = true; Sound.PlayClick(); }
        else if (Vectors.HitTestArrowHead(Grid, B, sx, sy)) { _draggingB = true; Sound.PlayClick(); }
        else if (IsQuizMode && Vectors.HitTestArrowHead(Grid, Result, sx, sy)) { _draggingResult = true; Sound.PlayClick(); }
    }

    public override void OnMouseMove(double worldX, double worldY)
    {
        var snappedX = Math.Round(worldX * 2) / 2;
        var snappedY = Math.Round(worldY * 2) / 2;

        if (_draggingA) { AX = snappedX; AY = snappedY; }
        else if (_draggingB) { BX = snappedX; BY = snappedY; }
    }

    public override void OnMouseUp()
    {
        var wasDragging = _draggingA || _draggingB || _draggingResult;
        _draggingA = _draggingB = _draggingResult = false;
        if (wasDragging) Sound.PlaySnap();
    }

    protected override void StartQuiz()
    {
        var q = Quiz.GenerateVectorOperationsQuestion();
        _quizTarget = (Vec2)q.CorrectAnswer!;
        QuizPrompt = q.Prompt;
        QuizFeedback = "";
        OperationIndex = 0;
    }

    protected override void EndQuiz()
    {
        base.EndQuiz();
        _quizTarget = null;
    }
}
