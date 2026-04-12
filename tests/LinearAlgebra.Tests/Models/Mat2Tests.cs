using FluentAssertions;
using LinearAlgebra.App.Models;

namespace LinearAlgebra.Tests.Models;

public class Mat2Tests
{
    private const double Tolerance = 1e-10;
    private const double LooseTolerance = 1e-6;

    // --- Static fields ---

    [Fact]
    public void Identity_HasCorrectValues()
    {
        Mat2.Identity.M11.Should().Be(1);
        Mat2.Identity.M12.Should().Be(0);
        Mat2.Identity.M21.Should().Be(0);
        Mat2.Identity.M22.Should().Be(1);
    }

    [Fact]
    public void Zero_HasAllZeroes()
    {
        var z = Mat2.Zero;
        z.M11.Should().Be(0);
        z.M12.Should().Be(0);
        z.M21.Should().Be(0);
        z.M22.Should().Be(0);
    }

    // --- Determinant ---

    [Fact]
    public void Determinant_OfIdentity_IsOne()
    {
        Mat2.Identity.Determinant.Should().BeApproximately(1, Tolerance);
    }

    [Fact]
    public void Determinant_OfZeroMatrix_IsZero()
    {
        Mat2.Zero.Determinant.Should().Be(0);
    }

    [Fact]
    public void Determinant_KnownValue()
    {
        // |2 3; 1 4| = 2*4 - 3*1 = 5
        new Mat2(2, 3, 1, 4).Determinant.Should().BeApproximately(5, Tolerance);
    }

    [Fact]
    public void Determinant_OfSingularMatrix_IsZero()
    {
        // Rows are linearly dependent: [1 2; 2 4]
        new Mat2(1, 2, 2, 4).Determinant.Should().BeApproximately(0, Tolerance);
    }

    [Fact]
    public void Determinant_NegativeValue()
    {
        // |1 3; 2 1| = 1 - 6 = -5
        new Mat2(1, 3, 2, 1).Determinant.Should().BeApproximately(-5, Tolerance);
    }

    [Fact]
    public void Determinant_OfScale_IsProductOfScaleFactors()
    {
        Mat2.Scale(3, 5).Determinant.Should().BeApproximately(15, Tolerance);
    }

    [Fact]
    public void Determinant_OfRotation_IsOne()
    {
        Mat2.Rotation(Math.PI / 4).Determinant.Should().BeApproximately(1, Tolerance);
    }

    [Fact]
    public void Determinant_OfReflection_IsNegativeOne()
    {
        Mat2.ReflectionX.Determinant.Should().BeApproximately(-1, Tolerance);
        Mat2.ReflectionY.Determinant.Should().BeApproximately(-1, Tolerance);
    }

    // --- Trace ---

    [Fact]
    public void Trace_OfIdentity_IsTwo()
    {
        Mat2.Identity.Trace.Should().BeApproximately(2, Tolerance);
    }

    [Fact]
    public void Trace_SumOfDiagonal()
    {
        new Mat2(3, 7, 2, 5).Trace.Should().BeApproximately(8, Tolerance);
    }

    // --- Column/Row accessors ---

    [Fact]
    public void Columns_ExtractCorrectly()
    {
        var m = new Mat2(1, 2, 3, 4);
        m.Column1.Should().Be(new Vec2(1, 3));
        m.Column2.Should().Be(new Vec2(2, 4));
    }

    [Fact]
    public void Rows_ExtractCorrectly()
    {
        var m = new Mat2(1, 2, 3, 4);
        m.Row1.Should().Be(new Vec2(1, 2));
        m.Row2.Should().Be(new Vec2(3, 4));
    }

    // --- Transform ---

    [Fact]
    public void Transform_ByIdentity_ReturnsSameVector()
    {
        var v = new Vec2(3, 7);
        Mat2.Identity.Transform(v).Should().Be(v);
    }

    [Fact]
    public void Transform_ByZeroMatrix_ReturnsZero()
    {
        Mat2.Zero.Transform(new Vec2(5, 3)).Should().Be(Vec2.Zero);
    }

