using Avalonia;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using AvaloniaDiApp.Views;
using AvaloniaDiApp.ViewModels;

namespace AvaloniaDiApp;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // 1. Generic Host の構築 (ローカル変数として保持しスタティック公開を避ける)
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                // App自身の登録
                services.AddSingleton<App>();

                // ViewModelの登録
                services.AddTransient<MainWindowViewModel>();

                // Viewの登録
                services.AddTransient<MainWindow>();
            })
            .Build();

        // 2. Host の開始
        host.Start();

        // 3. Avalonia アプリケーション起動 (DIからAppクラスを生成)
        BuildAvaloniaApp(host.Services)
            .StartWithClassicDesktopLifetime(args);

        // 4. アプリ終了時に Host を安全に停止・破棄する
        host.StopAsync().GetAwaiter().GetResult();
        host.Dispose();
    }

    // ランタイム用の設定 (DIからAppを取得する)
    public static AppBuilder BuildAvaloniaApp(IServiceProvider services)
        => AppBuilder.Configure(() => services.GetRequiredService<App>())
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
