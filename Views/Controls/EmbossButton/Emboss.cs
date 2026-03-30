using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AvaloniaDiApp.Views.Controls.EmbossButton;

public enum EmbossStatus
{
    Normal,
    Selected,
    Error,
    Disabled
}

public static class Emboss
{
    public static readonly AttachedProperty<bool> IsEmbossedProperty =
        AvaloniaProperty.RegisterAttached<Control, bool>("IsEmbossed", typeof(Emboss), defaultValue: false);

    public static readonly AttachedProperty<EmbossStatus> StatusProperty =
        AvaloniaProperty.RegisterAttached<Control, EmbossStatus>("Status", typeof(Emboss), defaultValue: EmbossStatus.Normal);

    public static readonly AttachedProperty<object?> NormalContentProperty = AvaloniaProperty.RegisterAttached<Control, object?>("NormalContent", typeof(Emboss));
    public static readonly AttachedProperty<object?> SelectedContentProperty = AvaloniaProperty.RegisterAttached<Control, object?>("SelectedContent", typeof(Emboss));
    public static readonly AttachedProperty<object?> ErrorContentProperty = AvaloniaProperty.RegisterAttached<Control, object?>("ErrorContent", typeof(Emboss));
    public static readonly AttachedProperty<object?> DisabledContentProperty = AvaloniaProperty.RegisterAttached<Control, object?>("DisabledContent", typeof(Emboss));

    public static readonly AttachedProperty<object?> DisplayContentProperty = 
        AvaloniaProperty.RegisterAttached<Control, object?>("DisplayContent", typeof(Emboss));

    public static readonly AttachedProperty<IBrush?> TopBrushProperty = AvaloniaProperty.RegisterAttached<Control, IBrush?>("TopBrush", typeof(Emboss));
    public static readonly AttachedProperty<IBrush?> BottomBrushProperty = AvaloniaProperty.RegisterAttached<Control, IBrush?>("BottomBrush", typeof(Emboss));
    public static readonly AttachedProperty<IBrush?> LeftBrushProperty = AvaloniaProperty.RegisterAttached<Control, IBrush?>("LeftBrush", typeof(Emboss));
    public static readonly AttachedProperty<IBrush?> RightBrushProperty = AvaloniaProperty.RegisterAttached<Control, IBrush?>("RightBrush", typeof(Emboss));

    public static readonly AttachedProperty<double> BevelWidthProperty =
        AvaloniaProperty.RegisterAttached<Control, double>("BevelWidth", typeof(Emboss), defaultValue: 6.0);

    private static readonly AttachedProperty<IDisposable?> SubscriptionProperty =
        AvaloniaProperty.RegisterAttached<Control, IDisposable?>("Subscription", typeof(Emboss));

    public static bool GetIsEmbossed(Control element) => element.GetValue(IsEmbossedProperty);
    public static void SetIsEmbossed(Control element, bool value) => element.SetValue(IsEmbossedProperty, value);

    public static EmbossStatus GetStatus(Control element) => element.GetValue(StatusProperty);
    public static void SetStatus(Control element, EmbossStatus value) => element.SetValue(StatusProperty, value);

    public static object? GetNormalContent(Control element) => element.GetValue(NormalContentProperty);
    public static void SetNormalContent(Control element, object? value) => element.SetValue(NormalContentProperty, value);

    public static object? GetSelectedContent(Control element) => element.GetValue(SelectedContentProperty);
    public static void SetSelectedContent(Control element, object? value) => element.SetValue(SelectedContentProperty, value);

    public static object? GetErrorContent(Control element) => element.GetValue(ErrorContentProperty);
    public static void SetErrorContent(Control element, object? value) => element.SetValue(ErrorContentProperty, value);

    public static object? GetDisabledContent(Control element) => element.GetValue(DisabledContentProperty);
    public static void SetDisabledContent(Control element, object? value) => element.SetValue(DisabledContentProperty, value);

