namespace LinearAlgebra.App.Models;

public record Lesson(string Id, string Title, string Description, LessonCategory Category);

public enum LessonCategory
{
    Vectors,
    Matrices,
    Advanced
}
