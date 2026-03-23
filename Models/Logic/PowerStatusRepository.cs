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

    // バイナリ受信時の解析処理を実装
    public void Receive(byte[] data)
    {
        if (data == null || data.Length < 3) return;

        // 簡単なパース処理（バイナリからドメインモデルへ）
        var battery = (double)data[0];
        var isCharging = data[1] == 1;
        var mode = data[2] switch
        {
            0 => "Performance",
            1 => "Silent",
            2 => "Balanced",
            _ => "Unknown"
        };

        // 状態の更新
        _status = new PowerStatus(
            BatteryLevel: Math.Clamp(battery, 0, 100),
            IsCharging: isCharging,
            Mode: mode
        );

        // 通知
        StatusUpdated?.Invoke(_status);
    }
}
