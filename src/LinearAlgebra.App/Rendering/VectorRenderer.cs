using LinearAlgebra.App.Models;
using SkiaSharp;

namespace LinearAlgebra.App.Rendering;

public class VectorRenderer
{
    private const float ArrowHeadLength = 12f;
    private const float ArrowHeadAngle = 25f * MathF.PI / 180f;

    public void DrawVector(SKCanvas canvas, GridRenderer grid, Vec2 from, Vec2 to,
        SKColor color, string? label = null, float strokeWidth = 3f, bool dashed = false)
    {
        var screenFrom = grid.WorldToScreen(from.X, from.Y);
        var screenTo = grid.WorldToScreen(to.X, to.Y);

        using var paint = new SKPaint
        {
            Color = color,
            StrokeWidth = strokeWidth,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeCap = SKStrokeCap.Round
        };

        if (dashed)
            paint.PathEffect = SKPathEffect.CreateDash(new float[] { 8, 6 }, 0);

        // Draw shaft
        canvas.DrawLine(screenFrom, screenTo, paint);

        // Draw arrowhead
        var dx = screenTo.X - screenFrom.X;
        var dy = screenTo.Y - screenFrom.Y;
        var length = MathF.Sqrt(dx * dx + dy * dy);

        if (length < 5f) return;

        var angle = MathF.Atan2(dy, dx);
        var headLen = Math.Min(ArrowHeadLength, length * 0.4f);

        paint.PathEffect = null;
        paint.Style = SKPaintStyle.Fill;

        using var path = new SKPath();
        path.MoveTo(screenTo);
        path.LineTo(
            screenTo.X - headLen * MathF.Cos(angle - ArrowHeadAngle),
            screenTo.Y - headLen * MathF.Sin(angle - ArrowHeadAngle));
        path.LineTo(
            screenTo.X - headLen * MathF.Cos(angle + ArrowHeadAngle),
            screenTo.Y - headLen * MathF.Sin(angle + ArrowHeadAngle));
        path.Close();
        canvas.DrawPath(path, paint);

        // Draw label
        if (label != null)
        {
            using var labelPaint = new SKPaint
            {
                Color = color,
                TextSize = 14,
                IsAntialias = true,
                Typeface = SKTypeface.FromFamilyName("Consolas"),
                Style = SKPaintStyle.Fill
            };

            var midX = (screenFrom.X + screenTo.X) / 2;
            var midY = (screenFrom.Y + screenTo.Y) / 2;

            // Offset label perpendicular to vector direction
            var perpX = -dy / length * 14;
            var perpY = dx / length * 14;

            canvas.DrawText(label, midX + perpX, midY + perpY, labelPaint);
        }
    }

    public void DrawVector(SKCanvas canvas, GridRenderer grid, Vec2 v,
        SKColor color, string? label = null, float strokeWidth = 3f, bool dashed = false)
        => DrawVector(canvas, grid, Vec2.Zero, v, color, label, strokeWidth, dashed);

    public void DrawComponentLines(SKCanvas canvas, GridRenderer grid, Vec2 v, SKColor color)
    {
        var xComp = new Vec2(v.X, 0);
        var yComp = new Vec2(0, v.Y);

        DrawVector(canvas, grid, Vec2.Zero, xComp, color.WithAlpha(100), null, 2f, dashed: true);
        DrawVector(canvas, grid, xComp, v, color.WithAlpha(100), null, 2f, dashed: true);
    }

    public bool HitTestArrowHead(GridRenderer grid, Vec2 vectorTip, float screenX, float screenY, float threshold = 15f)
    {
        var tipScreen = grid.WorldToScreen(vectorTip.X, vectorTip.Y);
        var dx = tipScreen.X - screenX;
        var dy = tipScreen.Y - screenY;
        return dx * dx + dy * dy <= threshold * threshold;
    }
}
