using LinearAlgebra.App.Models;
using SkiaSharp;

namespace LinearAlgebra.App.Rendering;

public class ShapeRenderer
{
    private static readonly Vec2[] UnitSquare =
    {
        new(0, 0), new(1, 0), new(1, 1), new(0, 1)
    };

    public void DrawPolygon(SKCanvas canvas, GridRenderer grid, Vec2[] vertices,
        SKColor fillColor, SKColor outlineColor, float outlineWidth = 2f)
    {
        if (vertices.Length < 2) return;

        using var fillPaint = new SKPaint
        {
            Color = fillColor,
            IsAntialias = true,
            Style = SKPaintStyle.Fill
        };

        using var outlinePaint = new SKPaint
        {
            Color = outlineColor,
            StrokeWidth = outlineWidth,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke,
            StrokeJoin = SKStrokeJoin.Round
        };

        using var path = new SKPath();
        var first = grid.WorldToScreen(vertices[0].X, vertices[0].Y);
        path.MoveTo(first);

        for (int i = 1; i < vertices.Length; i++)
        {
            var pt = grid.WorldToScreen(vertices[i].X, vertices[i].Y);
            path.LineTo(pt);
        }
        path.Close();

        canvas.DrawPath(path, fillPaint);
        canvas.DrawPath(path, outlinePaint);
    }

    public void DrawUnitSquare(SKCanvas canvas, GridRenderer grid, Mat2 transform,
        SKColor fillColor, SKColor outlineColor)
    {
        var transformed = UnitSquare.Select(v => transform.Transform(v)).ToArray();
        DrawPolygon(canvas, grid, transformed, fillColor, outlineColor);
    }

    public void DrawUnitSquareOriginal(SKCanvas canvas, GridRenderer grid)
    {
        DrawPolygon(canvas, grid, UnitSquare, ColorPalette.ShapeOriginal, ColorPalette.ShapeOutline, 1f);
    }

    public void DrawParallelogram(SKCanvas canvas, GridRenderer grid, Vec2 v1, Vec2 v2,
        SKColor fillColor, SKColor outlineColor)
    {
        var vertices = new[]
        {
            Vec2.Zero, v1, v1 + v2, v2
        };
        DrawPolygon(canvas, grid, vertices, fillColor, outlineColor);
    }

    public void DrawLine(SKCanvas canvas, GridRenderer grid, SKSize canvasSize,
        double a, double b, double c, SKColor color, float strokeWidth = 2f)
    {
        // Line: ax + by = c
        // Find two points on the line within visible range
        var worldLeft = grid.ScreenToWorldX(0);
        var worldRight = grid.ScreenToWorldX(canvasSize.Width);

        SKPoint p1, p2;

        if (Math.Abs(b) > 1e-10)
        {
            var y1 = (c - a * worldLeft) / b;
            var y2 = (c - a * worldRight) / b;
            p1 = grid.WorldToScreen(worldLeft, y1);
            p2 = grid.WorldToScreen(worldRight, y2);
        }
        else if (Math.Abs(a) > 1e-10)
        {
            var x = c / a;
            p1 = grid.WorldToScreen(x, -100);
            p2 = grid.WorldToScreen(x, 100);
        }
        else return;

        using var paint = new SKPaint
        {
            Color = color,
            StrokeWidth = strokeWidth,
            IsAntialias = true,
            Style = SKPaintStyle.Stroke
        };

        canvas.DrawLine(p1, p2, paint);
    }
}
