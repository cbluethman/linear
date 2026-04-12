# Lessons Guide

The application contains 7 interactive lessons organized into three categories. Each lesson combines a real-time SkiaSharp canvas visualization with a XAML control panel and an explanation bar at the bottom.

## Lesson Overview

### Category: Vectors

#### 1. Vector Basics

**What it teaches:** 2D vector fundamentals -- components (x, y), magnitude, direction angle.

**Canvas:** A single draggable vector drawn from the origin. Dashed component lines show the x and y projections. An arc at the origin indicates the angle from the positive x-axis.

**Controls:**
- Drag the vector arrowhead to reposition (snaps to 0.5-unit grid)
- Toggle "Show Components" to display/hide dashed projection lines
- Sliders or direct input for X and Y (bound to `VectorX`, `VectorY`)

**Explanation bar:** Shows `v = (x, y)`, magnitude `|v|`, and angle in degrees, updated in real time.

**Quiz mode:** A target vector appears as a dashed red arrow. Drag the blue vector to match the target coordinates. Answers validated within 0.5-unit tolerance.

---

#### 2. Vector Operations

**What it teaches:** Vector addition (parallelogram rule), subtraction, and scalar multiplication.

**Canvas:** Two vectors A (blue) and B (green) drawn from the origin, plus the result R (red). In addition mode, dashed parallelogram-rule lines connect A to A+B and B to A+B.

**Controls:**
- Drag arrowheads of A or B
- Operation selector (three modes): Add, Subtract, Scale
- Scalar slider (visible in Scale mode, range -5 to 5)

**Explanation bar:** Shows the selected operation name, A and B values, and the computed result.

**Quiz mode:** Generates a random addition problem. A gold target vector appears; drag to match.

---

#### 3. Linear Combinations

**What it teaches:** Linear combinations (`s1 * v1 + s2 * v2`), the span of two vectors, and collinearity.

**Canvas:**
- Two basis vectors v1 (blue) and v2 (green), draggable
- Scaled versions shown as dashed arrows labeled `s1*v1` and `s2*v2`
- Result vector (red)
- If vectors are not collinear: a shaded parallelogram shows the span region
- If vectors are collinear: a highlighted line through the origin indicates the 1D span, plus a warning in the explanation text

**Controls:**
- Drag v1 and v2 arrowheads
- Sliders for Scalar1 and Scalar2 (range -5 to 5, step 0.5)

**Explanation bar:** Shows the linear combination formula, vector values, result, and a collinearity warning when `|cross(v1, v2)| < 0.1`.

**Quiz mode:** Generates a question asking the user to find scalar values.

---

### Category: Matrices

#### 4. Matrix Transformations

**What it teaches:** How 2x2 matrices transform space. Covers rotation, scaling, shearing, and reflection by showing basis vectors and the unit square before and after transformation.

**Canvas:**
- Original unit square [0,1]x[0,1] (faded blue outline)
- Transformed unit square (red, filled)
- Original basis vectors i and j (faded, dashed)
- Transformed basis vectors i' and j' (solid, colored)
- Smooth animated transition when matrix changes (500ms cubic ease)

**Controls:**
- 2x2 matrix entry text boxes (M11, M12, M21, M22)
- Rotation angle slider + "Apply Rotation" button
- Preset buttons: Scale, Shear X, Shear Y, Reflect X, Reflect Y, Identity

**Explanation bar:** Shows matrix entries, determinant, and where i-hat and j-hat map to.

**Quiz mode:** Multiple-choice -- identifies which named transformation the displayed matrix represents (rotation, reflection, scale, shear).

---

#### 5. Determinants

**What it teaches:** The determinant as an area scaling factor with orientation. Positive determinant preserves orientation; negative determinant reverses it.

**Canvas:**
- Original unit square (faded)
- Transformed parallelogram, colored blue (positive det) or red (negative det)
- Transformed basis vectors i' and j'
- Area label at the parallelogram center, plus "(flipped)" when determinant is negative
- Animated transitions between matrix states

**Controls:**
- 2x2 matrix entry text boxes
- Determinant value displayed as a computed property

**Explanation bar:** Shows the determinant formula `M11*M22 - M12*M21`, the absolute area scale factor, and orientation status.

**Quiz mode:** Numeric input -- type the determinant value for a randomly generated matrix. Validated within 0.1 tolerance.

---

### Category: Advanced

#### 6. Eigenvalues & Eigenvectors

**What it teaches:** Eigenvectors are vectors that only scale (do not change direction) under a linear transformation. Eigenvalues are the corresponding scale factors.

