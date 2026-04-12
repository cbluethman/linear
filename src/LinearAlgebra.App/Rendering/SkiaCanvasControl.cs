using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace LinearAlgebra.App.Rendering;

public class SkiaCanvasControl : SKElement
{
    private ISceneRenderer? _renderer;
    private bool _renderLoopActive;

    // Mouse interaction state
    private bool _isPanning;
    private System.Windows.Point _lastMousePos;

    public GridRenderer Grid { get; } = new();

    public event Action<double, double>? WorldClick;
    public event Action<double, double>? WorldMouseDown;
    public event Action<double, double>? WorldMouseMove;
    public event Action? WorldMouseUp;

    public SkiaCanvasControl()
    {
        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
        MouseDown += OnMouseDown;
        MouseUp += OnMouseUp;
        MouseMove += OnMouseMove;
        MouseWheel += OnMouseWheel;
    }

    public void SetRenderer(ISceneRenderer renderer)
    {
        _renderer = renderer;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        Grid.SetCenter((float)ActualWidth, (float)ActualHeight);
        StartRenderLoop();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _renderLoopActive = false;
    }

    private void StartRenderLoop()
    {
        if (_renderLoopActive) return;
        _renderLoopActive = true;
        CompositionTarget.Rendering += OnCompositionTargetRendering;
    }

    private void OnCompositionTargetRendering(object? sender, EventArgs e)
    {
        if (!_renderLoopActive) return;
        InvalidateVisual();
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        base.OnPaintSurface(e);
        var canvas = e.Surface.Canvas;
        var size = new SKSize(e.Info.Width, e.Info.Height);

        Grid.Render(canvas, size);
        _renderer?.Render(canvas, size);
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        var pos = e.GetPosition(this);

        if (e.MiddleButton == MouseButtonState.Pressed)
        {
            _isPanning = true;
            _lastMousePos = pos;
            CaptureMouse();
            return;
        }

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var worldX = Grid.ScreenToWorldX((float)pos.X);
            var worldY = Grid.ScreenToWorldY((float)pos.Y);
            WorldMouseDown?.Invoke(worldX, worldY);
        }
    }

    private void OnMouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_isPanning && e.MiddleButton == MouseButtonState.Released)
        {
            _isPanning = false;
            ReleaseMouseCapture();
            return;
        }

        if (e.LeftButton == MouseButtonState.Released)
        {
            WorldMouseUp?.Invoke();
        }
    }

    private void OnMouseMove(object sender, MouseEventArgs e)
    {
        var pos = e.GetPosition(this);

        if (_isPanning)
        {
            var dx = (float)(pos.X - _lastMousePos.X);
            var dy = (float)(pos.Y - _lastMousePos.Y);
            Grid.Pan(dx, dy);
            _lastMousePos = pos;
            return;
        }

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var worldX = Grid.ScreenToWorldX((float)pos.X);
            var worldY = Grid.ScreenToWorldY((float)pos.Y);
            WorldMouseMove?.Invoke(worldX, worldY);
        }
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var pos = e.GetPosition(this);
        var factor = e.Delta > 0 ? 1.1f : 0.9f;
        Grid.Zoom(factor, (float)pos.X, (float)pos.Y);
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        Grid.SetCenter((float)ActualWidth, (float)ActualHeight);
    }
}
