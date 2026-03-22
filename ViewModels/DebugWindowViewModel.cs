using AvaloniaDiApp.Contracts;
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

                // 5% 減らす
                _powerReceiver.UpdateBatteryLevel(currentLevel - 5);
                
                await Task.Delay(1000);
            }
        }
        finally
        {
            IsDraining = false;
        }
    }

    [RelayCommand]
    private void StopDrain()
    {
        IsDraining = false;
    }

    [RelayCommand]
    private void ResetBattery()
    {
        _powerReceiver.UpdateBatteryLevel(100);
    }
}
