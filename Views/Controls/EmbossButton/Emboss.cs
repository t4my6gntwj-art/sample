using System; // システム基本名前空間
using System.Reactive.Disposables; // リソース解放（Disposable）用
using System.Reactive.Linq; // Reactive Extensions (Rx) のクエリ演算子
using Avalonia; // Avalonia 基本名前空間
using Avalonia.Controls; // Avalonia コントロール
using Avalonia.Controls.Primitives; // Avalonia 基本コントロール（ToggleButtonなど）
using Avalonia.Media; // グラフィックス・描画関連

namespace AvaloniaDiApp.Views.Controls.EmbossButton; // 名前空間の定義

public enum EmbossStatus // エンボスボタンの状態を定義する列挙型
{
    Normal, // 通常状態
    Selected, // 選択状態
    Error, // エラー状態
    Disabled // 無効状態
}

public static class Emboss // エンボス効果を制御するための静的クラス（添付プロパティを提供）
{
    // エンボス効果を有効にするかどうかを制御する添付プロパティ
    public static readonly AttachedProperty<bool> IsEmbossedProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsEmbossed", typeof(Emboss), defaultValue: false);

    // エンボスボタンの現在のステータスを保持する添付プロパティ
    public static readonly AttachedProperty<EmbossStatus> StatusProperty =
        AvaloniaProperty.RegisterAttached<Control, EmbossStatus>("Status", typeof(Emboss), defaultValue: EmbossStatus.Normal);

    // 各状態に応じた表示コンテンツを保持する添付プロパティ
    public static readonly AttachedProperty<object?> NormalContentProperty = AvaloniaProperty.RegisterAttached<Control, object?>("NormalContent", typeof(Emboss));
    public static readonly AttachedProperty<object?> SelectedContentProperty = AvaloniaProperty.RegisterAttached<Control, object?>("SelectedContent", typeof(Emboss));
    public static readonly AttachedProperty<object?> ErrorContentProperty = AvaloniaProperty.RegisterAttached<Control, object?>("ErrorContent", typeof(Emboss));
    public static readonly AttachedProperty<object?> DisabledContentProperty = AvaloniaProperty.RegisterAttached<Control, object?>("DisabledContent", typeof(Emboss));

    // 実際にUIに表示されるコンテンツを保持する内部用添付プロパティ
    public static readonly AttachedProperty<object?> DisplayContentProperty = 
        AvaloniaProperty.RegisterAttached<Control, object?>("DisplayContent", typeof(Emboss));

    // 立体感を出すための各辺のブラシ（色）を保持する添付プロパティ
    public static readonly AttachedProperty<IBrush?> TopBrushProperty = AvaloniaProperty.RegisterAttached<Control, IBrush?>("TopBrush", typeof(Emboss));
    public static readonly AttachedProperty<IBrush?> BottomBrushProperty = AvaloniaProperty.RegisterAttached<Control, IBrush?>("BottomBrush", typeof(Emboss));
    public static readonly AttachedProperty<IBrush?> LeftBrushProperty = AvaloniaProperty.RegisterAttached<Control, IBrush?>("LeftBrush", typeof(Emboss));
    public static readonly AttachedProperty<IBrush?> RightBrushProperty = AvaloniaProperty.RegisterAttached<Control, IBrush?>("RightBrush", typeof(Emboss));

    // ベベル（面取り）の幅を指定する添付プロパティ
    public static readonly AttachedProperty<double> BevelWidthProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("BevelWidth", typeof(Emboss), defaultValue: 6.0);

    // 内部的なイベント購読を管理するための添付プロパティ
    private static readonly AttachedProperty<IDisposable?> SubscriptionProperty =
        AvaloniaProperty.RegisterAttached<Control, IDisposable?>("Subscription", typeof(Emboss));

    // IsEmbossed プロパティのゲッター
    public static bool GetIsEmbossed(Control element) => element.GetValue(IsEmbossedProperty);
    // IsEmbossed プロパティのセッター
    public static void SetIsEmbossed(Control element, bool value) => element.SetValue(IsEmbossedProperty, value);

    // Status プロパティのゲッター
    public static EmbossStatus GetStatus(Control element) => element.GetValue(StatusProperty);
    // Status プロパティのセッター
    public static void SetStatus(Control element, EmbossStatus value) => element.SetValue(StatusProperty, value);

    // NormalContent プロパティのゲッター
    public static object? GetNormalContent(Control element) => element.GetValue(NormalContentProperty);
    // NormalContent プロパティのセッター
    public static void SetNormalContent(Control element, object? value) => element.SetValue(NormalContentProperty, value);

