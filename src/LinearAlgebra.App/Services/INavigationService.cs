using LinearAlgebra.App.Models;

namespace LinearAlgebra.App.Services;

public interface INavigationService
{
    event Action<string>? LessonChanged;
    void NavigateTo(string lessonId);
}
