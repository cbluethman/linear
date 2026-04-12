using LinearAlgebra.App.Models;

namespace LinearAlgebra.App.Services;

public class LessonService : ILessonService
{
    private static readonly List<Lesson> Lessons = new()
    {
        new("vector-basics", "Vector Basics", "Learn about 2D vectors, their components, magnitude, and direction.", LessonCategory.Vectors),
        new("vector-operations", "Vector Operations", "Explore vector addition, subtraction, and scalar multiplication.", LessonCategory.Vectors),
        new("linear-combinations", "Linear Combinations", "Understand linear combinations and the span of vectors.", LessonCategory.Vectors),
        new("matrix-transforms", "Matrix Transformations", "See how 2x2 matrices transform space — rotation, scaling, shearing, and reflection.", LessonCategory.Matrices),
        new("determinants", "Determinants", "Visualize determinants as area scaling factors with orientation.", LessonCategory.Matrices),
        new("eigenvalues", "Eigenvalues & Eigenvectors", "Find the vectors that only scale under a transformation.", LessonCategory.Advanced),
        new("systems", "Systems of Equations", "See systems of linear equations as intersecting lines.", LessonCategory.Advanced),
    };

    public IReadOnlyList<Lesson> GetAllLessons() => Lessons;

    public IReadOnlyList<Lesson> GetLessonsByCategory(LessonCategory category)
        => Lessons.Where(l => l.Category == category).ToList();
}