    // SelectedContent プロパティのゲッター
    public static object? GetSelectedContent(Control element) => element.GetValue(SelectedContentProperty);
    // SelectedContent プロパティのセッター
    public static void SetSelectedContent(Control element, object? value) => element.SetValue(SelectedContentProperty, value);

    // ErrorContent プロパティのゲッター
    public static object? GetErrorContent(Control element) => element.GetValue(ErrorContentProperty);
    // ErrorContent プロパティのセッター
    public static void SetErrorContent(Control element, object? value) => element.SetValue(ErrorContentProperty, value);

    // DisabledContent プロパティのゲッター
    public static object? GetDisabledContent(Control element) => element.GetValue(DisabledContentProperty);
    // DisabledContent プロパティのセッター
    public static void SetDisabledContent(Control element, object? value) => element.SetValue(DisabledContentProperty, value);

    // 静的コンストラクタ：プロパティの変更ハンドラを登録
    static Emboss()
    {
        // IsEmbossed プロパティが変更された時の処理
        IsEmbossedProperty.Changed.AddClassHandler<Control>((control, e) =>
        {
            if (e.NewValue is bool isEmbossed && isEmbossed) // 有効化された場合
            {
                // デバッグ用：有効化をコンソールに通知
                Console.WriteLine($"[DEBUG] Emboss effect activated for: {control.GetType().Name} (Name: {control.Name})");

                // 既存の購読があれば解除
                control.GetValue(SubscriptionProperty)?.Dispose();

                // 複数の購読をまとめて管理するリストを作成
                var compositeDisposable = new CompositeDisposable();

                if (control is TemplatedControl tc) // TemplatedControl（Button等）の場合
                {
                    // 背景色に基づいてブラシ（影と光）を初期化
                    if (tc.Background is ISolidColorBrush initBrush)
                        UpdateBrushes(tc, initBrush.Color);

                    // 背景色が変更されたらブラシも再計算するように購読
                    tc.GetPropertyChangedObservable(TemplatedControl.BackgroundProperty)
                      .Subscribe(change => { if (change.NewValue is ISolidColorBrush brush) UpdateBrushes(tc, brush.Color); })
                      .DisposeWith(compositeDisposable);
                }

                // Status プロパティの変更を購読し、表示と疑似クラスを更新
                compositeDisposable.Add(control.GetPropertyChangedObservable(StatusProperty).Subscribe(_ => { UpdateDisplayContent(control); UpdatePseudoClasses(control, GetStatus(control)); }));
                // 各コンテンツプロパティの変更を購読
                compositeDisposable.Add(control.GetPropertyChangedObservable(NormalContentProperty).Subscribe(_ => UpdateDisplayContent(control)));
                compositeDisposable.Add(control.GetPropertyChangedObservable(SelectedContentProperty).Subscribe(_ => UpdateDisplayContent(control)));
                compositeDisposable.Add(control.GetPropertyChangedObservable(ErrorContentProperty).Subscribe(_ => UpdateDisplayContent(control)));
                compositeDisposable.Add(control.GetPropertyChangedObservable(DisabledContentProperty).Subscribe(_ => UpdateDisplayContent(control)));

                if (control is ContentControl cc) // ContentControl の Content プロパティ変更を購読
                    compositeDisposable.Add(cc.GetPropertyChangedObservable(ContentControl.ContentProperty).Subscribe(_ => UpdateDisplayContent(control)));
                
                if (control is ToggleButton tb) // ToggleButton の IsChecked プロパティ変更を購読
                    compositeDisposable.Add(tb.GetPropertyChangedObservable(ToggleButton.IsCheckedProperty).Subscribe(_ => { UpdateDisplayContent(control); UpdatePseudoClasses(control, GetStatus(control)); }));

                // 購読リストをプロパティに保存（後で解除するため）
                control.SetValue(SubscriptionProperty, compositeDisposable);

                // 初回の表示更新
                UpdateDisplayContent(control);
                UpdatePseudoClasses(control, GetStatus(control));
            }
            else // 無効化された場合、リソースを解放
            {
                control.GetValue(SubscriptionProperty)?.Dispose();
                control.ClearValue(SubscriptionProperty);
            }
        });
    }

