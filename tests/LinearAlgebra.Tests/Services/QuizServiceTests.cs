using FluentAssertions;
using LinearAlgebra.App.Models;
using LinearAlgebra.App.Services;

namespace LinearAlgebra.Tests.Services;

public class QuizServiceTests
{
    private readonly QuizService _sut = new();

    // --- GenerateVectorBasicsQuestion ---

    [Fact]
    public void GenerateVectorBasicsQuestion_ReturnsNonNullQuestion()
    {
        var q = _sut.GenerateVectorBasicsQuestion();
        q.Should().NotBeNull();
        q.Prompt.Should().NotBeNullOrWhiteSpace();
        q.Type.Should().Be(QuizQuestionType.DragVector);
        q.CorrectAnswer.Should().BeOfType<Vec2>();
    }

    [Fact]
    public void GenerateVectorBasicsQuestion_AnswerIsNeverZeroVector()
    {
        // Run many times to check the zero-vector exclusion
        for (int i = 0; i < 100; i++)
        {
            var q = _sut.GenerateVectorBasicsQuestion();
            var answer = (Vec2)q.CorrectAnswer!;
            (answer.X == 0 && answer.Y == 0).Should().BeFalse();
        }
    }

    [Fact]
    public void GenerateVectorBasicsQuestion_AnswerInExpectedRange()
    {
        for (int i = 0; i < 50; i++)
        {
            var answer = (Vec2)_sut.GenerateVectorBasicsQuestion().CorrectAnswer!;
            answer.X.Should().BeInRange(-5, 5);
            answer.Y.Should().BeInRange(-5, 5);
        }
    }

    // --- GenerateVectorOperationsQuestion ---

    [Fact]
    public void GenerateVectorOperationsQuestion_ReturnsAdditionResult()
    {
        var q = _sut.GenerateVectorOperationsQuestion();
        q.Type.Should().Be(QuizQuestionType.DragVector);
        q.CorrectAnswer.Should().BeOfType<Vec2>();
        q.Prompt.Should().Contain("+");
    }

    // --- GenerateMatrixTransformQuestion ---

    [Fact]
    public void GenerateMatrixTransformQuestion_ReturnsMultipleChoice()
    {
        var q = _sut.GenerateMatrixTransformQuestion();
        q.Type.Should().Be(QuizQuestionType.MultipleChoice);
        q.MultipleChoiceOptions.Should().NotBeNullOrEmpty();
        q.CorrectAnswer.Should().BeOfType<string>();
    }

    [Fact]
    public void GenerateMatrixTransformQuestion_CorrectAnswerIsInOptions()
    {
        var q = _sut.GenerateMatrixTransformQuestion();
        q.MultipleChoiceOptions.Should().Contain((string)q.CorrectAnswer!);
    }

    [Fact]
    public void GenerateMatrixTransformQuestion_HasKnownPresets()
    {
        var knownNames = new HashSet<string>
        {
            "90-degree rotation",
            "Reflection over X-axis",
            "Reflection over Y-axis",
            "Scale by 2",
            "Horizontal shear"
        };

        // Run many times to sample different questions
        for (int i = 0; i < 50; i++)
        {
            var q = _sut.GenerateMatrixTransformQuestion();
            knownNames.Should().Contain((string)q.CorrectAnswer!);
        }
    }

    // --- GenerateLinearCombinationQuestion ---

    [Fact]
    public void GenerateLinearCombinationQuestion_ReturnsSliderInput()
    {
        var q = _sut.GenerateLinearCombinationQuestion();
        q.Type.Should().Be(QuizQuestionType.SliderInput);
        q.CorrectAnswer.Should().BeOfType<Vec2>();
    }

    // --- GenerateDeterminantQuestion ---

    [Fact]
    public void GenerateDeterminantQuestion_ReturnsNumericInput()
    {
        var q = _sut.GenerateDeterminantQuestion();
        q.Type.Should().Be(QuizQuestionType.NumericInput);
        q.CorrectAnswer.Should().BeOfType<Mat2>();
    }

    [Fact]
    public void GenerateDeterminantQuestion_PromptContainsMatrixValues()
    {
        var q = _sut.GenerateDeterminantQuestion();
        q.Prompt.Should().Contain("determinant");
    }

    // --- GenerateEigenQuestion ---

    [Fact]
    public void GenerateEigenQuestion_ReturnsClickVector()
    {
        var q = _sut.GenerateEigenQuestion();
        q.Type.Should().Be(QuizQuestionType.ClickVector);
        q.CorrectAnswer.Should().BeNull();
    }

