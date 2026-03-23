using System;
using AvaloniaDiApp.Models.Data;
using AvaloniaDiApp.Contracts;
using AvaloniaDiApp.Infrastructure.Network;

namespace AvaloniaDiApp.Models.Logic;

/// <summary>
/// 電源情報の番人（リポジトリ）。
/// 配送センターからのデータを監視し、自分宛のデータであれば最新状態（PowerStatus）を更新・蓄積します。
/// </summary>
public class PowerStatusRepository : IPowerStatusProvider, IPowerStatusReceiver
{
    private PowerStatus _status = new(100.0, false, "Performance");
    public event Action<PowerStatus>? StatusUpdated;

    public PowerStatus CurrentStatus => _status;

    public void RequestUpdate() => StatusUpdated?.Invoke(_status);

    public PowerStatusRepository(NetworkMessageDispatcher dispatcher)
    {
        // 配送センターが「新しい荷物（バイナリ）」を放流したらチェック
        dispatcher.MessageReady += (data) => 
        {
            // ID: 0x01 は「電源（電池）メッセージ」というマッピング定義
            if (data != null && data.Length > 0 && data[0] == 0x01) 
            {
                Receive(data);
            }
        };
    }

    /// <summary>
    /// バイナリデータを解析（パース）して、高レイヤーの PowerStatus 構造体に変換・保存します。
    /// </summary>
    public void Receive(byte[] data)
    {
        if (data == null || data.Length < 3) return;

        var battery = (double)data[0];
        var isCharging = data[1] == 1;
        var mode = data[2] switch
        {
            0 => "Performance",
            1 => "Silent",
            2 => "Balanced",
            _ => "Unknown"
        };

        var newStatus = new PowerStatus(
            BatteryLevel: Math.Clamp(battery, 0, 100),
            IsCharging: isCharging,
            Mode: mode
        );

        // 値に変化があった場合のみ、状態を更新して通知する
        if (newStatus != _status)
        {
            _status = newStatus;
            StatusUpdated?.Invoke(_status);
        }
    }
}