**Canvas:**
- 24 sample vectors arranged in a circle (radius 2), shown before (faded) and after transformation
- Vectors aligned with eigenvectors are highlighted in gold
- Dashed lines extending through the origin along each eigenvector direction
- Labeled eigenvector arrows with their eigenvalue (`lambda=...`)
- Complex eigenvalues (rotation-like): no eigenvector lines, explanation notes "no real eigenvectors"

**Controls:**
- 2x2 matrix entry text boxes
- "Show Eigenvectors" toggle
- "Find Eigenvectors" button (re-enables display and plays sound)

**Explanation bar:** Lists eigenvalues (lambda1, lambda2) and eigenvector components, or states "Complex eigenvalues" when the discriminant is negative.

**Quiz mode:** Visual -- click on vectors that only scale under the transformation. Eigenvectors are hidden during quiz.

---

#### 7. Systems of Equations

**What it teaches:** Systems of two linear equations as intersecting lines. Covers the three cases: one solution (intersecting), no solution (parallel), infinitely many solutions (coincident).

**Canvas:**
- Two lines L1 (blue) and L2 (green) drawn across the visible area
- Intersection point shown as a red dot with coordinates label
- "Parallel - No Solution" or "Coincident - Infinite Solutions" text overlay for degenerate cases

**Controls:**
- Coefficient inputs for both equations: `a1*x + b1*y = c1` and `a2*x + b2*y = c2`
- Preset buttons: Intersecting, Parallel, Coincident

**Explanation bar:** Shows both equations and the solution (computed via Cramer's rule) or the solution type.

**Quiz mode:** Multiple-choice -- "How many solutions does this system have?" with options: One solution, No solution, Infinitely many solutions. The system is configured to match the correct answer.

---

## Quiz System

Every lesson has a "Test Yourself" button in the `ExplanationPanel`. Clicking it enters quiz mode (`IsQuizMode = true`), which:

1. Resets the score counter (`QuizCorrect` / `QuizTotal`)
2. Calls the lesson's `StartQuiz()` to generate a question via `QuizService`
3. Displays the prompt in the explanation bar
4. Accepts answers through the lesson's input mechanism (drag, numeric input, multiple choice)
5. Calls `RecordAnswer(correct)` which increments counters, shows "Correct!"/"Try again!", and plays a sound
6. On correct answer, automatically advances to the next question

Question types are defined in `QuizQuestionType`: `DragVector`, `NumericInput`, `MultipleChoice`, `SliderInput`, `ClickVector`.

## How to Add a New Lesson

Follow these steps to add a lesson (e.g., "Dot Product"):

### Step 1: Define the lesson in the catalog

In `Services/LessonService.cs`, add an entry to the `Lessons` list:

```csharp
new("dot-product", "Dot Product", "Visualize the dot product geometrically.", LessonCategory.Vectors),
```

### Step 2: Create the ViewModel

Create `ViewModels/Lessons/DotProductViewModel.cs`:

```csharp
public partial class DotProductViewModel : LessonViewModelBase
{
    public override string Title => "Dot Product";

    public DotProductViewModel(GridRenderer grid, SoundService sound) : base(grid, sound)
    {
        // Initialize state
    }

    public override void Render(SKCanvas canvas, SKSize size)
    {
        // Draw using Vectors, Shapes, and Grid
    }

    // Override OnMouseDown/Move/Up for interactivity
    // Override StartQuiz/EndQuiz for quiz mode
}
```

Your ViewModel inherits `ISceneRenderer` through the base class, so its `Render` method is called every frame.

### Step 3: Create the View (control panel)

Create `Views/Lessons/DotProductView.xaml` -- this is the right-side panel with sliders, buttons, etc. Bind to your ViewModel's properties.

### Step 4: Register the DataTemplate

In `Resources/DataTemplates.xaml`, add:

```xml
<DataTemplate DataType="{x:Type lessons:DotProductViewModel}">
    <views:DotProductView />
</DataTemplate>
```

### Step 5: Register in MainViewModel

In `MainViewModel.CreateLessonViewModel`, add a case to the switch expression:

```csharp
"dot-product" => new DotProductViewModel(Grid, _sound),
```

### Step 6: Add quiz support (optional)

Add a question generator method to `QuizService` and call it from your ViewModel's `StartQuiz()` override.

### Step 7: Add tests

Add unit tests for any new model logic or quiz generation in `tests/LinearAlgebra.Tests/`.

## Related

- [Architecture](ARCHITECTURE.md) -- rendering pipeline, animation system, MVVM details
- [Development Guide](DEVELOPMENT.md) -- building, testing, CI/CD
