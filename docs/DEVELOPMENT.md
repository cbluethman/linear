# Development Guide

How to build, test, and contribute to the Linear Algebra Visual Learning application.

## Prerequisites

- **Windows 10 or later** (WPF is Windows-only)
- **.NET 8 SDK** -- [download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio 2022** (recommended) or any editor with C# support (VS Code + C# Dev Kit, Rider)

## Building and Running

### Command Line

```bash
# From the repository root
dotnet restore
dotnet build
dotnet run --project src/LinearAlgebra.App
```

### Visual Studio

Open `LinearAlgebra.sln`. Set `LinearAlgebra.App` as the startup project. Press F5.

### Release Build

```bash
dotnet build --configuration Release
dotnet run --project src/LinearAlgebra.App --configuration Release
```

## Running Tests

```bash
dotnet test
```

Or with verbose output:

```bash
dotnet test --verbosity normal
```

### Test Project

Tests live in `tests/LinearAlgebra.Tests/` and use:

| Package | Version | Purpose |
|---------|---------|---------|
| xUnit | 2.6.6 | Test framework |
| FluentAssertions | 6.12.0 | Assertion library |
| coverlet.collector | 6.0.0 | Code coverage |

### Test Coverage

| Test File | What it covers |
|-----------|---------------|
| `Models/Vec2Tests.cs` | Vec2 arithmetic, magnitude, angle, dot/cross product, normalization |
| `Models/Mat2Tests.cs` | Mat2 multiplication, determinant, eigenvalues, inverse, rotation, scale, shear |
| `Models/LessonTests.cs` | Lesson record construction and LessonCategory enum |
| `Helpers/MathHelpersTests.cs` | Degree/radian conversion, lerp, easing, grid snapping |
| `Rendering/GridRendererTests.cs` | World-to-screen and screen-to-world coordinate conversion |
| `Rendering/TransformAnimatorTests.cs` | Animation state transitions, SetImmediate, Tick progression |
| `Services/QuizServiceTests.cs` | Question generation, answer validation |
| `Services/QuizQuestionTests.cs` | QuizQuestion construction and QuizQuestionType enum |

## Project Structure

```
LinearAlgebra.sln                          # Solution file (2 projects)
src/LinearAlgebra.App/
  LinearAlgebra.App.csproj                 # WinExe, net8.0-windows, nullable enabled
  App.xaml / App.xaml.cs                   # Application entry, manual DI wiring
  MainWindow.xaml / MainWindow.xaml.cs     # Shell: nav + canvas + explanation bar

  Converters/
    BoolToVisibilityConverter.cs           # bool -> Visible/Collapsed (+ inverse)
    IndexToBoolConverter.cs                # int index comparison for RadioButtons
    InverseBoolConverter.cs                # bool negation
    IsNegativeConverter.cs                 # double < 0 check
    MuteButtonConverter.cs                 # bool -> "Mute"/"Unmute" (singleton)
    QuizButtonTextConverter.cs             # bool -> "Test Yourself"/"Exit Quiz" (singleton)
    StringMatchConverter.cs                # string equality for quiz answer highlighting

  Helpers/
    MathHelpers.cs                         # DegreesToRadians, RadiansToDegrees, Lerp,
                                           # EaseInOutCubic, SnapToGrid

  Models/
    Vec2.cs                                # readonly record struct, operators, Dot, Cross,
                                           # Magnitude, Angle, Normalized, FromAngle
    Mat2.cs                                # readonly record struct, Transform, Determinant,
                                           # Eigenvalues, Eigenvectors, Rotation, Scale,
                                           # Shear, Reflection, Inverse, Transpose, Lerp
    Lesson.cs                              # record(Id, Title, Description, Category)
                                           # + LessonCategory enum

  Rendering/
    ISceneRenderer.cs                      # interface: Render(SKCanvas, SKSize)
    SkiaCanvasControl.cs                   # SKElement subclass, render loop,
                                           # DPI-aware mouse input, pan/zoom
    GridRenderer.cs                        # Coordinate grid, world<->screen conversion,
                                           # pan/zoom state, theme-aware
    VectorRenderer.cs                      # Arrow drawing, labels, hit testing
    ShapeRenderer.cs                       # Polygons, unit square, parallelogram, lines
    TransformAnimator.cs                   # Mat2 lerp with cubic easing, tick-driven
    ColorPalette.cs                        # Static SKColor constants, dark + light themes

  Resources/
    Colors.xaml                            # WPF color definitions and SolidColorBrushes
    Styles.xaml                            # Button, slider, text box styles
    DataTemplates.xaml                     # ViewModel -> View DataTemplate mappings

  Services/
    ILessonService.cs                      # Interface: GetAllLessons, GetLessonsByCategory
    LessonService.cs                       # Static lesson catalog (7 lessons)
    INavigationService.cs                  # Interface: NavigateTo, LessonChanged event
    NavigationService.cs                   # Event-based navigation
    QuizService.cs                         # Question generation + answer validation
                                           # + QuizQuestion/QuizResult records
    SoundService.cs                        # NAudio WAV playback, mute/volume controls

  Sounds/
    click.wav, snap.wav, whoosh.wav,       # UI interaction sounds
    correct.wav, wrong.wav, navigate.wav   # Quiz + navigation sounds

  ViewModels/
    MainViewModel.cs                       # Root VM: ActiveLesson, theme, mute,
                                           # lesson factory switch expression
    NavigationViewModel.cs                 # Sidebar: lesson lists by category,
                                           # SelectLessonCommand
    Lessons/
      LessonViewModelBase.cs              # Abstract base: Render, mouse handlers,
                                           # quiz state, explanation text
      VectorBasicsViewModel.cs             # Draggable vector, components, angle arc
      VectorOperationsViewModel.cs         # Add/sub/scale with parallelogram rule
      LinearCombinationViewModel.cs        # Scalar weights, span visualization
      MatrixTransformViewModel.cs          # Animated basis vectors + unit square
      DeterminantViewModel.cs              # Area parallelogram, orientation color
      EigenViewModel.cs                    # Sample vector fan, eigenvector lines
      SystemsOfEquationsViewModel.cs       # Line intersection, three cases

  Views/
    NavigationPanel.xaml(.cs)              # Sidebar with categorized lesson buttons
    ExplanationPanel.xaml(.cs)             # Bottom bar: explanation + quiz controls
    Lessons/
      VectorBasicsView.xaml(.cs)           # Right panel for each lesson
      VectorOperationsView.xaml(.cs)
      LinearCombinationView.xaml(.cs)
      MatrixTransformView.xaml(.cs)
      DeterminantView.xaml(.cs)
      EigenView.xaml(.cs)
      SystemsOfEquationsView.xaml(.cs)

tests/LinearAlgebra.Tests/
  LinearAlgebra.Tests.csproj               # xUnit + FluentAssertions + coverlet
  Models/Vec2Tests.cs
  Models/Mat2Tests.cs
  Models/LessonTests.cs
  Helpers/MathHelpersTests.cs
  Rendering/GridRendererTests.cs
  Rendering/TransformAnimatorTests.cs
  Services/QuizServiceTests.cs
  Services/QuizQuestionTests.cs
```

## CI/CD

### Build Workflow (`.github/workflows/build.yml`)

Runs on every push and pull request to `main`/`master`:

1. Checkout
2. Setup .NET 8 SDK
3. `dotnet restore`
4. `dotnet build --configuration Release --no-restore`
5. `dotnet test --configuration Release --no-build --verbosity normal` (continue-on-error)

Runner: `windows-latest`

### Release Workflow (`.github/workflows/release.yml`)

Triggered by pushing a tag matching `v*` (e.g., `v1.0.0`):

1. Checkout + Setup .NET 8
2. Extract version from tag (`v1.2.3` -> `1.2.3`)
3. `dotnet build --configuration Release`
4. **Portable publish**: self-contained, single-file exe for `win-x64`
5. **MSIX publish**: self-contained with `GenerateAppxPackageOnBuild=true` (unsigned, continue-on-error)
6. Zip the portable output
7. Upload artifacts
8. Create GitHub Release with auto-generated release notes, attaching the zip and any MSIX packages

Required permission: `contents: write` (to create releases).

## Creating a Release

```bash
# Tag the commit
git tag v1.0.0
git push origin v1.0.0
```

This triggers the release workflow, which builds and publishes:

- `LinearAlgebra-1.0.0-win-x64.zip` -- portable self-contained executable
- `*.msix` -- MSIX installer package (unsigned, may not be present if MSIX packaging fails)

The GitHub Release page will appear at `https://github.com/cbluethman/linear/releases/tag/v1.0.0` with auto-generated release notes.

## MSIX Packaging Notes

The release workflow attempts MSIX packaging with `continue-on-error: true` because:

- MSIX generation requires a valid app manifest and may fail without a signing certificate
- `AppxPackageSigningEnabled=false` produces an unsigned package suitable for sideloading
- For production distribution, you would need a code-signing certificate and to configure `AppxPackageSigningEnabled=true` with the certificate thumbprint

The portable single-file exe is the primary distribution artifact.

## Related

- [Architecture](ARCHITECTURE.md) -- technical deep-dive into MVVM, rendering, and animation
- [Lessons Guide](LESSONS.md) -- lesson descriptions and how to add new ones