    // --- GenerateSystemsQuestion ---

    [Fact]
    public void GenerateSystemsQuestion_ReturnsMultipleChoice()
    {
        var q = _sut.GenerateSystemsQuestion();
        q.Type.Should().Be(QuizQuestionType.MultipleChoice);
        q.MultipleChoiceOptions.Should().HaveCount(3);
    }

    [Fact]
    public void GenerateSystemsQuestion_CorrectAnswerIsOneOfThreeOptions()
    {
        var validOptions = new[] { "One solution", "No solution", "Infinitely many solutions" };
        for (int i = 0; i < 30; i++)
        {
            var q = _sut.GenerateSystemsQuestion();
            validOptions.Should().Contain((string)q.CorrectAnswer!);
        }
    }

    // --- ValidateDragAnswer ---

    [Fact]
    public void ValidateDragAnswer_ExactMatch_ReturnsTrue()
    {
        _sut.ValidateDragAnswer(new Vec2(3, 4), new Vec2(3, 4)).Should().BeTrue();
    }

    [Fact]
    public void ValidateDragAnswer_WithinTolerance_ReturnsTrue()
    {
        _sut.ValidateDragAnswer(new Vec2(3.3, 4.3), new Vec2(3, 4)).Should().BeTrue();
    }

    [Fact]
    public void ValidateDragAnswer_ExactlyAtTolerance_ReturnsTrue()
    {
        _sut.ValidateDragAnswer(new Vec2(3.5, 4), new Vec2(3, 4), 0.5).Should().BeTrue();
    }

    [Fact]
    public void ValidateDragAnswer_BeyondTolerance_ReturnsFalse()
    {
        _sut.ValidateDragAnswer(new Vec2(5, 4), new Vec2(3, 4)).Should().BeFalse();
    }

    [Fact]
    public void ValidateDragAnswer_CustomTolerance()
    {
        _sut.ValidateDragAnswer(new Vec2(3, 4), new Vec2(3.01, 4.01), 0.02).Should().BeTrue();
        _sut.ValidateDragAnswer(new Vec2(3, 4), new Vec2(4, 5), 0.02).Should().BeFalse();
    }

    [Fact]
    public void ValidateDragAnswer_ZeroVector()
    {
        _sut.ValidateDragAnswer(Vec2.Zero, Vec2.Zero).Should().BeTrue();
    }

    // --- ValidateNumericAnswer ---

    [Fact]
    public void ValidateNumericAnswer_ExactMatch_ReturnsTrue()
    {
        _sut.ValidateNumericAnswer(5.0, 5.0).Should().BeTrue();
    }

    [Fact]
    public void ValidateNumericAnswer_WithinTolerance_ReturnsTrue()
    {
        _sut.ValidateNumericAnswer(5.005, 5.0).Should().BeTrue();
    }

    [Fact]
    public void ValidateNumericAnswer_BeyondTolerance_ReturnsFalse()
    {
        _sut.ValidateNumericAnswer(5.5, 5.0).Should().BeFalse();
    }

    [Fact]
    public void ValidateNumericAnswer_NegativeValues()
    {
        _sut.ValidateNumericAnswer(-3.0, -3.005).Should().BeTrue();
    }

    [Fact]
    public void ValidateNumericAnswer_CustomTolerance()
    {
        _sut.ValidateNumericAnswer(5.0, 5.05, 0.1).Should().BeTrue();
        _sut.ValidateNumericAnswer(5.0, 5.5, 0.1).Should().BeFalse();
    }

    [Fact]
    public void ValidateNumericAnswer_Zero()
    {
        _sut.ValidateNumericAnswer(0.0, 0.0).Should().BeTrue();
    }

    // --- QuizResult ---

    [Fact]
    public void QuizResult_Percentage_CalculatesCorrectly()
    {
        new QuizResult(7, 10).Percentage.Should().BeApproximately(70, 1e-10);
    }

    [Fact]
    public void QuizResult_Percentage_AllCorrect_Is100()
    {
        new QuizResult(5, 5).Percentage.Should().BeApproximately(100, 1e-10);
    }

    [Fact]
    public void QuizResult_Percentage_NoneCorrect_IsZero()
    {
        new QuizResult(0, 5).Percentage.Should().BeApproximately(0, 1e-10);
    }

    [Fact]
    public void QuizResult_Percentage_ZeroTotal_IsZero()
    {
        new QuizResult(0, 0).Percentage.Should().BeApproximately(0, 1e-10);
    }
}
