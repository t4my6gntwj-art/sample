using System;

namespace AvaloniaDiApp.Models.Data;

// 設定関連
public record Settings(string AppName, bool IsNotificationEnabled);

// 電源関連
public record PowerStatus(double BatteryLevel, bool IsCharging, string Mode);

// 接続関連
public record ConnectionStatus(bool IsOnline, string Type, int SignalStrength);

// 受信メッセージ用のレコード (null は「未受信・更新なし」を意味する)
public record PowerMessage(double? BatteryLevel = null, bool? IsCharging = null, string? Mode = null);
public record ConnectionMessage(bool? IsOnline = null, string? Type = null, int? SignalStrength = null);
