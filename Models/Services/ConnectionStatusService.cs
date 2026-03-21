using System;
using AvaloniaDiApp.Models.Data;
using AvaloniaDiApp.Services;

namespace AvaloniaDiApp.Models.Services;

public class ConnectionStatusService : IConnectionStatusService
{
    private ConnectionStatus _status = new(true, "WiFi", 80);
    public event Action<ConnectionStatus>? StatusUpdated;

    public ConnectionStatus CurrentStatus => _status;

    public void RequestUpdate()
    {
        // 模擬データの更新
        _status = _status with 
        { 
            SignalStrength = new Random().Next(0, 100)
        };
        StatusUpdated?.Invoke(_status);
    }
}
