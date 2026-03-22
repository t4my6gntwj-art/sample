using AvaloniaDiApp.Contracts;
using AvaloniaDiApp.Models.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace AvaloniaDiApp.ViewModels;

public partial class StatusViewModel : ViewModelBase
{
    private readonly IPowerStatusService _powerService;
    private readonly IConnectionStatusService _connectionService;

    // 電源状態
    [ObservableProperty] private double _batteryLevel;
    [ObservableProperty] private bool _isCharging;
    [ObservableProperty] private string _powerMode = string.Empty;

    // 接続状態
    [ObservableProperty] private bool _isOnline;
    [ObservableProperty] private string _connectionType = string.Empty;
    [ObservableProperty] private int _signalStrength;

    [ObservableProperty] private string _lastUpdated = string.Empty;

    public StatusViewModel(IPowerStatusService powerService, IConnectionStatusService connectionService)
    {
        _powerService = powerService;
        _connectionService = connectionService;

        // イベント購読
        _powerService.StatusUpdated += OnPowerStatusUpdated;
        _connectionService.StatusUpdated += OnConnectionStatusUpdated;

        // 初期値のセット
        UpdateFromPower(_powerService.CurrentStatus);
        UpdateFromConnection(_connectionService.CurrentStatus);
    }

    private void OnPowerStatusUpdated(PowerStatus status) => UpdateFromPower(status);
    private void OnConnectionStatusUpdated(ConnectionStatus status) => UpdateFromConnection(status);

    private void UpdateFromPower(PowerStatus status)
    {
        BatteryLevel = status.BatteryLevel;
        IsCharging = status.IsCharging;
        PowerMode = status.Mode;
        RefreshTimestamp();
    }

    private void UpdateFromConnection(ConnectionStatus status)
    {
        IsOnline = status.IsOnline;
        ConnectionType = status.Type;
        SignalStrength = status.SignalStrength;
        RefreshTimestamp();
    }

    private void RefreshTimestamp()
    {
        LastUpdated = DateTime.Now.ToString("HH:mm:ss");
    }

    [RelayCommand]
    private void Refresh()
    {
        _powerService.RequestUpdate();
        _connectionService.RequestUpdate();
    }
}
