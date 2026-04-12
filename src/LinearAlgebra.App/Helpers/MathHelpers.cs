namespace LinearAlgebra.App.Helpers;

public static class MathHelpers
{
    public static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;
    public static double RadiansToDegrees(double radians) => radians * 180.0 / Math.PI;
    public static double Lerp(double a, double b, double t) => a + (b - a) * t;

    /// <summary>
    /// Cubic ease-in-out for smooth animations.
    /// </summary>
    public static double EaseInOutCubic(double t)
    {
        if (t < 0.5) return 4 * t * t * t;
        var p = 2 * t - 2;
        return 0.5 * p * p * p + 1;
    }

    /// <summary>
    /// Snap value to nearest grid increment.
    /// </summary>
    public static double SnapToGrid(double value, double gridSize)
        => Math.Round(value / gridSize) * gridSize;
}
