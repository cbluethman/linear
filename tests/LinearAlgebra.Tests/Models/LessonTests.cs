using FluentAssertions;
using LinearAlgebra.App.Models;

namespace LinearAlgebra.Tests.Models;

public class LessonTests
{
    [Fact]
    public void Lesson_RecordProperties_AreCorrect()
    {
        var lesson = new Lesson("vec-basics", "Vector Basics", "Learn about vectors", LessonCategory.Vectors);
        lesson.Id.Should().Be("vec-basics");
        lesson.Title.Should().Be("Vector Basics");
        lesson.Description.Should().Be("Learn about vectors");
        lesson.Category.Should().Be(LessonCategory.Vectors);
    }

    [Fact]
    public void Lesson_EqualRecords_AreEqual()
    {
        var a = new Lesson("id", "Title", "Desc", LessonCategory.Matrices);
        var b = new Lesson("id", "Title", "Desc", LessonCategory.Matrices);
        a.Should().Be(b);
    }

    [Fact]
    public void Lesson_DifferentIds_AreNotEqual()
    {
        var a = new Lesson("id1", "Title", "Desc", LessonCategory.Matrices);
        var b = new Lesson("id2", "Title", "Desc", LessonCategory.Matrices);
        a.Should().NotBe(b);
    }

    [Fact]
    public void LessonCategory_HasExpectedValues()
    {
        Enum.GetValues<LessonCategory>().Should().HaveCount(3);
        Enum.IsDefined(LessonCategory.Vectors).Should().BeTrue();
        Enum.IsDefined(LessonCategory.Matrices).Should().BeTrue();
        Enum.IsDefined(LessonCategory.Advanced).Should().BeTrue();
    }
}
