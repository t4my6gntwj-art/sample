using AvaloniaDiApp.Contracts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AvaloniaDiApp.ViewModels;

public partial class LayoutViewModel : ViewModelBase, IContentHolder
{
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private ViewModelBase? _currentViewModel;

    // IContentHolder のイベント実装
    public event Action? CurrentViewModelChanged;

    // デザイナープレビュー用のデフォルトコンストラクタ
    public LayoutViewModel()
    {
        _navigationService = null!;
    }

    public LayoutViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
        
        // ナビゲーション完了時のイベントを購読して、自分のコンテンツを更新する
        _navigationService.Navigated += viewModel => CurrentViewModel = viewModel;
        
        // 初期表示をHomeに設定
        NavigateToHome();
    }

    partial void OnCurrentViewModelChanged(ViewModelBase? value)
    {
        CurrentViewModelChanged?.Invoke();
    }

    [RelayCommand]
    private void NavigateToHome() => _navigationService.NavigateTo<HomeViewModel>();

    [RelayCommand]
    private void NavigateToSetting() => _navigationService.NavigateTo<SettingViewModel>();

    [RelayCommand]
    private void NavigateToStatus() => _navigationService.NavigateTo<StatusViewModel>();
}