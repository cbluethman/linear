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
    private double _dpiScale = 1.0;

    // Mouse interaction state
    private bool _isPanning;
    private System.Windows.Point _lastMousePos;

    public GridRenderer? Grid { get; private set; }

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

    public void SetGrid(GridRenderer grid)
    {
        Grid = grid;
    }

    public void SetRenderer(ISceneRenderer renderer)
    {
        _renderer = renderer;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        // Detect DPI scale for correct coordinate mapping
        var source = PresentationSource.FromVisual(this);
        _dpiScale = source?.CompositionTarget?.TransformToDevice.M11 ?? 1.0;

        Grid?.SetCenter((float)(ActualWidth * _dpiScale), (float)(ActualHeight * _dpiScale));
        StartRenderLoop();
    }

    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        _renderLoopActive = false;
        CompositionTarget.Rendering -= OnCompositionTargetRendering;
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
        if (Grid == null) return;

        var canvas = e.Surface.Canvas;
        var size = new SKSize(e.Info.Width, e.Info.Height);

        Grid.Render(canvas, size);
        _renderer?.Render(canvas, size);
    }

    private void OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (Grid == null) return;
        var pos = e.GetPosition(this);
        var sx = (float)(pos.X * _dpiScale);
        var sy = (float)(pos.Y * _dpiScale);

        if (e.MiddleButton == MouseButtonState.Pressed)
        {
            _isPanning = true;
            _lastMousePos = pos;
            CaptureMouse();
            return;
        }

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var worldX = Grid.ScreenToWorldX(sx);
            var worldY = Grid.ScreenToWorldY(sy);
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
        if (Grid == null) return;
        var pos = e.GetPosition(this);

        if (_isPanning)
        {
            var dx = (float)((pos.X - _lastMousePos.X) * _dpiScale);
            var dy = (float)((pos.Y - _lastMousePos.Y) * _dpiScale);
            Grid.Pan(dx, dy);
            _lastMousePos = pos;
            return;
        }

        if (e.LeftButton == MouseButtonState.Pressed)
        {
            var sx = (float)(pos.X * _dpiScale);
            var sy = (float)(pos.Y * _dpiScale);
            var worldX = Grid.ScreenToWorldX(sx);
            var worldY = Grid.ScreenToWorldY(sy);
            WorldMouseMove?.Invoke(worldX, worldY);
        }
    }

    private void OnMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if (Grid == null) return;
        var pos = e.GetPosition(this);
        var factor = e.Delta > 0 ? 1.1f : 0.9f;
        Grid.Zoom(factor, (float)(pos.X * _dpiScale), (float)(pos.Y * _dpiScale));
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        base.OnRenderSizeChanged(sizeInfo);
        Grid?.SetCenter((float)(ActualWidth * _dpiScale), (float)(ActualHeight * _dpiScale));
    }
}
