using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LinearAlgebra.App.Models;
using LinearAlgebra.App.Rendering;
using LinearAlgebra.App.Services;
using SkiaSharp;

namespace LinearAlgebra.App.ViewModels.Lessons;

public partial class EigenViewModel : LessonViewModelBase
{
    private readonly TransformAnimator _animator = new();
    private readonly List<Vec2> _sampleVectors = new();
    private const int NumSampleVectors = 24;
    private bool _suppressUpdate;

    [ObservableProperty] private double _m11 = 2;
    [ObservableProperty] private double _m12 = 1;
    [ObservableProperty] private double _m21 = 1;
    [ObservableProperty] private double _m22 = 2;
    [ObservableProperty] private double _animationT;
    [ObservableProperty] private bool _showEigenvectors = true;

    public override string Title => "Eigenvalues & Eigenvectors";

    public Mat2 Matrix => new(M11, M12, M21, M22);

    public EigenViewModel(GridRenderer grid, SoundService sound) : base(grid, sound)
    {
        _animator.Updated += () => OnPropertyChanged(nameof(AnimationT));
        GenerateSampleVectors();
        UpdateExplanation();
    }

    private void GenerateSampleVectors()
    {
        _sampleVectors.Clear();
        for (int i = 0; i < NumSampleVectors; i++)
        {
            var angle = 2 * Math.PI * i / NumSampleVectors;
            _sampleVectors.Add(Vec2.FromAngle(angle, 2));
        }
    }

    partial void OnM11Changed(double value) { Animate(); UpdateExplanation(); }
    partial void OnM12Changed(double value) { Animate(); UpdateExplanation(); }
    partial void OnM21Changed(double value) { Animate(); UpdateExplanation(); }
    partial void OnM22Changed(double value) { Animate(); UpdateExplanation(); }

    private void Animate()
    {
        if (_suppressUpdate) return;
        _animator.AnimateTo(Matrix);
        Sound.PlayWhoosh();
    }

    private void UpdateExplanation()
    {
        var mat = Matrix;
        var (l1, l2) = mat.Eigenvalues();
        var (v1, v2) = mat.Eigenvectors();

        var eigenInfo = "";
        if (l1.HasValue && l2.HasValue)
        {
            eigenInfo = $"Eigenvalues: lambda1 = {l1.Value:F2}, lambda2 = {l2.Value:F2}\n";
            if (v1.HasValue) eigenInfo += $"Eigenvector 1: ({v1.Value.X:F2}, {v1.Value.Y:F2})\n";
            if (v2.HasValue) eigenInfo += $"Eigenvector 2: ({v2.Value.X:F2}, {v2.Value.Y:F2})";
        }
        else
        {
            eigenInfo = "Complex eigenvalues (rotation-like transformation, no real eigenvectors)";
        }

        ExplanationText = $"Matrix: [{M11:F2} {M12:F2}; {M21:F2} {M22:F2}]\n{eigenInfo}";
    }

    public override void Render(SKCanvas canvas, SKSize size)
    {
        var mat = _animator.Current;

        // Draw all sample vectors before and after transformation
        foreach (var v in _sampleVectors)
        {
            var transformed = mat.Transform(v);
            var isEigen = IsAlongEigenvector(v, mat);

            // Original vector (faded)
            Vectors.DrawVector(canvas, Grid, v,
                ColorPalette.GridLabel.WithAlpha(40), null, 1.5f, dashed: true);

            // Transformed vector
            var color = isEigen ? ColorPalette.VectorEigen : ColorPalette.VectorA.WithAlpha(120);
            Vectors.DrawVector(canvas, Grid, transformed, color, null, isEigen ? 3f : 2f);
        }

        // Draw eigenvectors prominently
        if (ShowEigenvectors)
        {
            var (v1, v2) = mat.Eigenvectors();
            var (l1, l2) = mat.Eigenvalues();

            if (v1.HasValue && l1.HasValue)
            {
                var ev = v1.Value * 4; // Extend for visibility
                DrawEigenvectorLine(canvas, v1.Value, ColorPalette.VectorEigen, $"lambda={l1.Value:F1}");
            }

            if (v2.HasValue && l2.HasValue && (v1 == null || v1.Value.DistanceTo(v2.Value) > 0.1))
            {
                DrawEigenvectorLine(canvas, v2.Value, ColorPalette.VectorResult, $"lambda={l2.Value:F1}");
            }
        }
    }

    private void DrawEigenvectorLine(SKCanvas canvas, Vec2 direction, SKColor color, string label)
    {
        var ext = direction * 8;
        using var paint = new SKPaint
        {
            Color = color.WithAlpha(60),
            StrokeWidth = 2,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            PathEffect = SKPathEffect.CreateDash(new float[] { 12, 6 }, 0)
        };

        var p1 = Grid.WorldToScreen(-ext.X, -ext.Y);
        var p2 = Grid.WorldToScreen(ext.X, ext.Y);
        canvas.DrawLine(p1, p2, paint);

        Vectors.DrawVector(canvas, Grid, direction * 3, color, label, 3f);
    }

    private static bool IsAlongEigenvector(Vec2 v, Mat2 mat)
    {
        var (v1, v2) = mat.Eigenvectors();
        if (v1.HasValue && IsParallel(v.Normalized, v1.Value)) return true;
        if (v2.HasValue && IsParallel(v.Normalized, v2.Value)) return true;
        return false;
    }

    private static bool IsParallel(Vec2 a, Vec2 b)
        => Math.Abs(Vec2.Cross(a, b)) < 0.15;

    public void SetMatrixFields(double m11, double m12, double m21, double m22)
    {
        _suppressUpdate = true;
        M11 = m11; M12 = m12; M21 = m21;
        _suppressUpdate = false;
        M22 = m22;
    }

    public override void OnTick() => _animator.Tick();

    [RelayCommand]
    private void FindEigenvectors()
    {
        ShowEigenvectors = true;
        var (l1, l2) = Matrix.Eigenvalues();
        if (l1.HasValue) Sound.PlayCorrect();
        UpdateExplanation();
    }

    protected override void StartQuiz()
    {
        var q = Quiz.GenerateEigenQuestion();
        QuizPrompt = q.Prompt;
        QuizFeedback = "";
        ShowEigenvectors = false;
    }
}
