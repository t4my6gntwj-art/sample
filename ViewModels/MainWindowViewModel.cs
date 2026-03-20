using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaDiApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}
