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
        => VM?.SetMatrixFields(2, 1, 1, 2);

    private void OnRotation(object sender, RoutedEventArgs e)
        => VM?.SetMatrixFields(0, -1, 1, 0);

    private void OnDiagonal(object sender, RoutedEventArgs e)
        => VM?.SetMatrixFields(3, 0, 0, 1);
}
