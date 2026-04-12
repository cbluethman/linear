using System.Windows;
using LinearAlgebra.App.Services;
using LinearAlgebra.App.ViewModels;

namespace LinearAlgebra.App;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Wire up services (manual DI)
        var navigationService = new NavigationService();
        var lessonService = new LessonService();
        var soundService = new SoundService();

        var mainViewModel = new MainViewModel(navigationService, lessonService, soundService);

        var mainWindow = new MainWindow();
        mainWindow.Initialize(mainViewModel);
        mainWindow.Show();
    }
}
