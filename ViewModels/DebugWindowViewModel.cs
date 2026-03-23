using AvaloniaDiApp.Contracts;
using AvaloniaDiApp.Models.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;

namespace AvaloniaDiApp.ViewModels;

public partial class DebugWindowViewModel : ViewModelBase
{
    private readonly IPowerStatusReceiver _powerReceiver;
    private readonly IPowerStatusProvider _powerProvider; // 現在値を知るために Provider も使う

    [ObservableProperty] private bool _isDraining;

    public DebugWindowViewModel(IPowerStatusReceiver powerReceiver, IPowerStatusProvider powerProvider)
    {
        _powerReceiver = powerReceiver;
        _powerProvider = powerProvider;
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

                // バイト配列（パケット）を組み立てる
                byte[] data = new byte[3];
                data[0] = (byte)Math.Max(0, currentLevel - 5); // 5% 減らす
                data[1] = 0; // 充電してない
                data[2] = 0; // Performanceモード
                
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
        data[0] = 100;
        data[1] = 1; // 充電中
        data[2] = 0; // Performanceモード
        
        _powerReceiver.Receive(data);
    }
}
