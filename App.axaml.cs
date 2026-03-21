using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using AvaloniaDiApp.ViewModels;
using AvaloniaDiApp.Views;
using AvaloniaDiApp.Services;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaDiApp;

public partial class App : Application
{
    private readonly IServiceProvider? _services;

    // プレビュー・デザイナー用
    public App()
    {
    }

    // DIによるコンストラクタインジェクション用
    public App(IServiceProvider services)
    {
        _services = services;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();

            // DI コンテナ (_services) から MainWindow を解決する
            // DI コンテナ (_services) からメインウィンドウを取得
            var mainWindow = _services?.GetRequiredService<MainWindow>();
            
            if (mainWindow != null)
            {
                // 起動時の初期画面（LayoutViewModel）をセット
                if (mainWindow.DataContext is MainWindowViewModel vm)
                {
                    vm.CurrentViewModel = _services?.GetRequiredService<LayoutViewModel>();
                }

                desktop.MainWindow = mainWindow;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}