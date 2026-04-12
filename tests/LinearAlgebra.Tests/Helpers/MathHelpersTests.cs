using FluentAssertions;
using LinearAlgebra.App.Helpers;

namespace LinearAlgebra.Tests.Helpers;

public class MathHelpersTests
{
    private const double Tolerance = 1e-10;

    // --- DegreesToRadians ---

    [Fact]
    public void DegreesToRadians_Zero_IsZero()
    {
        MathHelpers.DegreesToRadians(0).Should().Be(0);
    }

    [Fact]
    public void DegreesToRadians_180_IsPi()
    {
        MathHelpers.DegreesToRadians(180).Should().BeApproximately(Math.PI, Tolerance);
    }

    [Fact]
    public void DegreesToRadians_90_IsHalfPi()
    {
        MathHelpers.DegreesToRadians(90).Should().BeApproximately(Math.PI / 2, Tolerance);
    }

    [Fact]
    public void DegreesToRadians_360_IsTwoPi()
    {
        MathHelpers.DegreesToRadians(360).Should().BeApproximately(2 * Math.PI, Tolerance);
    }

    [Fact]
    public void DegreesToRadians_Negative_ReturnsNegative()
    {
        MathHelpers.DegreesToRadians(-90).Should().BeApproximately(-Math.PI / 2, Tolerance);
    }

    // --- RadiansToDegrees ---

    [Fact]
    public void RadiansToDegrees_Zero_IsZero()
    {
        MathHelpers.RadiansToDegrees(0).Should().Be(0);
    }

    [Fact]
    public void RadiansToDegrees_Pi_Is180()
    {
        MathHelpers.RadiansToDegrees(Math.PI).Should().BeApproximately(180, Tolerance);
    }

    [Fact]
    public void RadiansToDegrees_HalfPi_Is90()
    {
        MathHelpers.RadiansToDegrees(Math.PI / 2).Should().BeApproximately(90, Tolerance);
    }

    // --- Round-trip conversion ---

    [Theory]
    [InlineData(0)]
    [InlineData(45)]
    [InlineData(90)]
    [InlineData(180)]
    [InlineData(270)]
    [InlineData(-30)]
    public void DegreesToRadians_ThenBack_IsOriginal(double degrees)
    {
        MathHelpers.RadiansToDegrees(MathHelpers.DegreesToRadians(degrees))
            .Should().BeApproximately(degrees, Tolerance);
    }

    // --- Lerp ---

    [Fact]
    public void Lerp_AtZero_ReturnsA()
    {
        MathHelpers.Lerp(10, 20, 0).Should().BeApproximately(10, Tolerance);
    }

    [Fact]
    public void Lerp_AtOne_ReturnsB()
    {
        MathHelpers.Lerp(10, 20, 1).Should().BeApproximately(20, Tolerance);
    }

    [Fact]
    public void Lerp_AtHalf_ReturnsMidpoint()
    {
        MathHelpers.Lerp(0, 100, 0.5).Should().BeApproximately(50, Tolerance);
    }

    [Fact]
    public void Lerp_WithSameValues_ReturnsSame()
    {
        MathHelpers.Lerp(42, 42, 0.7).Should().BeApproximately(42, Tolerance);
    }

    [Fact]
    public void Lerp_WithNegativeValues()
    {
        MathHelpers.Lerp(-10, 10, 0.5).Should().BeApproximately(0, Tolerance);
    }

    [Fact]
    public void Lerp_Extrapolates_BeyondOne()
    {
        MathHelpers.Lerp(0, 10, 2).Should().BeApproximately(20, Tolerance);
    }

    [Fact]
    public void Lerp_Extrapolates_BelowZero()
    {
        MathHelpers.Lerp(0, 10, -1).Should().BeApproximately(-10, Tolerance);
    }

    // --- EaseInOutCubic ---

    [Fact]
    public void EaseInOutCubic_AtZero_IsZero()
    {
        MathHelpers.EaseInOutCubic(0).Should().BeApproximately(0, Tolerance);
    }

    [Fact]
    public void EaseInOutCubic_AtOne_IsOne()
    {
        MathHelpers.EaseInOutCubic(1).Should().BeApproximately(1, Tolerance);
    }

    [Fact]
    public void EaseInOutCubic_AtHalf_IsHalf()
    {
        MathHelpers.EaseInOutCubic(0.5).Should().BeApproximately(0.5, Tolerance);
    }

    [Fact]
    public void EaseInOutCubic_IsMonotonicallyIncreasing()
    {
        double prev = -1;
        for (int i = 0; i <= 100; i++)
        {
            double t = i / 100.0;
            double val = MathHelpers.EaseInOutCubic(t);
            val.Should().BeGreaterThanOrEqualTo(prev);
            prev = val;
        }
    }

    [Fact]
    public void EaseInOutCubic_FirstHalf_IsSlow()
    {
        // At t=0.25, eased value should be less than 0.25 (ease-in)
        MathHelpers.EaseInOutCubic(0.25).Should().BeLessThan(0.25);
    }

    [Fact]
    public void EaseInOutCubic_SecondHalf_IsFast()
    {
        // At t=0.75, eased value should be greater than 0.75 (ease-out)
        MathHelpers.EaseInOutCubic(0.75).Should().BeGreaterThan(0.75);
    }

    [Fact]
    public void EaseInOutCubic_IsSymmetric()
    {
        // f(t) + f(1-t) = 1 for symmetric ease curves
        for (double t = 0; t <= 0.5; t += 0.05)
        {
            var sum = MathHelpers.EaseInOutCubic(t) + MathHelpers.EaseInOutCubic(1 - t);
            sum.Should().BeApproximately(1, 1e-10);
        }
    }

    // --- SnapToGrid ---

    [Fact]
    public void SnapToGrid_ExactValue_ReturnsSame()
    {
        MathHelpers.SnapToGrid(3.0, 1.0).Should().BeApproximately(3.0, Tolerance);
    }

    [Fact]
    public void SnapToGrid_RoundsToNearest()
    {
        MathHelpers.SnapToGrid(2.3, 1.0).Should().BeApproximately(2.0, Tolerance);
        MathHelpers.SnapToGrid(2.7, 1.0).Should().BeApproximately(3.0, Tolerance);
    }

    [Fact]
    public void SnapToGrid_HalfGridSize_SnapsToHalf()
    {
        MathHelpers.SnapToGrid(1.3, 0.5).Should().BeApproximately(1.5, Tolerance);
        MathHelpers.SnapToGrid(1.1, 0.5).Should().BeApproximately(1.0, Tolerance);
    }

    [Fact]
    public void SnapToGrid_Zero_SnapsToZero()
    {
        MathHelpers.SnapToGrid(0.1, 1.0).Should().BeApproximately(0, Tolerance);
    }

    [Fact]
    public void SnapToGrid_NegativeValue_SnapsCorrectly()
    {
        MathHelpers.SnapToGrid(-2.7, 1.0).Should().BeApproximately(-3.0, Tolerance);
    }

    [Fact]
    public void SnapToGrid_LargeGridSize()
    {
        MathHelpers.SnapToGrid(7, 5.0).Should().BeApproximately(5.0, Tolerance);
        MathHelpers.SnapToGrid(8, 5.0).Should().BeApproximately(10.0, Tolerance);
    }
}
