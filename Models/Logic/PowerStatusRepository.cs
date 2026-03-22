using System;
using AvaloniaDiApp.Models.Data;
using AvaloniaDiApp.Contracts;

namespace AvaloniaDiApp.Models.Logic;

public class PowerStatusRepository : IPowerStatusProvider, IPowerStatusReceiver
{
    private PowerStatus _status = new(100.0, false, "Performance");
    public event Action<PowerStatus>? StatusUpdated;

    public PowerStatus CurrentStatus => _status;

    public void RequestUpdate() => StatusUpdated?.Invoke(_status);

    public void UpdateBatteryLevel(double level)
    {
        _status = _status with { BatteryLevel = Math.Clamp(level, 0, 100) };
        StatusUpdated?.Invoke(_status);
    }

    public void UpdateIsCharging(bool isCharging)
    {
        _status = _status with { IsCharging = isCharging };
        StatusUpdated?.Invoke(_status);
    }

    public void UpdatePowerMode(string mode)
    {
        _status = _status with { Mode = mode };
        StatusUpdated?.Invoke(_status);
    }
}
