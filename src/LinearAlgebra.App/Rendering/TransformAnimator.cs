using LinearAlgebra.App.Helpers;
using LinearAlgebra.App.Models;

namespace LinearAlgebra.App.Rendering;

public class TransformAnimator
{
    private Mat2 _from;
    private Mat2 _to;
    private DateTime _startTime;
    private double _durationMs;
    private bool _animating;

    public Mat2 Current { get; private set; } = Mat2.Identity;
    public bool IsAnimating => _animating;

    public event Action? Updated;

    public void AnimateTo(Mat2 target, double durationMs = 500)
    {
        _from = Current;
        _to = target;
        _durationMs = durationMs;
        _startTime = DateTime.UtcNow;
        _animating = true;
    }

    public void SetImmediate(Mat2 value)
    {
        Current = value;
        _animating = false;
        Updated?.Invoke();
    }

    public void Tick()
    {
        if (!_animating) return;

        var elapsed = (DateTime.UtcNow - _startTime).TotalMilliseconds;
        var t = Math.Clamp(elapsed / _durationMs, 0, 1);
        var eased = MathHelpers.EaseInOutCubic(t);

        Current = Mat2.Lerp(_from, _to, eased);

        if (t >= 1.0)
        {
            Current = _to;
            _animating = false;
        }

        Updated?.Invoke();
    }
}
