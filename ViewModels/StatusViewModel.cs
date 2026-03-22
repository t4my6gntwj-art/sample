using AvaloniaDiApp.Contracts;
using AvaloniaDiApp.Models.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace AvaloniaDiApp.ViewModels;

public partial class StatusViewModel : ViewModelBase
{
    private readonly IPowerStatusProvider _powerProvider;
    private readonly IConnectionStatusProvider _connectionProvider;

    // 電源状態
    [ObservableProperty] private double _batteryLevel;
    [ObservableProperty] private bool _isCharging;
    [ObservableProperty] private string _powerMode = string.Empty;

    // 接続状態
    [ObservableProperty] private bool _isOnline;
    [ObservableProperty] private string _connectionType = string.Empty;
    [ObservableProperty] private int _signalStrength;

    [ObservableProperty] private string _lastUpdated = string.Empty;

    public StatusViewModel(IPowerStatusProvider powerProvider, IConnectionStatusProvider connectionProvider)
    {
        _powerProvider = powerProvider;
        _connectionProvider = connectionProvider;

        // イベント購読
        _powerProvider.StatusUpdated += OnPowerStatusUpdated;
        _connectionProvider.StatusUpdated += OnConnectionStatusUpdated;

        // 初期値のセット
        UpdateFromPower(_powerProvider.CurrentStatus);
        UpdateFromConnection(_connectionProvider.CurrentStatus);
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
        _powerProvider.RequestUpdate();
        _connectionProvider.RequestUpdate();
    }
}
