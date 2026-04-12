using FluentAssertions;
using LinearAlgebra.App.Models;
using LinearAlgebra.App.Rendering;

namespace LinearAlgebra.Tests.Rendering;

public class TransformAnimatorTests
{
    private const double Tolerance = 1e-6;

    [Fact]
    public void Initial_Current_IsIdentity()
    {
        var animator = new TransformAnimator();
        animator.Current.Should().Be(Mat2.Identity);
    }

    [Fact]
    public void Initial_IsNotAnimating()
    {
        var animator = new TransformAnimator();
        animator.IsAnimating.Should().BeFalse();
    }

    // --- SetImmediate ---

    [Fact]
    public void SetImmediate_SetsCurrent()
    {
        var animator = new TransformAnimator();
        var target = Mat2.Scale(3);
        animator.SetImmediate(target);
        animator.Current.Should().Be(target);
    }

    [Fact]
    public void SetImmediate_StopsAnimation()
    {
        var animator = new TransformAnimator();
        animator.AnimateTo(Mat2.Scale(5), 10000);
        animator.IsAnimating.Should().BeTrue();

        animator.SetImmediate(Mat2.Scale(2));
        animator.IsAnimating.Should().BeFalse();
    }

    [Fact]
    public void SetImmediate_FiresUpdatedEvent()
    {
        var animator = new TransformAnimator();
        bool fired = false;
        animator.Updated += () => fired = true;

        animator.SetImmediate(Mat2.Scale(2));
        fired.Should().BeTrue();
    }

    // --- AnimateTo ---

    [Fact]
    public void AnimateTo_SetsAnimatingTrue()
    {
        var animator = new TransformAnimator();
        animator.AnimateTo(Mat2.Scale(2), 1000);
        animator.IsAnimating.Should().BeTrue();
    }

    // --- Tick ---

    [Fact]
    public void Tick_WhenNotAnimating_DoesNotFireEvent()
    {
        var animator = new TransformAnimator();
        bool fired = false;
        animator.Updated += () => fired = true;

        animator.Tick();
        fired.Should().BeFalse();
    }

    [Fact]
    public void Tick_AfterAnimationComplete_SetsIsAnimatingFalse()
    {
        var animator = new TransformAnimator();
        var target = Mat2.Scale(3);

        // Use a very short duration so it completes immediately
        animator.AnimateTo(target, 1);

        // Wait just a tiny bit then tick
        Thread.Sleep(10);
        animator.Tick();

        animator.IsAnimating.Should().BeFalse();
        animator.Current.M11.Should().BeApproximately(target.M11, Tolerance);
        animator.Current.M22.Should().BeApproximately(target.M22, Tolerance);
    }

    [Fact]
    public void Tick_FiresUpdatedEvent_WhenAnimating()
    {
        var animator = new TransformAnimator();
        animator.AnimateTo(Mat2.Scale(2), 1000);

        bool fired = false;
        animator.Updated += () => fired = true;

        animator.Tick();
        fired.Should().BeTrue();
    }

    [Fact]
    public void Tick_CompletedAnimation_SetsFinalValue()
    {
        var animator = new TransformAnimator();
        var target = new Mat2(5, 6, 7, 8);

        animator.AnimateTo(target, 1);
        Thread.Sleep(10);
        animator.Tick();

        animator.Current.M11.Should().BeApproximately(5, Tolerance);
        animator.Current.M12.Should().BeApproximately(6, Tolerance);
        animator.Current.M21.Should().BeApproximately(7, Tolerance);
        animator.Current.M22.Should().BeApproximately(8, Tolerance);
    }

    [Fact]
    public void AnimateTo_FromNonIdentity_AnimatesFromCurrent()
    {
        var animator = new TransformAnimator();
        animator.SetImmediate(Mat2.Scale(2));

        animator.AnimateTo(Mat2.Scale(4), 1);
        Thread.Sleep(10);
        animator.Tick();

        // After completion, should be at target
        animator.Current.M11.Should().BeApproximately(4, Tolerance);
        animator.Current.M22.Should().BeApproximately(4, Tolerance);
    }

    [Fact]
    public void Tick_MultipleCallsAfterCompletion_DoNothing()
    {
        var animator = new TransformAnimator();
        animator.AnimateTo(Mat2.Scale(3), 1);
        Thread.Sleep(10);
        animator.Tick();
        animator.IsAnimating.Should().BeFalse();

        int eventCount = 0;
        animator.Updated += () => eventCount++;

        animator.Tick();
        animator.Tick();
        eventCount.Should().Be(0);
    }
}
