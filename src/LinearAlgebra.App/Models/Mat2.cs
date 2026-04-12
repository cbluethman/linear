namespace LinearAlgebra.App.Models;

/// <summary>
/// Double-precision 2x2 matrix for educational linear algebra.
/// Layout: | M11 M12 |
///         | M21 M22 |
/// </summary>
public readonly record struct Mat2(double M11, double M12, double M21, double M22)
{
    public static readonly Mat2 Identity = new(1, 0, 0, 1);
    public static readonly Mat2 Zero = new(0, 0, 0, 0);

    public double Determinant => M11 * M22 - M12 * M21;
    public double Trace => M11 + M22;

    public Vec2 Column1 => new(M11, M21);
    public Vec2 Column2 => new(M12, M22);
    public Vec2 Row1 => new(M11, M12);
    public Vec2 Row2 => new(M21, M22);

    public Vec2 Transform(Vec2 v) => new(M11 * v.X + M12 * v.Y, M21 * v.X + M22 * v.Y);

    public static Mat2 operator *(Mat2 a, Mat2 b) => new(
        a.M11 * b.M11 + a.M12 * b.M21, a.M11 * b.M12 + a.M12 * b.M22,
        a.M21 * b.M11 + a.M22 * b.M21, a.M21 * b.M12 + a.M22 * b.M22);

    public static Vec2 operator *(Mat2 m, Vec2 v) => m.Transform(v);

    public static Mat2 Rotation(double radians)
    {
        var c = Math.Cos(radians);
        var s = Math.Sin(radians);
        return new(c, -s, s, c);
    }

    public static Mat2 Scale(double sx, double sy) => new(sx, 0, 0, sy);
    public static Mat2 Scale(double s) => Scale(s, s);

    public static Mat2 ShearX(double k) => new(1, k, 0, 1);
    public static Mat2 ShearY(double k) => new(1, 0, k, 1);

    public static Mat2 ReflectionX => new(1, 0, 0, -1);
    public static Mat2 ReflectionY => new(-1, 0, 0, 1);

    public static Mat2 ReflectionLine(double angleRadians)
    {
        var c = Math.Cos(2 * angleRadians);
        var s = Math.Sin(2 * angleRadians);
        return new(c, s, s, -c);
    }

    public Mat2 Inverse()
    {
        var det = Determinant;
        if (Math.Abs(det) < 1e-12) throw new InvalidOperationException("Matrix is singular.");
        return new(M22 / det, -M12 / det, -M21 / det, M11 / det);
    }

    public Mat2 Transpose() => new(M11, M21, M12, M22);

    /// <summary>
    /// Computes eigenvalues of a 2x2 matrix using the characteristic polynomial.
    /// Returns (lambda1, lambda2) where lambda1 >= lambda2.
    /// Returns null components for complex eigenvalues.
    /// </summary>
    public (double? Lambda1, double? Lambda2) Eigenvalues()
    {
        var t = Trace;
        var d = Determinant;
        var discriminant = t * t - 4 * d;

        if (discriminant < -1e-12)
            return (null, null);

        discriminant = Math.Max(0, discriminant);
        var sqrtD = Math.Sqrt(discriminant);
        var l1 = (t + sqrtD) / 2;
        var l2 = (t - sqrtD) / 2;
        return (l1, l2);
    }

    /// <summary>
    /// Computes eigenvectors for real eigenvalues.
    /// </summary>
    public (Vec2? V1, Vec2? V2) Eigenvectors()
    {
        var (l1, l2) = Eigenvalues();
        return (l1.HasValue ? EigenvectorFor(l1.Value) : null,
                l2.HasValue ? EigenvectorFor(l2.Value) : null);
    }

    private Vec2? EigenvectorFor(double lambda)
    {
        var a = M11 - lambda;
        var b = M12;
        var c = M21;
        var d = M22 - lambda;

        if (Math.Abs(b) > 1e-12) return new Vec2(-b, a).Normalized;
        if (Math.Abs(d) > 1e-12) return new Vec2(d, -c).Normalized;
        if (Math.Abs(a) > 1e-12) return new Vec2(0, 1);
        return new Vec2(1, 0);
    }

    public static Mat2 Lerp(Mat2 a, Mat2 b, double t)
        => new(a.M11 + (b.M11 - a.M11) * t, a.M12 + (b.M12 - a.M12) * t,
               a.M21 + (b.M21 - a.M21) * t, a.M22 + (b.M22 - a.M22) * t);

    public override string ToString() => $"[{M11:F2} {M12:F2}; {M21:F2} {M22:F2}]";
}
