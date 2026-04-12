using FluentAssertions;
using LinearAlgebra.App.Models;

namespace LinearAlgebra.Tests.Models;

public class Vec2Tests
{
    private const double Tolerance = 1e-10;

    // --- Construction and static fields ---

    [Fact]
    public void Constructor_SetsComponents()
    {
        var v = new Vec2(3, -4);
        v.X.Should().Be(3);
        v.Y.Should().Be(-4);
    }

    [Fact]
    public void Zero_IsOrigin()
    {
        Vec2.Zero.X.Should().Be(0);
        Vec2.Zero.Y.Should().Be(0);
    }

    [Fact]
    public void UnitX_IsOneZero()
    {
        Vec2.UnitX.Should().Be(new Vec2(1, 0));
    }

    [Fact]
    public void UnitY_IsZeroOne()
    {
        Vec2.UnitY.Should().Be(new Vec2(0, 1));
    }

    // --- Magnitude ---

    [Fact]
    public void Magnitude_Of345Triangle_Is5()
    {
        new Vec2(3, 4).Magnitude.Should().BeApproximately(5.0, Tolerance);
    }

    [Fact]
    public void Magnitude_OfZeroVector_IsZero()
    {
        Vec2.Zero.Magnitude.Should().Be(0);
    }

    [Fact]
    public void Magnitude_OfUnitX_IsOne()
    {
        Vec2.UnitX.Magnitude.Should().BeApproximately(1.0, Tolerance);
    }

    [Fact]
    public void Magnitude_OfNegativeComponents_IsPositive()
    {
        new Vec2(-3, -4).Magnitude.Should().BeApproximately(5.0, Tolerance);
    }

    // --- Angle ---

    [Fact]
    public void AngleRadians_OfUnitX_IsZero()
    {
        Vec2.UnitX.AngleRadians.Should().BeApproximately(0, Tolerance);
    }

    [Fact]
    public void AngleRadians_OfUnitY_IsHalfPi()
    {
        Vec2.UnitY.AngleRadians.Should().BeApproximately(Math.PI / 2, Tolerance);
    }

    [Fact]
    public void AngleDegrees_OfUnitY_Is90()
    {
        Vec2.UnitY.AngleDegrees.Should().BeApproximately(90.0, Tolerance);
    }

    [Fact]
    public void AngleDegrees_OfNegativeX_Is180()
    {
        new Vec2(-1, 0).AngleDegrees.Should().BeApproximately(180.0, Tolerance);
    }

    [Fact]
    public void AngleDegrees_Of45DegreeVector_Is45()
    {
        new Vec2(1, 1).AngleDegrees.Should().BeApproximately(45.0, Tolerance);
    }

    // --- Normalization ---

    [Fact]
    public void Normalized_HasMagnitudeOne()
    {
        var n = new Vec2(3, 4).Normalized;
        n.Magnitude.Should().BeApproximately(1.0, Tolerance);
    }

    [Fact]
    public void Normalized_PreservesDirection()
    {
        var v = new Vec2(3, 4);
        var n = v.Normalized;
        n.X.Should().BeApproximately(0.6, Tolerance);
        n.Y.Should().BeApproximately(0.8, Tolerance);
    }

    [Fact]
    public void Normalized_OfZeroVector_ReturnsZero()
    {
        Vec2.Zero.Normalized.Should().Be(Vec2.Zero);
    }

    [Fact]
    public void Normalized_OfVerySmallVector_ReturnsZero()
    {
        new Vec2(1e-15, 1e-15).Normalized.Should().Be(Vec2.Zero);
    }

    [Fact]
    public void Normalized_OfUnitVector_ReturnsSame()
    {
        var n = Vec2.UnitX.Normalized;
        n.X.Should().BeApproximately(1.0, Tolerance);
        n.Y.Should().BeApproximately(0.0, Tolerance);
    }

    // --- FromAngle ---

    [Fact]
    public void FromAngle_ZeroRadians_ReturnsUnitX()
    {
        var v = Vec2.FromAngle(0);
        v.X.Should().BeApproximately(1.0, Tolerance);
        v.Y.Should().BeApproximately(0.0, Tolerance);
    }

    [Fact]
    public void FromAngle_HalfPi_ReturnsUnitY()
    {
        var v = Vec2.FromAngle(Math.PI / 2);
        v.X.Should().BeApproximately(0.0, Tolerance);
        v.Y.Should().BeApproximately(1.0, Tolerance);
    }

    [Fact]
    public void FromAngle_WithMagnitude_ScalesResult()
    {
        var v = Vec2.FromAngle(0, 5.0);
        v.X.Should().BeApproximately(5.0, Tolerance);
        v.Y.Should().BeApproximately(0.0, Tolerance);
    }

    [Fact]
    public void FromAngle_Pi_ReturnsNegativeX()
    {
        var v = Vec2.FromAngle(Math.PI);
        v.X.Should().BeApproximately(-1.0, Tolerance);
        v.Y.Should().BeApproximately(0.0, Tolerance);
    }

    // --- Arithmetic operators ---

    [Fact]
    public void Addition_SumsComponents()
    {
        var result = new Vec2(1, 2) + new Vec2(3, 4);
        result.Should().Be(new Vec2(4, 6));
    }

    [Fact]
    public void Addition_WithZero_ReturnsOriginal()
    {
        var v = new Vec2(3, 4);
        (v + Vec2.Zero).Should().Be(v);
    }

    [Fact]
    public void Subtraction_SubtractsComponents()
    {
        var result = new Vec2(5, 7) - new Vec2(2, 3);
        result.Should().Be(new Vec2(3, 4));
    }

    [Fact]
    public void Subtraction_FromSelf_ReturnsZero()
    {
        var v = new Vec2(3, 4);
        (v - v).Should().Be(Vec2.Zero);
    }

