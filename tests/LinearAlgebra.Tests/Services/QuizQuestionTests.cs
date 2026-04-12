using FluentAssertions;
using LinearAlgebra.App.Services;

namespace LinearAlgebra.Tests.Services;

public class QuizQuestionTests
{
    [Fact]
    public void QuizQuestion_DefaultMultipleChoiceOptions_IsNull()
    {
        var q = new QuizQuestion("prompt", QuizQuestionType.NumericInput, 42.0);
        q.MultipleChoiceOptions.Should().BeNull();
    }

    [Fact]
    public void QuizQuestion_WithOptions_StoresOptions()
    {
        var options = new[] { "A", "B", "C" };
        var q = new QuizQuestion("prompt", QuizQuestionType.MultipleChoice, "A", options);
        q.MultipleChoiceOptions.Should().BeEquivalentTo(options);
    }

    [Fact]
    public void QuizQuestionType_HasAllExpectedValues()
    {
        Enum.GetValues<QuizQuestionType>().Should().HaveCount(5);
    }
}
