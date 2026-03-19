using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaDiApp.ViewModels;

namespace AvaloniaDiApp.Views;

public partial class MainWindow : Window
{
    private readonly DebugWindow? _debugWindow;

    // デザイナープレビュー用のデフォルトコンストラクタ
    public MainWindow()
    {
        InitializeComponent();
    }

    // DIから ViewModel と DebugWindow が注入されるコンストラクタ
    public MainWindow(MainWindowViewModel viewModel, DebugWindow debugWindow)
    {
        InitializeComponent();
        DataContext = viewModel;
        
        _debugWindow = debugWindow;
        
        // メインウィンドウのUI要素の準備が完了したときのイベントに登録
        this.Loaded += MainWindow_Loaded;

        // メインウィンドウが閉じられたときのイベントに登録
        this.Closed += MainWindow_Closed;
    }

    private void MainWindow_Loaded(object? sender, RoutedEventArgs e)
    {
        // DebugWindow を別ウィンドウとして表示する
        // (引数に this を渡すことで、メインウィンドウを親に設定することも可能です)
        _debugWindow?.Show();
    }

    private void MainWindow_Closed(object? sender, System.EventArgs e)
    {
        // MainWindow が閉じられたら DebugWindow も連動して閉じる
        _debugWindow?.Close();
    }
}