    [Fact]
    public void Negation_NegatesComponents()
    {
        var v = -new Vec2(3, -4);
        v.Should().Be(new Vec2(-3, 4));
    }

    [Fact]
    public void Negation_OfZero_IsZero()
    {
        (-Vec2.Zero).Should().Be(Vec2.Zero);
    }

    [Fact]
    public void ScalarMultiplication_VectorTimesScalar()
    {
        var result = new Vec2(2, 3) * 3;
        result.Should().Be(new Vec2(6, 9));
    }

    [Fact]
    public void ScalarMultiplication_ScalarTimesVector()
    {
        var result = 3.0 * new Vec2(2, 3);
        result.Should().Be(new Vec2(6, 9));
    }

    [Fact]
    public void ScalarMultiplication_ByZero_ReturnsZero()
    {
        (new Vec2(5, 7) * 0).Should().Be(Vec2.Zero);
    }

    [Fact]
    public void ScalarMultiplication_ByNegativeOne_Negates()
    {
        (new Vec2(3, 4) * -1).Should().Be(new Vec2(-3, -4));
    }

    [Fact]
    public void Division_DividesComponents()
    {
        var result = new Vec2(6, 9) / 3;
        result.Should().Be(new Vec2(2, 3));
    }

    [Fact]
    public void Division_ByOne_ReturnsSame()
    {
        var v = new Vec2(3, 4);
        (v / 1).Should().Be(v);
    }

    // --- Dot product ---

    [Fact]
    public void Dot_OfPerpendicularVectors_IsZero()
    {
        Vec2.Dot(Vec2.UnitX, Vec2.UnitY).Should().BeApproximately(0, Tolerance);
    }

    [Fact]
    public void Dot_OfParallelVectors_IsProductOfMagnitudes()
    {
        Vec2.Dot(new Vec2(3, 0), new Vec2(4, 0)).Should().BeApproximately(12, Tolerance);
    }

    [Fact]
    public void Dot_OfAntiParallelVectors_IsNegative()
    {
        Vec2.Dot(new Vec2(1, 0), new Vec2(-1, 0)).Should().BeApproximately(-1, Tolerance);
    }

    [Fact]
    public void Dot_IsCommutative()
    {
        var a = new Vec2(2, 3);
        var b = new Vec2(4, -1);
        Vec2.Dot(a, b).Should().BeApproximately(Vec2.Dot(b, a), Tolerance);
    }

    [Fact]
    public void Dot_WithSelf_IsMagnitudeSquared()
    {
        var v = new Vec2(3, 4);
        Vec2.Dot(v, v).Should().BeApproximately(25.0, Tolerance);
    }

    [Fact]
    public void Dot_WithZero_IsZero()
    {
        Vec2.Dot(new Vec2(5, 7), Vec2.Zero).Should().Be(0);
    }

    [Fact]
    public void Dot_GeneralCase()
    {
        // (2,3) . (4,-1) = 8 - 3 = 5
        Vec2.Dot(new Vec2(2, 3), new Vec2(4, -1)).Should().BeApproximately(5, Tolerance);
    }

    // --- Cross product ---

    [Fact]
    public void Cross_OfParallelVectors_IsZero()
    {
        Vec2.Cross(new Vec2(2, 4), new Vec2(1, 2)).Should().BeApproximately(0, Tolerance);
    }

    [Fact]
    public void Cross_OfPerpendicularVectors_IsNonZero()
    {
        // UnitX x UnitY = 1*1 - 0*0 = 1
        Vec2.Cross(Vec2.UnitX, Vec2.UnitY).Should().BeApproximately(1, Tolerance);
    }

    [Fact]
    public void Cross_IsAntiCommutative()
    {
        var a = new Vec2(2, 3);
        var b = new Vec2(4, -1);
        Vec2.Cross(a, b).Should().BeApproximately(-Vec2.Cross(b, a), Tolerance);
    }

    [Fact]
    public void Cross_GeneralCase()
    {
        // (2,3) x (4,-1) = 2*(-1) - 3*4 = -2 - 12 = -14
        Vec2.Cross(new Vec2(2, 3), new Vec2(4, -1)).Should().BeApproximately(-14, Tolerance);
    }

    // --- DistanceTo ---

    [Fact]
    public void DistanceTo_SamePoint_IsZero()
    {
        var v = new Vec2(3, 4);
        v.DistanceTo(v).Should().Be(0);
    }

    [Fact]
    public void DistanceTo_IsSymmetric()
    {
        var a = new Vec2(1, 2);
        var b = new Vec2(4, 6);
        a.DistanceTo(b).Should().BeApproximately(b.DistanceTo(a), Tolerance);
    }

    [Fact]
    public void DistanceTo_KnownTriangle()
    {
        var a = new Vec2(0, 0);
        var b = new Vec2(3, 4);
        a.DistanceTo(b).Should().BeApproximately(5, Tolerance);
    }

    [Fact]
    public void DistanceTo_FromOrigin_EqualsMagnitude()
    {
        var v = new Vec2(5, 12);
        Vec2.Zero.DistanceTo(v).Should().BeApproximately(v.Magnitude, Tolerance);
    }

    // --- ToString ---

    [Fact]
    public void ToString_FormatsWithTwoDecimals()
    {
        new Vec2(1.5, -2.75).ToString().Should().Be("(1.50, -2.75)");
    }

    // --- Record equality ---

    [Fact]
    public void Equality_SameComponents_AreEqual()
    {
        new Vec2(1, 2).Should().Be(new Vec2(1, 2));
    }

    [Fact]
    public void Equality_DifferentComponents_AreNotEqual()
    {
        new Vec2(1, 2).Should().NotBe(new Vec2(2, 1));
    }
}