    [Fact]
    public void Transform_KnownResult()
    {
        // [2 1; 0 3] * (4, 1) = (2*4+1*1, 0*4+3*1) = (9, 3)
        var m = new Mat2(2, 1, 0, 3);
        var result = m.Transform(new Vec2(4, 1));
        result.X.Should().BeApproximately(9, Tolerance);
        result.Y.Should().BeApproximately(3, Tolerance);
    }

    [Fact]
    public void Transform_ZeroVector_ReturnsZero()
    {
        new Mat2(5, 3, 2, 7).Transform(Vec2.Zero).Should().Be(Vec2.Zero);
    }

    [Fact]
    public void OperatorMultiply_MatrixTimesVector_SameAsTransform()
    {
        var m = new Mat2(2, 1, 0, 3);
        var v = new Vec2(4, 1);
        (m * v).Should().Be(m.Transform(v));
    }

    // --- Matrix multiplication ---

    [Fact]
    public void MatrixMultiply_IdentityTimesA_ReturnsA()
    {
        var a = new Mat2(2, 3, 4, 5);
        (Mat2.Identity * a).Should().Be(a);
    }

    [Fact]
    public void MatrixMultiply_ATimesIdentity_ReturnsA()
    {
        var a = new Mat2(2, 3, 4, 5);
        (a * Mat2.Identity).Should().Be(a);
    }

    [Fact]
    public void MatrixMultiply_KnownResult()
    {
        // [1 2; 3 4] * [5 6; 7 8] = [1*5+2*7  1*6+2*8; 3*5+4*7  3*6+4*8]
        //                           = [19 22; 43 50]
        var a = new Mat2(1, 2, 3, 4);
        var b = new Mat2(5, 6, 7, 8);
        var result = a * b;
        result.M11.Should().BeApproximately(19, Tolerance);
        result.M12.Should().BeApproximately(22, Tolerance);
        result.M21.Should().BeApproximately(43, Tolerance);
        result.M22.Should().BeApproximately(50, Tolerance);
    }

    [Fact]
    public void MatrixMultiply_IsNotCommutative()
    {
        var a = new Mat2(1, 2, 3, 4);
        var b = new Mat2(5, 6, 7, 8);
        (a * b).Should().NotBe(b * a);
    }

    [Fact]
    public void MatrixMultiply_IsAssociative()
    {
        var a = new Mat2(1, 2, 3, 4);
        var b = new Mat2(5, 6, 7, 8);
        var c = new Mat2(2, 0, 1, 3);

        var left = (a * b) * c;
        var right = a * (b * c);

        left.M11.Should().BeApproximately(right.M11, Tolerance);
        left.M12.Should().BeApproximately(right.M12, Tolerance);
        left.M21.Should().BeApproximately(right.M21, Tolerance);
        left.M22.Should().BeApproximately(right.M22, Tolerance);
    }

    [Fact]
    public void MatrixMultiply_ZeroTimesA_ReturnsZero()
    {
        var a = new Mat2(2, 3, 4, 5);
        (Mat2.Zero * a).Should().Be(Mat2.Zero);
    }

    // --- Rotation ---

    [Fact]
    public void Rotation_Zero_IsIdentity()
    {
        var r = Mat2.Rotation(0);
        r.M11.Should().BeApproximately(1, Tolerance);
        r.M12.Should().BeApproximately(0, Tolerance);
        r.M21.Should().BeApproximately(0, Tolerance);
        r.M22.Should().BeApproximately(1, Tolerance);
    }

    [Fact]
    public void Rotation_90Degrees_RotatesUnitXToUnitY()
    {
        var r = Mat2.Rotation(Math.PI / 2);
        var result = r.Transform(Vec2.UnitX);
        result.X.Should().BeApproximately(0, LooseTolerance);
        result.Y.Should().BeApproximately(1, LooseTolerance);
    }

    [Fact]
    public void Rotation_90Degrees_RotatesUnitYToNegativeUnitX()
    {
        var r = Mat2.Rotation(Math.PI / 2);
        var result = r.Transform(Vec2.UnitY);
        result.X.Should().BeApproximately(-1, LooseTolerance);
        result.Y.Should().BeApproximately(0, LooseTolerance);
    }

