using System;
using AvaloniaDiApp.Models.Data;

namespace AvaloniaDiApp.Contracts;

// 入力ポート：データを流し込む側が使用する
public interface IConnectionStatusReceiver
{
    void UpdateIsOnline(bool isOnline);
    void UpdateConnectionType(string type);
    void UpdateSignalStrength(int strength);
}