    // 現在の状態に基づいて表示すべきコンテンツを決定する
    private static void UpdateDisplayContent(Control control)
    {
        if (!GetIsEmbossed(control)) return; // エンボス無効なら何もしない

        var status = GetStatus(control); // 現在のステータスを取得
        object? newContent = status switch // ステータス優先で表示内容を決定
        {
            EmbossStatus.Error => control.GetValue(ErrorContentProperty),
            EmbossStatus.Disabled => control.GetValue(DisabledContentProperty),
            _ => null
        };

        // トグルボタンの場合、チェック状態に応じてコンテンツを決定（ステータスが未指定の場合）
        if (newContent == null && control is ToggleButton tb)
            newContent = tb.IsChecked == true ? control.GetValue(SelectedContentProperty) : control.GetValue(NormalContentProperty);

        // まだ決まらなければ明示的なステータスから取得
        if (newContent == null)
            newContent = status switch
            {
                EmbossStatus.Normal => control.GetValue(NormalContentProperty),
                EmbossStatus.Selected => control.GetValue(SelectedContentProperty),
                _ => null
            };

        // 最終手段として元の Content プロパティを使用
        if (newContent == null && control is ContentControl cc)
            newContent = cc.Content;

        // DisplayContent プロパティに最終的な値をセット
        control.SetValue(DisplayContentProperty, newContent);
    }

    // ステータスに応じて XAML で利用可能な疑似クラス（:normal等）を更新する
    private static void UpdatePseudoClasses(Control control, EmbossStatus status)
    {
        var pc = (IPseudoClasses)control.Classes;

        // 外部から指定されたステータス、もしくは ToggleButton系 の IsChecked が true なら「選択状態」とみなす
        bool isActuallySelected = status == EmbossStatus.Selected || (control is ToggleButton tb && tb.IsChecked == true);

        pc.Set(":normal", status == EmbossStatus.Normal && !isActuallySelected); // 通常
        pc.Set(":selected", isActuallySelected); // 選択
        pc.Set(":error", status == EmbossStatus.Error); // エラー
        pc.Set(":status-disabled", status == EmbossStatus.Disabled); // ステータス起因の無効

        // ステータス別活性制御
        if (status == EmbossStatus.Disabled)
        {
            control.IsEnabled = false; // 無効状態なら操作不能にする
        }
        else if (isActuallySelected)
        {
            // ToggleButton（チェック解除が可能）の場合は活性を維持。
            // ただし RadioButton は一度選択されるとクリックによる解除ができないため非活性にする。
            control.IsEnabled = control is ToggleButton && control is not RadioButton;
        }
        else
        {
            control.IsEnabled = true; // それ以外は活性
        }
    }

    // 背景色から明るい色（光）と暗い色（影）を作成し、各辺のブラシに適用する
    public static void UpdateBrushes(Control control, Color baseColor)
    {
        control.SetValue(LeftBrushProperty, new SolidColorBrush(Lighten(baseColor, 0.4))); // 左：最も明るい
        control.SetValue(TopBrushProperty, new SolidColorBrush(Lighten(baseColor, 0.15))); // 上：少し明るい
        control.SetValue(BottomBrushProperty, new SolidColorBrush(Darken(baseColor, 0.25))); // 下：少し暗い
        control.SetValue(RightBrushProperty, new SolidColorBrush(Darken(baseColor, 0.5))); // 右：最も暗い
    }

    // 指定された割合で色を明るくする
    private static Color Lighten(Color baseColor, double factor) =>
        Color.FromUInt32(((uint)baseColor.A << 24) |
                        ((uint)Math.Clamp(baseColor.R + (255 - baseColor.R) * factor, 0, 255) << 16) |
                        ((uint)Math.Clamp(baseColor.G + (255 - baseColor.G) * factor, 0, 255) << 8) |
                        ((uint)Math.Clamp(baseColor.B + (255 - baseColor.B) * factor, 0, 255)));

    // 指定された割合で色を暗くする
    private static Color Darken(Color baseColor, double factor) =>
        Color.FromUInt32(((uint)baseColor.A << 24) |
                        ((uint)Math.Clamp(baseColor.R * (1 - factor), 0, 255) << 16) |
                        ((uint)Math.Clamp(baseColor.G * (1 - factor), 0, 255) << 8) |
                        ((uint)Math.Clamp(baseColor.B * (1 - factor), 0, 255)));

    // Rx の購読を CompositeDisposable に追加するための拡張メソッド
    public static IDisposable DisposeWith(this IDisposable disposable, CompositeDisposable compositeDisposable)
    {
        compositeDisposable.Add(disposable);
        return disposable;
    }
}
