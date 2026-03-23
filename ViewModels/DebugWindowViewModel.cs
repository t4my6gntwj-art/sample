using AvaloniaDiApp.Contracts;
using AvaloniaDiApp.Models.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace AvaloniaDiApp.ViewModels;

/// <summary>
/// デバッグウィンドウ用の View 制御ロジック（ViewModel）。
/// モデル界の機能（SetupService や Repository）を呼び出し、その進捗を画面へ反映します。
/// </summary>
public partial class DebugWindowViewModel : ViewModelBase
{
    private readonly IPowerStatusReceiver _powerReceiver;
    private readonly IPowerStatusProvider _powerProvider;
    private readonly IDeviceSetupService _setupService; // ビジネスロジックとしての手順担当

    [ObservableProperty] private bool _isDraining;
    [ObservableProperty] private string _log = "待機中...";

    public DebugWindowViewModel(
        IPowerStatusReceiver powerReceiver, 
        IPowerStatusProvider powerProvider, 
        IDeviceSetupService setupService)
    {
        _powerReceiver = powerReceiver;
        _powerProvider = powerProvider;
        _setupService = setupService;
    }

    /// <summary>
    /// モデル内の「設定シーケンス」を開始し、その進捗をログ画面に表示します。
    /// </summary>
    [RelayCommand]
    private async Task RunSetupSequence()
    {
        // サービスからの進捗報告 (Report) を、ViewModel の Log プロパティに橋渡しする
        var progress = new Progress<string>(msg => Log = msg);

        try 
        {
            // モデル層に全ての仕様を任せる、最も幸せな呼び出しの形。
            await _setupService.RunSetupAsync(progress);
        }
        catch (Exception)
        {
            // エラー表示自体は、サービスが Report を通じて出してくれているので
            // ここでは UI 側の後片付けなどがあれば行う。
        }
    }

    [RelayCommand]
    private async Task StartDrain()
    {
        if (IsDraining) return;
        IsDraining = true;

        try
        {
            while (IsDraining)
            {
                var currentLevel = _powerProvider.CurrentStatus.BatteryLevel;
                if (currentLevel <= 0) break;

                // 疑似的な「電池状態バイナリ(ID:0x01)」を作成
                byte[] data = new byte[3];
                data[0] = 0x01; // メッセージID
                data[1] = (byte)Math.Max(0, currentLevel - 5); // 残量
                data[2] = 0; // 充電無
                
                // 本来の口（Receiver）から流し込む
                _powerReceiver.Receive(data);
                
                await Task.Delay(1000);
            }
        }
        finally
        {
            IsDraining = false;
        }
    }

    [RelayCommand]
    private void StopDrain() => IsDraining = false;

    [RelayCommand]
    private void ResetBattery()
    {
        // 100%かつ充電中のパケットを作成
        byte[] data = new byte[3];
        data[0] = 0x01; // ID
        data[1] = 100;
        data[2] = 1; // 充電中
        
        _powerReceiver.Receive(data);
    }
}
