using System;
using AvaloniaDiApp.Models.Data;
using AvaloniaDiApp.Contracts;

namespace AvaloniaDiApp.Models.Logic;

public class PowerStatusService : IPowerStatusService
{
    private PowerStatus _status = new(100.0, false, "Performance");
    public event Action<PowerStatus>? StatusUpdated;

    public PowerStatus CurrentStatus => _status;

    public void RequestUpdate()
    {
        // 模擬データの更新
        _status = _status with 
        { 
            BatteryLevel = Math.Max(0, _status.BatteryLevel - 1.0),
            IsCharging = false
        };
        StatusUpdated?.Invoke(_status);
    }
}
