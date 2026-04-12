namespace LinearAlgebra.App.Models;

/// <summary>
/// Double-precision 2D vector for educational clarity.
/// </summary>
public readonly record struct Vec2(double X, double Y)
{
    public static readonly Vec2 Zero = new(0, 0);
    public static readonly Vec2 UnitX = new(1, 0);
    public static readonly Vec2 UnitY = new(0, 1);

    public double Magnitude => Math.Sqrt(X * X + Y * Y);
    public double AngleRadians => Math.Atan2(Y, X);
    public double AngleDegrees => AngleRadians * 180.0 / Math.PI;

    public Vec2 Normalized => Magnitude > 1e-10 ? this / Magnitude : Zero;

    public static Vec2 FromAngle(double radians, double magnitude = 1.0)
        => new(Math.Cos(radians) * magnitude, Math.Sin(radians) * magnitude);

    public static Vec2 operator +(Vec2 a, Vec2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Vec2 operator -(Vec2 a, Vec2 b) => new(a.X - b.X, a.Y - b.Y);
    public static Vec2 operator -(Vec2 v) => new(-v.X, -v.Y);
    public static Vec2 operator *(Vec2 v, double s) => new(v.X * s, v.Y * s);
    public static Vec2 operator *(double s, Vec2 v) => new(v.X * s, v.Y * s);
    public static Vec2 operator /(Vec2 v, double s) => new(v.X / s, v.Y / s);

    public static double Dot(Vec2 a, Vec2 b) => a.X * b.X + a.Y * b.Y;
    public static double Cross(Vec2 a, Vec2 b) => a.X * b.Y - a.Y * b.X;

    public double DistanceTo(Vec2 other) => (this - other).Magnitude;

    public override string ToString() => $"({X:F2}, {Y:F2})";
}
