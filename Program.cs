using Avalonia;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaDiApp;

sealed class Program
{
    // アプリケーション全体でアクセス可能な Host インスタンス
    public static IHost? AppHost { get; private set; }

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // 1. Generic Host の構築
        AppHost = Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                // ここにViewModelやサービスの登録を追加していきます
                // 例: services.AddSingleton<MainWindowViewModel>();
            })
            .Build();

        // 2. Host の開始 (IHostedServiceなどのバックグラウンドタスクが動く)
        AppHost.Start();

        // 3. Avalonia アプリケーションを起動し、メインループに入る
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);

        // 4. アプリ終了時に Host を安全に停止・破棄する
        AppHost.StopAsync().GetAwaiter().GetResult();
        AppHost.Dispose();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
