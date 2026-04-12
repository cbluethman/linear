using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LinearAlgebra.App.Rendering;
using LinearAlgebra.App.Services;
using SkiaSharp;

namespace LinearAlgebra.App.ViewModels.Lessons;

public abstract partial class LessonViewModelBase : ObservableObject, ISceneRenderer
{
    protected readonly GridRenderer Grid;
    protected readonly VectorRenderer Vectors = new();
    protected readonly ShapeRenderer Shapes = new();
    protected readonly SoundService Sound;
    protected readonly QuizService Quiz = new();

    [ObservableProperty] private string _explanationText = "";
    [ObservableProperty] private bool _isQuizMode;
    [ObservableProperty] private string _quizPrompt = "";
    [ObservableProperty] private string _quizFeedback = "";
    [ObservableProperty] private int _quizCorrect;
    [ObservableProperty] private int _quizTotal;

    public abstract string Title { get; }

    protected LessonViewModelBase(GridRenderer grid, SoundService sound)
    {
        Grid = grid;
        Sound = sound;
    }

    public abstract void Render(SKCanvas canvas, SKSize size);

    public virtual void OnMouseDown(double worldX, double worldY) { }
    public virtual void OnMouseMove(double worldX, double worldY) { }
    public virtual void OnMouseUp() { }
    public virtual void OnTick() { }

    [RelayCommand]
    public void ToggleQuiz()
    {
        IsQuizMode = !IsQuizMode;
        if (IsQuizMode)
        {
            QuizCorrect = 0;
            QuizTotal = 0;
            StartQuiz();
        }
        else
        {
            EndQuiz();
        }
    }

    protected virtual void StartQuiz() { }
    protected virtual void EndQuiz()
    {
        QuizPrompt = "";
        QuizFeedback = "";
    }

    protected void RecordAnswer(bool correct)
    {
        QuizTotal++;
        if (correct)
        {
            QuizCorrect++;
            QuizFeedback = "Correct!";
            Sound.PlayCorrect();
        }
        else
        {
            QuizFeedback = "Try again!";
            Sound.PlayWrong();
        }
    }
}
