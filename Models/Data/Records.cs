using System;

namespace AvaloniaDiApp.Models.Data;

// 設定関連
public record Settings(string AppName, bool IsNotificationEnabled);

// 電源関連
public record PowerStatus(double BatteryLevel, bool IsCharging, string Mode);

// 接続関連
public record ConnectionStatus(bool IsOnline, string Type, int SignalStrength);