    [Fact]
    public void Rotation_180Degrees_NegatesVector()
    {
        var r = Mat2.Rotation(Math.PI);
        var result = r.Transform(new Vec2(3, 4));
        result.X.Should().BeApproximately(-3, LooseTolerance);
        result.Y.Should().BeApproximately(-4, LooseTolerance);
    }

    [Fact]
    public void Rotation_360Degrees_ReturnsSame()
    {
        var r = Mat2.Rotation(2 * Math.PI);
        var v = new Vec2(3, 4);
        var result = r.Transform(v);
        result.X.Should().BeApproximately(v.X, LooseTolerance);
        result.Y.Should().BeApproximately(v.Y, LooseTolerance);
    }

    [Fact]
    public void Rotation_PreservesMagnitude()
    {
        var v = new Vec2(3, 4);
        var rotated = Mat2.Rotation(1.23).Transform(v);
        rotated.Magnitude.Should().BeApproximately(v.Magnitude, LooseTolerance);
    }

    [Fact]
    public void Rotation_DeterminantIsOne()
    {
        Mat2.Rotation(0.7).Determinant.Should().BeApproximately(1, Tolerance);
    }

    [Fact]
    public void Rotation_ComposesTwoRotations()
    {
        var r1 = Mat2.Rotation(Math.PI / 6);
        var r2 = Mat2.Rotation(Math.PI / 3);
        var combined = r1 * r2;
        var direct = Mat2.Rotation(Math.PI / 2);
        combined.M11.Should().BeApproximately(direct.M11, LooseTolerance);
        combined.M12.Should().BeApproximately(direct.M12, LooseTolerance);
        combined.M21.Should().BeApproximately(direct.M21, LooseTolerance);
        combined.M22.Should().BeApproximately(direct.M22, LooseTolerance);
    }

    // --- Scale ---

    [Fact]
    public void Scale_UniformByOne_IsIdentity()
    {
        Mat2.Scale(1).Should().Be(Mat2.Identity);
    }

    [Fact]
    public void Scale_Uniform_ScalesBothAxes()
    {
        var v = Mat2.Scale(3).Transform(new Vec2(2, 4));
        v.Should().Be(new Vec2(6, 12));
    }

    [Fact]
    public void Scale_NonUniform_ScalesIndependently()
    {
        var result = Mat2.Scale(2, 3).Transform(new Vec2(4, 5));
        result.Should().Be(new Vec2(8, 15));
    }

    [Fact]
    public void Scale_ByZero_CollapsesToOrigin()
    {
        Mat2.Scale(0).Transform(new Vec2(5, 7)).Should().Be(Vec2.Zero);
    }

    [Fact]
    public void Scale_Negative_Reflects()
    {
        var result = Mat2.Scale(-1, -1).Transform(new Vec2(3, 4));
        result.Should().Be(new Vec2(-3, -4));
    }

    // --- Shear ---

    [Fact]
    public void ShearX_Zero_IsIdentity()
    {
        Mat2.ShearX(0).Should().Be(Mat2.Identity);
    }

    [Fact]
    public void ShearX_TransformsCorrectly()
    {
        // ShearX(k): [1 k; 0 1] * (x,y) = (x+ky, y)
        var result = Mat2.ShearX(2).Transform(new Vec2(1, 3));
        result.X.Should().BeApproximately(7, Tolerance); // 1 + 2*3
        result.Y.Should().BeApproximately(3, Tolerance);
    }

    [Fact]
    public void ShearY_Zero_IsIdentity()
    {
        Mat2.ShearY(0).Should().Be(Mat2.Identity);
    }

    [Fact]
    public void ShearY_TransformsCorrectly()
    {
        // ShearY(k): [1 0; k 1] * (x,y) = (x, kx+y)
        var result = Mat2.ShearY(2).Transform(new Vec2(3, 1));
        result.X.Should().BeApproximately(3, Tolerance);
        result.Y.Should().BeApproximately(7, Tolerance); // 2*3 + 1
    }

    [Fact]
    public void Shear_PreservesDeterminant()
    {
        Mat2.ShearX(5).Determinant.Should().BeApproximately(1, Tolerance);
        Mat2.ShearY(-3).Determinant.Should().BeApproximately(1, Tolerance);
    }

