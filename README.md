# Linear Algebra -- Visual Learning

A Windows desktop application that interactively teaches linear algebra through real-time visual demonstrations. Built with WPF and .NET 8, it renders 2D vector and matrix concepts on a SkiaSharp canvas and lets you drag vectors, tweak matrix entries, and test yourself with built-in quizzes.

<!-- TODO: Add screenshots -->

## Features

- **7 interactive lessons** covering vectors, matrices, and advanced topics (eigenvalues, systems of equations)
- **Real-time rendering** with SkiaSharp -- draggable vectors, animated matrix transformations, and a zoomable/pannable coordinate grid
- **Quiz mode** in every lesson -- drag-to-answer, numeric input, and multiple-choice questions with score tracking
- **Dark and light themes** with a single toggle
- **Sound effects** via NAudio (click, snap, whoosh, correct/wrong) with mute support
- **Smooth animations** using cubic ease-in-out interpolation on `CompositionTarget.Rendering`

## Lessons

| # | Lesson | Category | What you learn |
|---|--------|----------|----------------|
| 1 | Vector Basics | Vectors | Components, magnitude, direction, angle arc |
| 2 | Vector Operations | Vectors | Addition (parallelogram rule), subtraction, scalar multiplication |
| 3 | Linear Combinations | Vectors | Scalar-weighted sums, span, collinearity detection |
| 4 | Matrix Transformations | Matrices | Rotation, scaling, shearing, reflection via 2x2 matrices |
| 5 | Determinants | Matrices | Area scaling factor, orientation (sign), animated parallelogram |
| 6 | Eigenvalues & Eigenvectors | Advanced | Characteristic polynomial, eigenvector highlighting, sample-vector fan |
| 7 | Systems of Equations | Advanced | Intersecting/parallel/coincident lines, Cramer's rule solution |

## Getting Started

### Prerequisites

- Windows 10 or later
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (build) or .NET 8 Desktop Runtime (run only)

### Build and Run

```bash
# Clone the repository
git clone https://github.com/cbluethman/linear.git
cd linear

# Restore, build, run
dotnet build
dotnet run --project src/LinearAlgebra.App
```

### Run Tests

```bash
dotnet test
```

Tests use xUnit and FluentAssertions. See `tests/LinearAlgebra.Tests/` for coverage of `Vec2`, `Mat2`, `MathHelpers`, `GridRenderer`, `TransformAnimator`, `QuizService`, and the `Lesson` model.

## Project Structure

```
LinearAlgebra.sln
src/
  LinearAlgebra.App/
    App.xaml(.cs)                  # Startup, manual DI wiring
    MainWindow.xaml(.cs)           # Shell layout, canvas + event wiring
    Converters/                    # WPF IValueConverters
    Helpers/MathHelpers.cs         # Easing, snapping, unit conversions
    Models/                        # Vec2, Mat2, Lesson, LessonCategory
    Rendering/                     # SkiaSharp rendering pipeline
      SkiaCanvasControl.cs         # SKElement subclass, render loop, input
      GridRenderer.cs              # Coordinate grid, world<->screen mapping
      VectorRenderer.cs            # Arrow drawing, labels, hit testing
      ShapeRenderer.cs             # Polygons, unit square, lines
      TransformAnimator.cs         # Mat2 lerp with easing
      ColorPalette.cs              # Dark + light theme color constants
    Resources/                     # XAML resource dictionaries
    Services/                      # Navigation, lessons catalog, quiz, sound
    Sounds/                        # WAV files (click, snap, whoosh, etc.)
    ViewModels/
      MainViewModel.cs             # Root VM, theme/mute, lesson switching
      NavigationViewModel.cs       # Sidebar categories and selection
      Lessons/                     # One VM per lesson + shared base class
    Views/
      NavigationPanel.xaml          # Sidebar
      ExplanationPanel.xaml         # Bottom bar (explanation + quiz)
      Lessons/                      # One XAML control panel per lesson
tests/
  LinearAlgebra.Tests/             # xUnit + FluentAssertions
.github/workflows/
  build.yml                        # CI: build + test on push/PR
  release.yml                      # CD: tag-triggered portable exe + MSIX
```

## Documentation

- [Architecture](docs/ARCHITECTURE.md) -- tech stack, MVVM pattern, rendering pipeline, coordinate system, animation, sound, navigation
- [Lessons Guide](docs/LESSONS.md) -- what each lesson teaches, controls, quiz modes, how to add a new lesson
- [Development Guide](docs/DEVELOPMENT.md) -- prerequisites, building, testing, CI/CD, MSIX packaging, creating a release

## Contributing

1. Fork the repository and create a feature branch.
2. Follow existing code conventions: file-scoped namespaces, `CommunityToolkit.Mvvm` source generators, SkiaSharp for all canvas rendering.
3. Add or update unit tests for any new models, helpers, or services.
4. Open a pull request against `main`. CI must pass (build + test).

## License

<!-- TODO: Add license -->
