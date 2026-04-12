using FluentAssertions;
using LinearAlgebra.App.Rendering;

namespace LinearAlgebra.Tests.Rendering;

public class GridRendererTests
{
    private const double Tolerance = 1e-6;

    private GridRenderer CreateCentered(float width = 800, float height = 600)
    {
        var grid = new GridRenderer();
        grid.SetCenter(width, height);
        return grid;
    }

    // --- Initial state ---

    [Fact]
    public void Initial_PixelsPerUnit_Is50()
    {
        new GridRenderer().PixelsPerUnit.Should().Be(50f);
    }

    [Fact]
    public void Initial_Origin_IsZero()
    {
        var grid = new GridRenderer();
        grid.OriginX.Should().Be(0);
        grid.OriginY.Should().Be(0);
    }

    // --- SetCenter ---

    [Fact]
    public void SetCenter_SetsOriginToMiddle()
    {
        var grid = CreateCentered(800, 600);
        grid.OriginX.Should().Be(400);
        grid.OriginY.Should().Be(300);
    }

    [Fact]
    public void SetCenter_OddDimensions()
    {
        var grid = CreateCentered(801, 601);
        grid.OriginX.Should().BeApproximately(400.5f, 0.01f);
        grid.OriginY.Should().BeApproximately(300.5f, 0.01f);
    }

    // --- WorldToScreen ---

    [Fact]
    public void WorldToScreen_Origin_MapsToCenter()
    {
        var grid = CreateCentered(800, 600);
        grid.WorldToScreenX(0).Should().BeApproximately(400, Tolerance);
        grid.WorldToScreenY(0).Should().BeApproximately(300, Tolerance);
    }

    [Fact]
    public void WorldToScreenX_PositiveX_MovesRight()
    {
        var grid = CreateCentered(800, 600);
        // 1 world unit = 50 pixels to the right
        grid.WorldToScreenX(1).Should().BeApproximately(450, Tolerance);
    }

    [Fact]
    public void WorldToScreenY_PositiveY_MovesUp()
    {
        var grid = CreateCentered(800, 600);
        // Positive Y in world goes UP, which is negative in screen coords
        grid.WorldToScreenY(1).Should().BeApproximately(250, Tolerance);
    }

    [Fact]
    public void WorldToScreenY_NegativeY_MovesDown()
    {
        var grid = CreateCentered(800, 600);
        grid.WorldToScreenY(-1).Should().BeApproximately(350, Tolerance);
    }

    [Fact]
    public void WorldToScreen_ReturnsCorrectPoint()
    {
        var grid = CreateCentered(800, 600);
        var pt = grid.WorldToScreen(2, 3);
        pt.X.Should().BeApproximately(500, Tolerance); // 400 + 2*50
        pt.Y.Should().BeApproximately(150, Tolerance); // 300 - 3*50
    }

    // --- ScreenToWorld ---

    [Fact]
    public void ScreenToWorld_Center_MapsToOrigin()
    {
        var grid = CreateCentered(800, 600);
        grid.ScreenToWorldX(400).Should().BeApproximately(0, Tolerance);
        grid.ScreenToWorldY(300).Should().BeApproximately(0, Tolerance);
    }

    [Fact]
    public void ScreenToWorldX_RightOfCenter_IsPositive()
    {
        var grid = CreateCentered(800, 600);
        grid.ScreenToWorldX(450).Should().BeApproximately(1, Tolerance);
    }

    [Fact]
    public void ScreenToWorldY_AboveCenter_IsPositive()
    {
        var grid = CreateCentered(800, 600);
        // Screen Y 250 is above center 300 => positive world Y
        grid.ScreenToWorldY(250).Should().BeApproximately(1, Tolerance);
    }

    // --- Round-trip: WorldToScreen then ScreenToWorld ---

