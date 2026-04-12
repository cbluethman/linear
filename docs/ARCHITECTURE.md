# Architecture

Technical architecture of the Linear Algebra Visual Learning application -- a .NET 8 WPF desktop app that renders interactive linear algebra lessons on a SkiaSharp canvas.

## Tech Stack and Dependencies

| Package | Version | Purpose |
|---------|---------|---------|
| .NET 8 (WPF) | `net8.0-windows` | Application framework |
| [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet) | 8.2.2 | MVVM source generators (`[ObservableProperty]`, `[RelayCommand]`, `ObservableObject`) |
| [SkiaSharp](https://github.com/mono/SkiaSharp) | 2.88.7 | 2D rendering engine (canvas, paints, paths) |
| SkiaSharp.Views.WPF | 2.88.7 | `SKElement` WPF control for hosting SkiaSharp surfaces |
| [MathNet.Numerics](https://numerics.mathdotnet.com/) | 5.0.0 | Numerical computing (available for future use; current eigen/determinant math is hand-rolled in `Mat2`) |
| [NAudio](https://github.com/naudio/NAudio) | 2.2.1 | WAV playback for UI sound effects |

Testing: xUnit 2.6.6, FluentAssertions 6.12.0, coverlet 6.0.0.

## MVVM Pattern

The application uses CommunityToolkit.Mvvm with **DataTemplate-based view resolution** and **manual dependency injection** (no DI container).

### Startup Wiring

`App.OnStartup` creates services and the root ViewModel by hand:

```csharp
// App.xaml.cs
var navigationService = new NavigationService();
var lessonService = new LessonService();
var soundService = new SoundService();
var mainViewModel = new MainViewModel(navigationService, lessonService, soundService);
```

`MainWindow.Initialize(mainViewModel)` sets `DataContext` and wires canvas events.

### View Resolution

Lesson views are resolved implicitly via `DataTemplate` in `Resources/DataTemplates.xaml`. A `ContentControl` in `MainWindow.xaml` binds to `ActiveLesson` (typed as `LessonViewModelBase`); WPF picks the matching `DataTemplate` at runtime:

```xml
<!-- MainWindow.xaml -->
<ContentControl Grid.Column="1" Content="{Binding ActiveLesson}" />
```

```xml
<!-- DataTemplates.xaml -->
<DataTemplate DataType="{x:Type lessons:VectorBasicsViewModel}">
    <views:VectorBasicsView />
</DataTemplate>
<!-- ... one per lesson -->
```

This means adding a new lesson requires registering a new `DataTemplate` mapping.

### ViewModel Hierarchy

```
MainViewModel (root)
  +-- NavigationViewModel         (sidebar lesson list)
  +-- LessonViewModelBase         (abstract, implements ISceneRenderer)
        +-- VectorBasicsViewModel
        +-- VectorOperationsViewModel
        +-- LinearCombinationViewModel
        +-- MatrixTransformViewModel
        +-- DeterminantViewModel
        +-- EigenViewModel
        +-- SystemsOfEquationsViewModel
```

`MainViewModel` owns a `GridRenderer` instance shared by all lessons. When `NavigationService.LessonChanged` fires, `MainViewModel.CreateLessonViewModel` instantiates the correct subclass via a `switch` expression on the lesson ID string.

### Source Generators

All ViewModels derive from `ObservableObject`. Properties use `[ObservableProperty]` (generates `OnPropertyChanged` calls) and commands use `[RelayCommand]` (generates `ICommand` implementations). Partial methods like `partial void OnVectorXChanged(double value)` hook into property-change notifications.

## Rendering Pipeline

All visual output goes through SkiaSharp. The pipeline has three layers:

```
CompositionTarget.Rendering (WPF frame callback)
  |
  v
SkiaCanvasControl.InvalidateVisual()
  |
  v
OnPaintSurface(SKPaintSurfaceEventArgs)
  |
  +-- GridRenderer.Render(canvas, size)      // grid lines, axes, labels
  +-- ISceneRenderer.Render(canvas, size)    // active lesson draws vectors/shapes
```

### SkiaCanvasControl

`SkiaCanvasControl` extends `SKElement` (from SkiaSharp.Views.WPF). Key responsibilities:

- **Render loop**: On `Loaded`, registers for `CompositionTarget.Rendering` and calls `InvalidateVisual()` every frame. On `Unloaded`, unregisters.
- **DPI awareness**: Reads `PresentationSource.CompositionTarget.TransformToDevice.M11` on load. All mouse positions are multiplied by `_dpiScale` before conversion.
- **Input dispatch**: Translates screen-space mouse events into world-space coordinates via `GridRenderer.ScreenToWorldX/Y` and fires `WorldMouseDown`, `WorldMouseMove`, `WorldMouseUp`.
- **Panning**: Middle-mouse drag translates the grid origin (`GridRenderer.Pan`).
- **Zooming**: Mouse wheel calls `GridRenderer.Zoom` with a 1.1x/0.9x factor, clamped to [10, 200] pixels-per-unit.

### GridRenderer

`GridRenderer` is the coordinate system authority. It owns three pieces of state:

| Field | Default | Purpose |
|-------|---------|---------|
| `_pixelsPerUnit` | 50 | Zoom level |
| `_originX` | canvas width / 2 | Screen-space X of world origin |
| `_originY` | canvas height / 2 | Screen-space Y of world origin |

**Coordinate conversion** (world Y-up, screen Y-down):

```csharp
WorldToScreenX(worldX) = _originX + worldX * _pixelsPerUnit
WorldToScreenY(worldY) = _originY - worldY * _pixelsPerUnit   // note the minus
ScreenToWorldX(screenX) = (screenX - _originX) / _pixelsPerUnit
ScreenToWorldY(screenY) = (_originY - screenY) / _pixelsPerUnit
```

The `Render` method draws integer grid lines across the visible range, with thicker axis lines at x=0 and y=0, and numeric labels in Consolas 12pt.

### VectorRenderer

Draws arrows from one `Vec2` to another with:
- Configurable color, stroke width, dashed style
- Triangular arrowhead (25-degree half-angle, length capped at 40% of shaft)
- Optional perpendicular label at the midpoint
- `DrawComponentLines` for dashed x/y component projections
- `HitTestArrowHead` for drag interaction (15px screen-space threshold)

### ShapeRenderer

Draws filled+outlined polygons in world coordinates. Helpers include:
- `DrawUnitSquare(canvas, grid, transform)` -- transforms the unit square [0,1]x[0,1] through a `Mat2`
- `DrawParallelogram(canvas, grid, v1, v2)` -- parallelogram from origin along two vectors
- `DrawLine(canvas, grid, size, a, b, c)` -- infinite line `ax + by = c` clipped to visible range

### ColorPalette

Static color constants for both dark and light themes. The grid renderer reads `_isDarkTheme` to pick between `ColorPalette.*` and `ColorPalette.Light.*`. Lesson renderers use the palette directly (always dark-theme vector colors).

Key color assignments:
- **VectorA** (blue `#50A0FF`), **VectorB** (green `#64DC64`), **VectorResult** (red `#FF6464`)
- **BasisI** (red), **BasisJ** (green) -- for transformed basis vectors
- **VectorEigen** (gold `#FFC832`) -- eigenvector highlighting

## Coordinate System

The application uses a standard mathematical coordinate system:

- **World space**: Y-up. Positive X is right, positive Y is up. Origin at grid center.
- **Screen space**: Y-down (standard WPF/Skia). Origin at top-left.
- **Conversion**: `GridRenderer` handles all mapping. The Y-flip happens in `WorldToScreenY` (subtracts instead of adds).
- **Snapping**: Mouse-dragged vectors snap to 0.5-unit increments via `Math.Round(worldCoord * 2) / 2`.
- **DPI**: `SkiaCanvasControl` multiplies WPF logical pixels by the DPI scale factor before passing to `GridRenderer`.

## Animation System

Matrix-based lessons (Matrix Transformations, Determinants, Eigenvalues) animate between matrix states using `TransformAnimator`:

1. `AnimateTo(target, durationMs)` stores the current matrix as `_from`, the target as `_to`, and records `DateTime.UtcNow`.
2. On each frame, `MainWindow` fires `CompositionTarget.Rendering`, which calls `LessonViewModelBase.OnTick()`.
3. The lesson's `OnTick` calls `_animator.Tick()`, which computes elapsed time, applies `MathHelpers.EaseInOutCubic(t)`, and lerps each matrix component: `Mat2.Lerp(_from, _to, eased)`.
4. The `Updated` event notifies the ViewModel, which triggers `OnPropertyChanged`.

Default animation duration is 500ms. The easing function is:

```csharp
// MathHelpers.cs
public static double EaseInOutCubic(double t)
{
    if (t < 0.5) return 4 * t * t * t;
    var p = 2 * t - 2;
    return 0.5 * p * p * p + 1;
}
```

## Sound System

`SoundService` wraps NAudio for fire-and-forget WAV playback:

- 6 sound effects: `click.wav`, `snap.wav`, `whoosh.wav`, `correct.wav`, `wrong.wav`, `navigate.wav`
- Loaded from WPF pack URIs (`pack://application:,,,/Sounds/...`)
- Each play creates a `WaveOutEvent` + `WaveChannel32` pipeline, disposed on `PlaybackStopped`
- Global mute (`IsMuted`) and volume (0.0--1.0, default 0.5) controls
- All playback failures are silently caught -- sound is non-critical

Sound triggers:
| Sound | When |
|-------|------|
| `click` | Mouse down on a draggable vector |
| `snap` | Mouse up after dragging |
| `whoosh` | Matrix entry changed (triggers animation) |
| `correct` | Quiz answer correct |
| `wrong` | Quiz answer incorrect |
| `navigate` | Lesson switched via sidebar |

## Navigation and Lesson Switching

```
User clicks lesson in NavigationPanel
  -> NavigationViewModel.SelectLesson(lesson)
    -> NavigationService.NavigateTo(lesson.Id)
      -> fires LessonChanged event
        -> MainViewModel.OnLessonChanged(lessonId)
          -> CreateLessonViewModel(lessonId)  // switch expression
            -> sets ActiveLesson property
              -> MainWindow.OnActiveLessonChanged()
                -> Canvas.SetRenderer(activeLesson as ISceneRenderer)
              -> ContentControl re-resolves DataTemplate (right panel)
              -> ExplanationPanel re-binds DataContext (bottom bar)
```

Lessons are cataloged in `LessonService` as a static list of `Lesson` records, each with an `Id`, `Title`, `Description`, and `LessonCategory` (Vectors, Matrices, Advanced). The sidebar groups them by category.

### Layout

The `MainWindow` is a 2-column, 2-row grid:

| | Column 0 (200px) | Column 1 (*) |
|---|---|---|
| **Row 0 (*)** | NavigationPanel (row-span 2) | SkiaCanvasControl + lesson control panel (ContentControl) |
| **Row 1 (120px)** | (spanned) | ExplanationPanel (explanation text + quiz UI) |

Default window size: 1280x800, minimum 900x600.

## Related

- [Lessons Guide](LESSONS.md)
- [Development Guide](DEVELOPMENT.md)
