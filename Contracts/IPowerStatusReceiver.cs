using System;
using AvaloniaDiApp.Models.Data;

namespace AvaloniaDiApp.Contracts;

// 入力ポート：データを流し込む側が使用する
public interface IPowerStatusReceiver
{
    void UpdateBatteryLevel(double level);
    void UpdateIsCharging(bool isCharging);
    void UpdatePowerMode(string mode);
}
