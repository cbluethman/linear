namespace LinearAlgebra.App.Services;

public class NavigationService : INavigationService
{
    public event Action<string>? LessonChanged;

    public void NavigateTo(string lessonId)
    {
        LessonChanged?.Invoke(lessonId);
    }
}
