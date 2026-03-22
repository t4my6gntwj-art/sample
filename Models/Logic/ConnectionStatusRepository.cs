using System;
using AvaloniaDiApp.Models.Data;
using AvaloniaDiApp.Contracts;

namespace AvaloniaDiApp.Models.Logic;

public class ConnectionStatusRepository : IConnectionStatusProvider, IConnectionStatusReceiver
{
    private ConnectionStatus _status = new(true, "WiFi", 80);
    public event Action<ConnectionStatus>? StatusUpdated;

    public ConnectionStatus CurrentStatus => _status;

    public void RequestUpdate() => StatusUpdated?.Invoke(_status);

    public void UpdateIsOnline(bool isOnline)
    {
        _status = _status with { IsOnline = isOnline };
        StatusUpdated?.Invoke(_status);
    }

    public void UpdateConnectionType(string type)
    {
        _status = _status with { Type = type };
        StatusUpdated?.Invoke(_status);
    }

    public void UpdateSignalStrength(int strength)
    {
        _status = _status with { SignalStrength = Math.Clamp(strength, 0, 100) };
        StatusUpdated?.Invoke(_status);
    }
}
