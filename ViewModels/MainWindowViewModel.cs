using CommunityToolkit.Mvvm.ComponentModel;
using AvaloniaDiApp.Views;
using Avalonia.Controls;

namespace AvaloniaDiApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";

    // プロパティのみを持ち、中身は外部からセットされるのを待つ
    [ObservableProperty]
    private object? _currentViewModel;

    public MainWindowViewModel()
    {
    }
}
