using Avalonia.Controls;
using AvaloniaDiApp.ViewModels;

namespace AvaloniaDiApp.Views;

public partial class DebugWindow : Window
{
    // デザイナープレビュー用のデフォルトコンストラクタ
    public DebugWindow()
    {
        InitializeComponent();
    }

    // DIから ViewModel が注入されるコンストラクタ
    public DebugWindow(DebugWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}
