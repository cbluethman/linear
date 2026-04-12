using LinearAlgebra.App.Models;

namespace LinearAlgebra.App.Services;

public record QuizQuestion(
    string Prompt,
    QuizQuestionType Type,
    object? CorrectAnswer,
    string[]? MultipleChoiceOptions = null);

public enum QuizQuestionType
{
    DragVector,
    NumericInput,
    MultipleChoice,
    SliderInput,
    ClickVector
}

public record QuizResult(int Correct, int Total)
{
    public double Percentage => Total > 0 ? (double)Correct / Total * 100 : 0;
}

public class QuizService
{
    private readonly Random _random = new();

    public QuizQuestion GenerateVectorBasicsQuestion()
    {
        var x = _random.Next(-5, 6);
        var y = _random.Next(-5, 6);
        while (x == 0 && y == 0) { x = _random.Next(-5, 6); y = _random.Next(-5, 6); }

        return new QuizQuestion(
            $"Place the vector at ({x}, {y})",
            QuizQuestionType.DragVector,
            new Vec2(x, y));
    }

    public QuizQuestion GenerateVectorOperationsQuestion()
    {
        var ax = _random.Next(-4, 5);
        var ay = _random.Next(-4, 5);
        var bx = _random.Next(-4, 5);
        var by = _random.Next(-4, 5);
        var a = new Vec2(ax, ay);
        var b = new Vec2(bx, by);
        var result = a + b;

        return new QuizQuestion(
            $"What is ({ax}, {ay}) + ({bx}, {by})? Drag the result vector.",
            QuizQuestionType.DragVector,
            result);
    }

    public QuizQuestion GenerateMatrixTransformQuestion()
    {
        var presets = new (string Name, Mat2 Matrix)[]
        {
            ("90-degree rotation", Mat2.Rotation(Math.PI / 2)),
            ("Reflection over X-axis", Mat2.ReflectionX),
            ("Reflection over Y-axis", Mat2.ReflectionY),
            ("Scale by 2", Mat2.Scale(2)),
            ("Horizontal shear", Mat2.ShearX(1)),
        };

        var correct = presets[_random.Next(presets.Length)];
        var options = presets.Select(p => p.Name).Distinct().ToArray();

        return new QuizQuestion(
            "Which transformation does this matrix represent?",
            QuizQuestionType.MultipleChoice,
            correct.Name,
            options);
    }

    public QuizQuestion GenerateLinearCombinationQuestion()
    {
        var s1 = _random.Next(-3, 4);
        var s2 = _random.Next(-3, 4);
        return new QuizQuestion(
            $"Express the target vector using scalars. Find s1 and s2.",
            QuizQuestionType.SliderInput,
            new Vec2(s1, s2));
    }

    public QuizQuestion GenerateDeterminantQuestion()
    {
        var a = _random.Next(-3, 4);
        var b = _random.Next(-3, 4);
        var c = _random.Next(-3, 4);
        var d = _random.Next(-3, 4);
        var mat = new Mat2(a, b, c, d);

        return new QuizQuestion(
            $"What is the determinant of [{a} {b}; {c} {d}]?",
            QuizQuestionType.NumericInput,
            mat.Determinant);
    }

    public QuizQuestion GenerateEigenQuestion()
    {
        return new QuizQuestion(
            "Click on the vectors that only scale (don't change direction) under this transformation.",
            QuizQuestionType.ClickVector,
            null); // Validated against computed eigenvectors
    }

    public QuizQuestion GenerateSystemsQuestion()
    {
        var options = new[] { "One solution", "No solution", "Infinitely many solutions" };
        var choice = _random.Next(3);

        return new QuizQuestion(
            "How many solutions does this system have?",
            QuizQuestionType.MultipleChoice,
            options[choice],
            options);
    }

    public bool ValidateDragAnswer(Vec2 answer, Vec2 correct, double tolerance = 0.5)
        => answer.DistanceTo(correct) <= tolerance;

    public bool ValidateNumericAnswer(double answer, double correct, double tolerance = 0.01)
        => Math.Abs(answer - correct) <= tolerance;
}
