using LinearAlgebra.App.Models;

namespace LinearAlgebra.App.Services;

public interface ILessonService
{
    IReadOnlyList<Lesson> GetAllLessons();
    IReadOnlyList<Lesson> GetLessonsByCategory(LessonCategory category);
}
