using Avalonia.Controls;
using AvaloniaDiApp.ViewModels;

namespace AvaloniaDiApp.Views;

public partial class MainWindow : Window
{
    // デザイナープレビュー用のデフォルトコンストラクタ
    public MainWindow()
    {
        InitializeComponent();
    }

    // DIから ViewModel が注入されるコンストラクタ
    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}