    // --- Reflections ---

    [Fact]
    public void ReflectionX_FlipsYComponent()
    {
        var result = Mat2.ReflectionX.Transform(new Vec2(3, 4));
        result.Should().Be(new Vec2(3, -4));
    }

    [Fact]
    public void ReflectionY_FlipsXComponent()
    {
        var result = Mat2.ReflectionY.Transform(new Vec2(3, 4));
        result.Should().Be(new Vec2(-3, 4));
    }

    [Fact]
    public void ReflectionX_AppliedTwice_IsIdentity()
    {
        var rr = Mat2.ReflectionX * Mat2.ReflectionX;
        rr.M11.Should().BeApproximately(1, Tolerance);
        rr.M12.Should().BeApproximately(0, Tolerance);
        rr.M21.Should().BeApproximately(0, Tolerance);
        rr.M22.Should().BeApproximately(1, Tolerance);
    }

    [Fact]
    public void ReflectionLine_AtZero_IsReflectionX()
    {
        var r = Mat2.ReflectionLine(0);
        r.M11.Should().BeApproximately(1, LooseTolerance);
        r.M12.Should().BeApproximately(0, LooseTolerance);
        r.M21.Should().BeApproximately(0, LooseTolerance);
        r.M22.Should().BeApproximately(-1, LooseTolerance);
    }

    [Fact]
    public void ReflectionLine_At45Degrees_SwapsXY()
    {
        var r = Mat2.ReflectionLine(Math.PI / 4);
        var result = r.Transform(new Vec2(3, 0));
        result.X.Should().BeApproximately(0, LooseTolerance);
        result.Y.Should().BeApproximately(3, LooseTolerance);
    }

    [Fact]
    public void ReflectionLine_AppliedTwice_IsIdentity()
    {
        var r = Mat2.ReflectionLine(0.7);
        var rr = r * r;
        rr.M11.Should().BeApproximately(1, LooseTolerance);
        rr.M12.Should().BeApproximately(0, LooseTolerance);
        rr.M21.Should().BeApproximately(0, LooseTolerance);
        rr.M22.Should().BeApproximately(1, LooseTolerance);
    }

    // --- Inverse ---

    [Fact]
    public void Inverse_OfIdentity_IsIdentity()
    {
        Mat2.Identity.Inverse().Should().Be(Mat2.Identity);
    }

    [Fact]
    public void Inverse_TimesOriginal_IsIdentity()
    {
        var m = new Mat2(2, 3, 1, 4);
        var inv = m.Inverse();
        var product = m * inv;
        product.M11.Should().BeApproximately(1, LooseTolerance);
        product.M12.Should().BeApproximately(0, LooseTolerance);
        product.M21.Should().BeApproximately(0, LooseTolerance);
        product.M22.Should().BeApproximately(1, LooseTolerance);
    }

    [Fact]
    public void Inverse_OfSingularMatrix_Throws()
    {
        var singular = new Mat2(1, 2, 2, 4);
        var act = () => singular.Inverse();
        act.Should().Throw<InvalidOperationException>().WithMessage("*singular*");
    }

