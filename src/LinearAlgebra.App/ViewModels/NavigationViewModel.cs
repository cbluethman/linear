using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LinearAlgebra.App.Models;
using LinearAlgebra.App.Services;

namespace LinearAlgebra.App.ViewModels;

public partial class NavigationViewModel : ObservableObject
{
    private readonly ILessonService _lessonService;
    private readonly INavigationService _navigationService;
    private readonly SoundService _sound;

    [ObservableProperty] private Lesson? _selectedLesson;

    public IReadOnlyList<Lesson> VectorLessons { get; }
    public IReadOnlyList<Lesson> MatrixLessons { get; }
    public IReadOnlyList<Lesson> AdvancedLessons { get; }

    public NavigationViewModel(ILessonService lessonService, INavigationService navigationService, SoundService sound)
    {
        _lessonService = lessonService;
        _navigationService = navigationService;
        _sound = sound;

        VectorLessons = lessonService.GetLessonsByCategory(LessonCategory.Vectors);
        MatrixLessons = lessonService.GetLessonsByCategory(LessonCategory.Matrices);
        AdvancedLessons = lessonService.GetLessonsByCategory(LessonCategory.Advanced);
    }

    partial void OnSelectedLessonChanged(Lesson? value)
    {
        if (value != null)
        {
            _navigationService.NavigateTo(value.Id);
            _sound.PlayNavigate();
        }
    }

    [RelayCommand]
    private void SelectLesson(Lesson lesson)
    {
        SelectedLesson = lesson;
    }
}
