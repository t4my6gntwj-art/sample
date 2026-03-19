using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaDiApp.ViewModels;

public partial class DebugWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _debugMessage = "This is a Debug Window!";

    [RelayCommand]
    private void Test()
    {
        DebugMessage = "Test Button Clicked!";
    }
}
