using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LinearAlgebra.App.Rendering;
using LinearAlgebra.App.Services;
using LinearAlgebra.App.ViewModels.Lessons;

namespace LinearAlgebra.App.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;
    private readonly SoundService _sound;

    [ObservableProperty] private LessonViewModelBase? _activeLesson;
    [ObservableProperty] private bool _isDarkTheme = true;
    [ObservableProperty] private bool _isMuted;

    public NavigationViewModel Navigation { get; }
    public GridRenderer Grid { get; } = new();

    public MainViewModel(INavigationService navigationService, ILessonService lessonService, SoundService sound)
    {
        _navigationService = navigationService;
        _sound = sound;

        Navigation = new NavigationViewModel(lessonService, navigationService, sound);

        _navigationService.LessonChanged += OnLessonChanged;

        // Start with first lesson
        navigationService.NavigateTo("vector-basics");
    }

    private void OnLessonChanged(string lessonId)
    {
        ActiveLesson = CreateLessonViewModel(lessonId);
    }

    private LessonViewModelBase CreateLessonViewModel(string lessonId)
    {
        return lessonId switch
        {
            "vector-basics" => new VectorBasicsViewModel(Grid, _sound),
            "vector-operations" => new VectorOperationsViewModel(Grid, _sound),
            "linear-combinations" => new LinearCombinationViewModel(Grid, _sound),
            "matrix-transforms" => new MatrixTransformViewModel(Grid, _sound),
            "determinants" => new DeterminantViewModel(Grid, _sound),
            "eigenvalues" => new EigenViewModel(Grid, _sound),
            "systems" => new SystemsOfEquationsViewModel(Grid, _sound),
            _ => new VectorBasicsViewModel(Grid, _sound)
        };
    }

    partial void OnIsDarkThemeChanged(bool value)
    {
        Grid.SetTheme(value);
    }

    partial void OnIsMutedChanged(bool value)
    {
        _sound.IsMuted = value;
    }

    [RelayCommand]
    private void ToggleTheme() => IsDarkTheme = !IsDarkTheme;

    [RelayCommand]
    private void ToggleMute() => IsMuted = !IsMuted;
}