    static Emboss()
    {
        IsEmbossedProperty.Changed.AddClassHandler<Control>((control, e) =>
        {
            if (e.NewValue is bool isEmbossed && isEmbossed)
            {
                // デバッグ用ログ出力（ボタンがエンボス化された時に出力されるはず）
                Console.WriteLine($"[DEBUG] Emboss effect activated for: {control.GetType().Name} (Name: {control.Name})");

                control.GetValue(SubscriptionProperty)?.Dispose();

                var compositeDisposable = new CompositeDisposable();

                if (control is TemplatedControl tc)
                {
                    if (tc.Background is ISolidColorBrush initBrush)
                        UpdateBrushes(tc, initBrush.Color);

                    tc.GetPropertyChangedObservable(TemplatedControl.BackgroundProperty)
                      .Subscribe(change => { if (change.NewValue is ISolidColorBrush brush) UpdateBrushes(tc, brush.Color); })
                      .DisposeWith(compositeDisposable);
                }

                compositeDisposable.Add(control.GetPropertyChangedObservable(StatusProperty).Subscribe(_ => { UpdateDisplayContent(control); UpdatePseudoClasses(control, GetStatus(control)); }));
                compositeDisposable.Add(control.GetPropertyChangedObservable(NormalContentProperty).Subscribe(_ => UpdateDisplayContent(control)));
                compositeDisposable.Add(control.GetPropertyChangedObservable(SelectedContentProperty).Subscribe(_ => UpdateDisplayContent(control)));
                compositeDisposable.Add(control.GetPropertyChangedObservable(ErrorContentProperty).Subscribe(_ => UpdateDisplayContent(control)));
                compositeDisposable.Add(control.GetPropertyChangedObservable(DisabledContentProperty).Subscribe(_ => UpdateDisplayContent(control)));

                if (control is ContentControl cc)
                    compositeDisposable.Add(cc.GetPropertyChangedObservable(ContentControl.ContentProperty).Subscribe(_ => UpdateDisplayContent(control)));
                
                if (control is ToggleButton tb)
                    compositeDisposable.Add(tb.GetPropertyChangedObservable(ToggleButton.IsCheckedProperty).Subscribe(_ => UpdateDisplayContent(control)));

                control.SetValue(SubscriptionProperty, compositeDisposable);

                UpdateDisplayContent(control);
                UpdatePseudoClasses(control, GetStatus(control));
            }
            else
            {
                control.GetValue(SubscriptionProperty)?.Dispose();
                control.ClearValue(SubscriptionProperty);
            }
        });
    }

    private static void UpdateDisplayContent(Control control)
    {
        if (!GetIsEmbossed(control)) return;

        var status = GetStatus(control);
        object? newContent = status switch
        {
            EmbossStatus.Error => control.GetValue(ErrorContentProperty),
            EmbossStatus.Disabled => control.GetValue(DisabledContentProperty),
            _ => null
        };

        if (newContent == null && control is ToggleButton tb)
            newContent = tb.IsChecked == true ? control.GetValue(SelectedContentProperty) : control.GetValue(NormalContentProperty);

        if (newContent == null)
            newContent = status switch
            {
                EmbossStatus.Normal => control.GetValue(NormalContentProperty),
                EmbossStatus.Selected => control.GetValue(SelectedContentProperty),
                _ => null
            };

        if (newContent == null && control is ContentControl cc)
            newContent = cc.Content;

        control.SetValue(DisplayContentProperty, newContent);
    }

    private static void UpdatePseudoClasses(Control control, EmbossStatus status)
    {
        var pc = (IPseudoClasses)control.Classes;
        pc.Set(":normal", status == EmbossStatus.Normal);
        pc.Set(":selected", status == EmbossStatus.Selected);
        pc.Set(":error", status == EmbossStatus.Error);
        pc.Set(":status-disabled", status == EmbossStatus.Disabled);

        // 【修正】ステータス別活性制御
        if (status == EmbossStatus.Disabled)
        {
            control.IsEnabled = false;
        }
        else if (status == EmbossStatus.Selected)
        {
            // トグルボタンやラジオボタンなら活性を維持（オフへの切り替え等が必要なため）
            // 通常のボタンなら非活性にする（二重実行防止、状態表示としてのみ機能させる）
            control.IsEnabled = control is ToggleButton;
        }
        else
        {
            control.IsEnabled = true;
        }
    }

    public static void UpdateBrushes(Control control, Color baseColor)
    {
        control.SetValue(LeftBrushProperty, new SolidColorBrush(Lighten(baseColor, 0.4)));
        control.SetValue(TopBrushProperty, new SolidColorBrush(Lighten(baseColor, 0.15)));
        control.SetValue(BottomBrushProperty, new SolidColorBrush(Darken(baseColor, 0.25)));
        control.SetValue(RightBrushProperty, new SolidColorBrush(Darken(baseColor, 0.5)));
    }

    private static Color Lighten(Color baseColor, double factor) =>
        Color.FromUInt32(((uint)baseColor.A << 24) |
                        ((uint)Math.Clamp(baseColor.R + (255 - baseColor.R) * factor, 0, 255) << 16) |
                        ((uint)Math.Clamp(baseColor.G + (255 - baseColor.G) * factor, 0, 255) << 8) |
                        ((uint)Math.Clamp(baseColor.B + (255 - baseColor.B) * factor, 0, 255)));

    private static Color Darken(Color baseColor, double factor) =>
        Color.FromUInt32(((uint)baseColor.A << 24) |
                        ((uint)Math.Clamp(baseColor.R * (1 - factor), 0, 255) << 16) |
                        ((uint)Math.Clamp(baseColor.G * (1 - factor), 0, 255) << 8) |
                        ((uint)Math.Clamp(baseColor.B * (1 - factor), 0, 255)));

    // DisposeWith の簡易実装
    public static IDisposable DisposeWith(this IDisposable disposable, CompositeDisposable compositeDisposable)
    {
        compositeDisposable.Add(disposable);
        return disposable;
    }
}
