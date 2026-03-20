using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AvaloniaDiApp.ViewModels;

public partial class DebugWindowViewModel : ViewModelBase
{
    private readonly IServiceProvider? _services;

    // プレビュー・デザイナー用のデフォルトコンストラクタ
    public DebugWindowViewModel()
    {
    }

    // DIコンテナから注入されるコンストラクタ
    public DebugWindowViewModel(IServiceProvider services)
    {
        _services = services;
    }

    [ObservableProperty]
    private string _debugMessage = "This is a Debug Window!";

    [RelayCommand]
    private void Test()
    {
        // サービスプロバイダが注入されているか確認するメッセージ
        if (_services != null)
        {
            DebugMessage = "Test Button Clicked! (ServiceProvider 注入済み)";

            var mainWindowViewModel = _services.GetRequiredService<MainWindowViewModel>();
            mainWindowViewModel.Greeting = "Test Button Clicked! (ServiceProvider 注入済み)";
        }
        else
        {
            DebugMessage = "Test Button Clicked! (ServiceProvider なし)";
        }
    }
}
