using Avalonia;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using AvaloniaDiApp.Views;
using AvaloniaDiApp.ViewModels;
using AvaloniaDiApp.Contracts;
using AvaloniaDiApp.Models.Data;
using AvaloniaDiApp.Models.Logic;
using AvaloniaDiApp.Infrastructure.Navigation;
using AvaloniaDiApp.Infrastructure.Network;

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
                // サービスの登録
                services.AddSingleton<LayoutViewModel>();
                services.AddSingleton<IContentHolder>(sp => (IContentHolder)sp.GetRequiredService<LayoutViewModel>());
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<ISettingsService, SettingsService>();

                // Power リポジトリの登録 (1つのシングルトンを2つの口で公開)
                services.AddSingleton<PowerStatusRepository>();
                services.AddSingleton<IPowerStatusProvider>(sp => sp.GetRequiredService<PowerStatusRepository>());
                services.AddSingleton<IPowerStatusReceiver>(sp => sp.GetRequiredService<PowerStatusRepository>());

                // Connection リポジトリの登録
                services.AddSingleton<ConnectionStatusRepository>();
                services.AddSingleton<IConnectionStatusProvider>(sp => sp.GetRequiredService<ConnectionStatusRepository>());
                services.AddSingleton<IConnectionStatusReceiver>(sp => sp.GetRequiredService<ConnectionStatusRepository>());

                // トランスポート & 配送デリバリーの登録
                services.AddSingleton<FakeNetworkTransport>();
                services.AddSingleton<INetworkTransport>(sp => sp.GetRequiredService<FakeNetworkTransport>());
                
                // NetworkMessageDispatcher をシングルトン登録、かつホストサービスとしても登録
                services.AddSingleton<NetworkMessageDispatcher>();
                services.AddHostedService<NetworkMessageDispatcher>(sp => sp.GetRequiredService<NetworkMessageDispatcher>());

                // 魔法のクライアント (同期化ラッパー)
                services.AddSingleton<INetworkClient, NetworkClient>();

                // 設定シナリオサービス
                services.AddSingleton<IDeviceSetupService, DeviceSetupService>();

                // 模擬デバイス (テスト用)
                services.AddSingleton<MockDeviceService>();

                // App自身の登録
                services.AddSingleton<App>();

                // ViewModelの登録
                services.AddSingleton<MainWindowViewModel>();
                services.AddSingleton<DebugWindowViewModel>();
                services.AddSingleton<LayoutViewModel>();
                services.AddTransient<HomeViewModel>();
                services.AddTransient<SettingViewModel>();
                services.AddTransient<StatusViewModel>();

                // Viewの登録
                services.AddTransient<MainWindow>();
                services.AddTransient<DebugWindow>();
                services.AddTransient<HomeView>();
                services.AddTransient<SettingView>();
                services.AddTransient<StatusView>();
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
