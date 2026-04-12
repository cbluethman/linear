using System.Windows;
using System.Windows.Media;
using LinearAlgebra.App.Rendering;
using LinearAlgebra.App.ViewModels;

namespace LinearAlgebra.App;

public partial class MainWindow : Window
{
    private MainViewModel _viewModel = null!;

    public MainWindow()
    {
        InitializeComponent();
    }

    public void Initialize(MainViewModel viewModel)
    {
        _viewModel = viewModel;
        DataContext = viewModel;

        // Wire canvas to active lesson
        _viewModel.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(MainViewModel.ActiveLesson))
                OnActiveLessonChanged();
        };

        Canvas.WorldMouseDown += (x, y) => _viewModel.ActiveLesson?.OnMouseDown(x, y);
        Canvas.WorldMouseMove += (x, y) => _viewModel.ActiveLesson?.OnMouseMove(x, y);
        Canvas.WorldMouseUp += () => _viewModel.ActiveLesson?.OnMouseUp();

        // Animation tick
        CompositionTarget.Rendering += (_, _) => _viewModel.ActiveLesson?.OnTick();

        // Share the grid instance
        _viewModel.Grid.SetTheme(_viewModel.IsDarkTheme);

        OnActiveLessonChanged();
    }

    private void OnActiveLessonChanged()
    {
        if (_viewModel.ActiveLesson is ISceneRenderer renderer)
            Canvas.SetRenderer(renderer);
    }
}