    [Fact]
    public void Inverse_OfZeroMatrix_Throws()
    {
        var act = () => Mat2.Zero.Inverse();
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Inverse_KnownValues()
    {
        // [2 1; 5 3] => det = 1, inverse = [3 -1; -5 2]
        var m = new Mat2(2, 1, 5, 3);
        var inv = m.Inverse();
        inv.M11.Should().BeApproximately(3, Tolerance);
        inv.M12.Should().BeApproximately(-1, Tolerance);
        inv.M21.Should().BeApproximately(-5, Tolerance);
        inv.M22.Should().BeApproximately(2, Tolerance);
    }

    [Fact]
    public void Inverse_OfRotation_IsReverseRotation()
    {
        var angle = 0.7;
        var r = Mat2.Rotation(angle);
        var inv = r.Inverse();
        var reverse = Mat2.Rotation(-angle);
        inv.M11.Should().BeApproximately(reverse.M11, LooseTolerance);
        inv.M12.Should().BeApproximately(reverse.M12, LooseTolerance);
        inv.M21.Should().BeApproximately(reverse.M21, LooseTolerance);
        inv.M22.Should().BeApproximately(reverse.M22, LooseTolerance);
    }

    // --- Transpose ---

    [Fact]
    public void Transpose_SwapsOffDiagonal()
    {
        var m = new Mat2(1, 2, 3, 4);
        var t = m.Transpose();
        t.Should().Be(new Mat2(1, 3, 2, 4));
    }

    [Fact]
    public void Transpose_OfIdentity_IsIdentity()
    {
        Mat2.Identity.Transpose().Should().Be(Mat2.Identity);
    }

    [Fact]
    public void Transpose_OfSymmetric_ReturnsSame()
    {
        var sym = new Mat2(1, 3, 3, 5);
        sym.Transpose().Should().Be(sym);
    }

    [Fact]
    public void Transpose_AppliedTwice_ReturnsSame()
    {
        var m = new Mat2(1, 2, 3, 4);
        m.Transpose().Transpose().Should().Be(m);
    }

    // --- Eigenvalues ---

    [Fact]
    public void Eigenvalues_OfIdentity_AreBothOne()
    {
        var (l1, l2) = Mat2.Identity.Eigenvalues();
        l1.Should().BeApproximately(1, Tolerance);
        l2.Should().BeApproximately(1, Tolerance);
    }

    [Fact]
    public void Eigenvalues_OfDiagonalMatrix_AreDiagonalEntries()
    {
        var (l1, l2) = new Mat2(3, 0, 0, 5).Eigenvalues();
        l1.Should().BeApproximately(5, Tolerance);
        l2.Should().BeApproximately(3, Tolerance);
    }

    [Fact]
    public void Eigenvalues_OfKnownSymmetricMatrix()
    {
        // [2 1; 1 2] => eigenvalues: (2+1)/2 +/- sqrt((4-3))/2 = 1.5 +/- 0.5 => 3, 1
        var (l1, l2) = new Mat2(2, 1, 1, 2).Eigenvalues();
        l1.Should().BeApproximately(3, Tolerance);
        l2.Should().BeApproximately(1, Tolerance);
    }

    [Fact]
    public void Eigenvalues_Lambda1_GreaterThanOrEqual_Lambda2()
    {
        var (l1, l2) = new Mat2(1, 4, 2, 3).Eigenvalues();
        l1!.Value.Should().BeGreaterThanOrEqualTo(l2!.Value);
    }

    [Fact]
    public void Eigenvalues_OfPureRotation_AreComplex()
    {
        // 90-degree rotation has no real eigenvalues
        var (l1, l2) = Mat2.Rotation(Math.PI / 2).Eigenvalues();
        l1.Should().BeNull();
        l2.Should().BeNull();
    }

    [Fact]
    public void Eigenvalues_OfReflection_AreOneAndNegativeOne()
    {
        var (l1, l2) = Mat2.ReflectionX.Eigenvalues();
        l1.Should().BeApproximately(1, Tolerance);
        l2.Should().BeApproximately(-1, Tolerance);
    }

    [Fact]
    public void Eigenvalues_OfZeroMatrix_AreBothZero()
    {
        var (l1, l2) = Mat2.Zero.Eigenvalues();
        l1.Should().BeApproximately(0, Tolerance);
        l2.Should().BeApproximately(0, Tolerance);
    }

    [Fact]
    public void Eigenvalues_OfScale_AreScaleFactors()
    {
        var (l1, l2) = Mat2.Scale(7, 2).Eigenvalues();
        l1.Should().BeApproximately(7, Tolerance);
        l2.Should().BeApproximately(2, Tolerance);
    }

    // --- Eigenvectors ---

    [Fact]
    public void Eigenvectors_OfDiagonalMatrix_AreAxisAligned()
    {
        var (v1, v2) = new Mat2(3, 0, 0, 5).Eigenvectors();
        // Eigenvector for lambda=5: should be along Y axis
        // Eigenvector for lambda=3: should be along X axis
        v1.Should().NotBeNull();
        v2.Should().NotBeNull();

        // Verify they are actual eigenvectors: M*v = lambda*v
        var m = new Mat2(3, 0, 0, 5);
        var (l1, l2) = m.Eigenvalues();
        var mv1 = m.Transform(v1!.Value);
        var ev1 = v1.Value * l1!.Value;
        mv1.X.Should().BeApproximately(ev1.X, LooseTolerance);
        mv1.Y.Should().BeApproximately(ev1.Y, LooseTolerance);
    }

    [Fact]
    public void Eigenvectors_OfSymmetricMatrix_AreOthogonal()
    {
        var m = new Mat2(2, 1, 1, 2);
        var (v1, v2) = m.Eigenvectors();
        v1.Should().NotBeNull();
        v2.Should().NotBeNull();
        Vec2.Dot(v1!.Value, v2!.Value).Should().BeApproximately(0, LooseTolerance);
    }

    [Fact]
    public void Eigenvectors_VerifyEigenvectorEquation()
    {
        // For M = [2 1; 1 2], eigenvalues 3 and 1
        // Eigenvector for 3: (1,1)/sqrt(2), eigenvector for 1: (-1,1)/sqrt(2)
        var m = new Mat2(2, 1, 1, 2);
        var (v1, v2) = m.Eigenvectors();
        var (l1, l2) = m.Eigenvalues();

        // M * v1 should equal l1 * v1
        var mv1 = m.Transform(v1!.Value);
        var scaled1 = v1.Value * l1!.Value;
        mv1.X.Should().BeApproximately(scaled1.X, LooseTolerance);
        mv1.Y.Should().BeApproximately(scaled1.Y, LooseTolerance);

        // M * v2 should equal l2 * v2
        var mv2 = m.Transform(v2!.Value);
        var scaled2 = v2.Value * l2!.Value;
        mv2.X.Should().BeApproximately(scaled2.X, LooseTolerance);
        mv2.Y.Should().BeApproximately(scaled2.Y, LooseTolerance);
    }

    [Fact]
    public void Eigenvectors_OfPureRotation_AreNull()
    {
        var (v1, v2) = Mat2.Rotation(Math.PI / 2).Eigenvectors();
        v1.Should().BeNull();
        v2.Should().BeNull();
    }

    [Fact]
    public void Eigenvectors_AreNormalized()
    {
        var (v1, v2) = new Mat2(2, 1, 1, 2).Eigenvectors();
        v1!.Value.Magnitude.Should().BeApproximately(1, LooseTolerance);
        v2!.Value.Magnitude.Should().BeApproximately(1, LooseTolerance);
    }

    // --- Lerp ---

    [Fact]
    public void Lerp_AtZero_ReturnsFirst()
    {
        var a = new Mat2(1, 2, 3, 4);
        var b = new Mat2(5, 6, 7, 8);
        Mat2.Lerp(a, b, 0).Should().Be(a);
    }

    [Fact]
    public void Lerp_AtOne_ReturnsSecond()
    {
        var a = new Mat2(1, 2, 3, 4);
        var b = new Mat2(5, 6, 7, 8);
        Mat2.Lerp(a, b, 1).Should().Be(b);
    }

    [Fact]
    public void Lerp_AtHalf_ReturnsMidpoint()
    {
        var a = new Mat2(0, 0, 0, 0);
        var b = new Mat2(4, 8, 12, 16);
        var mid = Mat2.Lerp(a, b, 0.5);
        mid.M11.Should().BeApproximately(2, Tolerance);
        mid.M12.Should().BeApproximately(4, Tolerance);
        mid.M21.Should().BeApproximately(6, Tolerance);
        mid.M22.Should().BeApproximately(8, Tolerance);
    }

    [Fact]
    public void Lerp_IdentityToIdentity_AlwaysIdentity()
    {
        Mat2.Lerp(Mat2.Identity, Mat2.Identity, 0.5).Should().Be(Mat2.Identity);
    }

    // --- ToString ---

    [Fact]
    public void ToString_FormatsCorrectly()
    {
        new Mat2(1, 2, 3, 4).ToString().Should().Be("[1.00 2.00; 3.00 4.00]");
    }
}
