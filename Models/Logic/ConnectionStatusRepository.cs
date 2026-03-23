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

    public void Receive(byte[] data)
    {
        if (data == null || data.Length < 3) return;

        var isOnline = data[0] == 1;
        var type = data[1] switch
        {
            0 => "WiFi",
            1 => "Ethernet",
            2 => "Cellular",
            _ => "Unknown"
        };
        var signal = (int)data[2];

        _status = new ConnectionStatus(
            IsOnline: isOnline,
            Type: type,
            SignalStrength: Math.Clamp(signal, 0, 100)
        );

        StatusUpdated?.Invoke(_status);
    }
}
