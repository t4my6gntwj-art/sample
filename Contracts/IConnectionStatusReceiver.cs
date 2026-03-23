using System;

namespace AvaloniaDiApp.Contracts;

/// <summary>
/// 接続状態の入力を受け取る（書き込み）ためのインターフェース。
/// </summary>
public interface IConnectionStatusReceiver
{
    void Receive(byte[] data);
}
