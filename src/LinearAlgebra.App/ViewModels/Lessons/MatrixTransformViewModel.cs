using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LinearAlgebra.App.Models;
using LinearAlgebra.App.Rendering;
using LinearAlgebra.App.Services;
using SkiaSharp;

namespace LinearAlgebra.App.ViewModels.Lessons;

public partial class MatrixTransformViewModel : LessonViewModelBase
{
    private readonly TransformAnimator _animator = new();

    [ObservableProperty] private double _m11 = 1;
    [ObservableProperty] private double _m12 = 0;
    [ObservableProperty] private double _m21 = 0;
    [ObservableProperty] private double _m22 = 1;
    [ObservableProperty] private double _rotationAngle;

    public override string Title => "Matrix Transformations";

    public Mat2 CurrentMatrix => _animator.Current;

    public MatrixTransformViewModel(GridRenderer grid, SoundService sound) : base(grid, sound)
    {
        _animator.Updated += () => OnPropertyChanged(nameof(CurrentMatrix));
        UpdateExplanation();
    }

    partial void OnM11Changed(double value) => ApplyManualMatrix();
    partial void OnM12Changed(double value) => ApplyManualMatrix();
    partial void OnM21Changed(double value) => ApplyManualMatrix();
    partial void OnM22Changed(double value) => ApplyManualMatrix();

    private void ApplyManualMatrix()
    {
        var mat = new Mat2(M11, M12, M21, M22);
        _animator.AnimateTo(mat);
        Sound.PlayWhoosh();
        UpdateExplanation();
    }

    private void UpdateExplanation()
    {
        var mat = new Mat2(M11, M12, M21, M22);
        ExplanationText = $"Matrix: [{M11:F2} {M12:F2}; {M21:F2} {M22:F2}]\n" +
                         $"Determinant: {mat.Determinant:F2}\n" +
                         $"i-hat maps to ({M11:F2}, {M21:F2}), j-hat maps to ({M12:F2}, {M22:F2})";
    }

    [RelayCommand]
    private void ApplyRotation()
    {
        var rad = RotationAngle * Math.PI / 180;
        var mat = Mat2.Rotation(rad);
        SetMatrixFields(mat);
    }

    [RelayCommand]
    private void ApplyScale(string param)
    {
        var parts = param.Split(',');
        var sx = double.Parse(parts[0]);
        var sy = double.Parse(parts[1]);
        SetMatrixFields(Mat2.Scale(sx, sy));
    }

    [RelayCommand]
    private void ApplyShearX(double k) => SetMatrixFields(Mat2.ShearX(k));

    [RelayCommand]
    private void ApplyShearY(double k) => SetMatrixFields(Mat2.ShearY(k));

    [RelayCommand]
    private void ApplyReflectX() => SetMatrixFields(Mat2.ReflectionX);

    [RelayCommand]
    private void ApplyReflectY() => SetMatrixFields(Mat2.ReflectionY);

    [RelayCommand]
    private void ApplyIdentity() => SetMatrixFields(Mat2.Identity);

    private void SetMatrixFields(Mat2 mat)
    {
        M11 = mat.M11; M12 = mat.M12;
        M21 = mat.M21; M22 = mat.M22;
    }

    public override void Render(SKCanvas canvas, SKSize size)
    {
        var mat = _animator.Current;

        // Draw original unit square
        Shapes.DrawUnitSquareOriginal(canvas, Grid);

        // Draw transformed unit square
        Shapes.DrawUnitSquare(canvas, Grid, mat, ColorPalette.ShapeTransformed, ColorPalette.VectorResult);

        // Draw basis vectors (original)
        Vectors.DrawVector(canvas, Grid, Vec2.UnitX, ColorPalette.BasisI.WithAlpha(80), "i", 2f, dashed: true);
        Vectors.DrawVector(canvas, Grid, Vec2.UnitY, ColorPalette.BasisJ.WithAlpha(80), "j", 2f, dashed: true);

        // Draw transformed basis vectors
        var iHat = mat.Transform(Vec2.UnitX);
        var jHat = mat.Transform(Vec2.UnitY);
        Vectors.DrawVector(canvas, Grid, iHat, ColorPalette.BasisI, "i'");
        Vectors.DrawVector(canvas, Grid, jHat, ColorPalette.BasisJ, "j'");
    }

    public override void OnTick() => _animator.Tick();

    protected override void StartQuiz()
    {
        var q = Quiz.GenerateMatrixTransformQuestion();
        QuizPrompt = q.Prompt;
        QuizFeedback = "";
    }
}
