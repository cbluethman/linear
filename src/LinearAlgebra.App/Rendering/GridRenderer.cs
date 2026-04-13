using SkiaSharp;

namespace LinearAlgebra.App.Rendering;

public class GridRenderer
{
    private static readonly SKTypeface ConsolasTypeface = SKTypeface.FromFamilyName("Consolas");
    private float _pixelsPerUnit = 50f;
    private float _originX;
    private float _originY;
    private bool _isDarkTheme = true;

    public float PixelsPerUnit => _pixelsPerUnit;
    public float OriginX => _originX;
    public float OriginY => _originY;

    public void SetTheme(bool isDark) => _isDarkTheme = isDark;

    public void SetCenter(float canvasWidth, float canvasHeight)
    {
        _originX = canvasWidth / 2;
        _originY = canvasHeight / 2;
    }

    public void Pan(float deltaX, float deltaY)
    {
        _originX += deltaX;
        _originY += deltaY;
    }

    public void Zoom(float factor, float focusX, float focusY)
    {
        var worldX = ScreenToWorldX(focusX);
        var worldY = ScreenToWorldY(focusY);

        _pixelsPerUnit *= factor;
        _pixelsPerUnit = Math.Clamp(_pixelsPerUnit, 10f, 200f);

        _originX = focusX - (float)(worldX * _pixelsPerUnit);
        _originY = focusY + (float)(worldY * _pixelsPerUnit);
    }

    public float WorldToScreenX(double worldX) => _originX + (float)(worldX * _pixelsPerUnit);
    public float WorldToScreenY(double worldY) => _originY - (float)(worldY * _pixelsPerUnit);
    public SKPoint WorldToScreen(double worldX, double worldY) => new(WorldToScreenX(worldX), WorldToScreenY(worldY));

    public double ScreenToWorldX(float screenX) => (screenX - _originX) / _pixelsPerUnit;
    public double ScreenToWorldY(float screenY) => (_originY - screenY) / _pixelsPerUnit;

    public void Render(SKCanvas canvas, SKSize size)
    {
        var bgColor = _isDarkTheme ? ColorPalette.Background : ColorPalette.Light.Background;
        canvas.Clear(bgColor);

        var gridColor = _isDarkTheme ? ColorPalette.GridLine : ColorPalette.Light.GridLine;
        var axisColor = _isDarkTheme ? ColorPalette.GridAxis : ColorPalette.Light.GridAxis;
        var labelColor = _isDarkTheme ? ColorPalette.GridLabel : ColorPalette.Light.GridLabel;

        using var gridPaint = new SKPaint
        {
            Color = gridColor,
            StrokeWidth = 1,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        using var axisPaint = new SKPaint
        {
            Color = axisColor,
            StrokeWidth = 2,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        using var labelPaint = new SKPaint
        {
            Color = labelColor,
            TextSize = 12,
            IsAntialias = true,
            Typeface = ConsolasTypeface
        };

        // Determine visible range in world coordinates
        var worldLeft = ScreenToWorldX(0);
        var worldRight = ScreenToWorldX(size.Width);
        var worldTop = ScreenToWorldY(0);
        var worldBottom = ScreenToWorldY(size.Height);

        var minX = (int)Math.Floor(Math.Min(worldLeft, worldRight)) - 1;
        var maxX = (int)Math.Ceiling(Math.Max(worldLeft, worldRight)) + 1;
        var minY = (int)Math.Floor(Math.Min(worldBottom, worldTop)) - 1;
        var maxY = (int)Math.Ceiling(Math.Max(worldBottom, worldTop)) + 1;

        // Draw grid lines
        for (int x = minX; x <= maxX; x++)
        {
            var sx = WorldToScreenX(x);
            canvas.DrawLine(sx, 0, sx, size.Height, x == 0 ? axisPaint : gridPaint);

            if (x != 0)
            {
                var label = x.ToString();
                var labelWidth = labelPaint.MeasureText(label);
                canvas.DrawText(label, sx - labelWidth / 2, WorldToScreenY(0) + 16, labelPaint);
            }
        }

        for (int y = minY; y <= maxY; y++)
        {
            var sy = WorldToScreenY(y);
            canvas.DrawLine(0, sy, size.Width, sy, y == 0 ? axisPaint : gridPaint);

            if (y != 0)
            {
                var label = y.ToString();
                canvas.DrawText(label, WorldToScreenX(0) + 6, sy + 4, labelPaint);
            }
        }

        // Origin label
        canvas.DrawText("0", WorldToScreenX(0) + 6, WorldToScreenY(0) + 16, labelPaint);
    }
}
