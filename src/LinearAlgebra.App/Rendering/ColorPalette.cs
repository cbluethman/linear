using SkiaSharp;

namespace LinearAlgebra.App.Rendering;

public static class ColorPalette
{
    // Dark theme colors
    public static readonly SKColor Background = new(30, 30, 40);
    public static readonly SKColor GridLine = new(60, 60, 80);
    public static readonly SKColor GridAxis = new(120, 120, 150);
    public static readonly SKColor GridLabel = new(160, 160, 180);

    // Vector colors
    public static readonly SKColor VectorA = new(80, 160, 255);       // Blue
    public static readonly SKColor VectorB = new(100, 220, 100);      // Green
    public static readonly SKColor VectorResult = new(255, 100, 100); // Red
    public static readonly SKColor VectorEigen = new(255, 200, 50);   // Gold

    // Shape colors
    public static readonly SKColor ShapeOriginal = new(80, 160, 255, 60);
    public static readonly SKColor ShapeTransformed = new(255, 100, 100, 100);
    public static readonly SKColor ShapeOutline = new(200, 200, 220);

    // UI colors
    public static readonly SKColor TextPrimary = new(230, 230, 240);
    public static readonly SKColor TextSecondary = new(160, 160, 180);
    public static readonly SKColor Correct = new(80, 200, 120);
    public static readonly SKColor Wrong = new(255, 80, 80);
    public static readonly SKColor Highlight = new(255, 200, 50, 80);

    // Basis vectors
    public static readonly SKColor BasisI = new(255, 80, 80);  // i-hat red
    public static readonly SKColor BasisJ = new(80, 255, 80);  // j-hat green

    // Light theme overrides
    public static class Light
    {
        public static readonly SKColor Background = new(245, 245, 250);
        public static readonly SKColor GridLine = new(210, 210, 220);
        public static readonly SKColor GridAxis = new(100, 100, 120);
        public static readonly SKColor GridLabel = new(80, 80, 100);
        public static readonly SKColor TextPrimary = new(30, 30, 40);
        public static readonly SKColor TextSecondary = new(80, 80, 100);
    }
}
