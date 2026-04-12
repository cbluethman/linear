using System.Windows;
using System.Windows.Controls;
using LinearAlgebra.App.ViewModels.Lessons;

namespace LinearAlgebra.App.Views.Lessons;

public partial class EigenView : UserControl
{
    public EigenView()
    {
        InitializeComponent();
    }

    private EigenViewModel? VM => DataContext as EigenViewModel;

    private void OnSymmetric(object sender, RoutedEventArgs e)
    {
        if (VM == null) return;
        VM.M11 = 2; VM.M12 = 1; VM.M21 = 1; VM.M22 = 2;
    }

    private void OnRotation(object sender, RoutedEventArgs e)
    {
        if (VM == null) return;
        VM.M11 = 0; VM.M12 = -1; VM.M21 = 1; VM.M22 = 0;
    }

    private void OnDiagonal(object sender, RoutedEventArgs e)
    {
        if (VM == null) return;
        VM.M11 = 3; VM.M12 = 0; VM.M21 = 0; VM.M22 = 1;
    }
}