    [Theory]
    [InlineData(0, 0)]
    [InlineData(3.5, -2.7)]
    [InlineData(-4, 5)]
    [InlineData(10, 10)]
    public void WorldToScreen_ThenScreenToWorld_IsRoundTrip(double wx, double wy)
    {
        var grid = CreateCentered(800, 600);
        var sx = grid.WorldToScreenX(wx);
        var sy = grid.WorldToScreenY(wy);
        grid.ScreenToWorldX(sx).Should().BeApproximately(wx, Tolerance);
        grid.ScreenToWorldY(sy).Should().BeApproximately(wy, Tolerance);
    }

    // --- Pan ---

    [Fact]
    public void Pan_ShiftsOrigin()
    {
        var grid = CreateCentered(800, 600);
        grid.Pan(10, -20);
        grid.OriginX.Should().BeApproximately(410, Tolerance);
        grid.OriginY.Should().BeApproximately(280, Tolerance);
    }

    [Fact]
    public void Pan_AffectsWorldToScreen()
    {
        var grid = CreateCentered(800, 600);
        grid.Pan(100, 0);
        // Origin moved right by 100. World (0,0) now at screen (500, 300)
        grid.WorldToScreenX(0).Should().BeApproximately(500, Tolerance);
    }

    [Fact]
    public void Pan_MultipleCalls_Accumulate()
    {
        var grid = CreateCentered(800, 600);
        grid.Pan(10, 20);
        grid.Pan(30, 40);
        grid.OriginX.Should().BeApproximately(440, Tolerance);
        grid.OriginY.Should().BeApproximately(360, Tolerance);
    }

    // --- Zoom ---

    [Fact]
    public void Zoom_IncreasesPixelsPerUnit()
    {
        var grid = CreateCentered(800, 600);
        var original = grid.PixelsPerUnit;
        grid.Zoom(1.5f, 400, 300);
        grid.PixelsPerUnit.Should().BeGreaterThan(original);
    }

    [Fact]
    public void Zoom_DecreasesPixelsPerUnit_WithFactorLessThanOne()
    {
        var grid = CreateCentered(800, 600);
        var original = grid.PixelsPerUnit;
        grid.Zoom(0.5f, 400, 300);
        grid.PixelsPerUnit.Should().BeLessThan(original);
    }

    [Fact]
    public void Zoom_ClampsMinimum()
    {
        var grid = CreateCentered(800, 600);
        // Zoom way out
        grid.Zoom(0.01f, 400, 300);
        grid.PixelsPerUnit.Should().BeGreaterThanOrEqualTo(10f);
    }

    [Fact]
    public void Zoom_ClampsMaximum()
    {
        var grid = CreateCentered(800, 600);
        grid.Zoom(100f, 400, 300);
        grid.PixelsPerUnit.Should().BeLessThanOrEqualTo(200f);
    }

    [Fact]
    public void Zoom_AtCenter_KeepsCenterWorldPointFixed()
    {
        var grid = CreateCentered(800, 600);
        // Zoom centered on screen center (400, 300). That's world (0,0).
        // After zoom, world (0,0) should still map to screen (400, 300).
        grid.Zoom(2f, 400, 300);
        grid.WorldToScreenX(0).Should().BeApproximately(400, 1);
        grid.WorldToScreenY(0).Should().BeApproximately(300, 1);
    }

    [Fact]
    public void Zoom_OffCenter_KeepsFocusPointFixed()
    {
        var grid = CreateCentered(800, 600);
        float focusX = 500, focusY = 200;
        var worldBeforeX = grid.ScreenToWorldX(focusX);
        var worldBeforeY = grid.ScreenToWorldY(focusY);

        grid.Zoom(1.5f, focusX, focusY);

        // The focus point in screen space should still map to the same world point
        grid.ScreenToWorldX(focusX).Should().BeApproximately(worldBeforeX, 0.01);
        grid.ScreenToWorldY(focusY).Should().BeApproximately(worldBeforeY, 0.01);
    }

    // --- Theme ---

    [Fact]
    public void SetTheme_DoesNotThrow()
    {
        var grid = new GridRenderer();
        var act = () =>
        {
            grid.SetTheme(true);
            grid.SetTheme(false);
        };
        act.Should().NotThrow();
    }
}
