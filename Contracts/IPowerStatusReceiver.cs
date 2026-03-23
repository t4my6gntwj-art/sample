using System;

namespace AvaloniaDiApp.Contracts;

/// <summary>
/// 電源情報の入力を受け取る（書き込み）ためのインターフェース。
/// ネットワーク通信やシミュレーターなど、「情報を更新したい側」が使用します。
/// </summary>
public interface IPowerStatusReceiver
{
    /// <summary>
    /// 生のバイナリデータを受け取り、電源状態を更新・変換します。
    /// </summary>
    void Receive(byte[] data);
}
