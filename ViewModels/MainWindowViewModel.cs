using CommunityToolkit.Mvvm.ComponentModel;
using AvaloniaDiApp.Views;
using Avalonia.Controls;

namespace AvaloniaDiApp.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";

    [ObservableProperty]
    private object? _currentViewModel;

    public MainWindowViewModel()
    {
        // TODO: DIコンテナからLayoutViewを取得するように変更する  
        //       IMainLayoutBuilder を作ってそこで、
        //       メイン画面のレイアウトのインスタンスを生成するようにするのがベストなんだろうけど、さすがにくどいので簡易にnewしておく
        _currentViewModel = new LayoutViewModel();
    }
}
