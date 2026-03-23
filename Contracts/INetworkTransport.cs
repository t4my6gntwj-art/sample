using System;

namespace AvaloniaDiApp.Contracts;

/// <summary>
/// ネットワーク転送（トランスポート）の抽象化。
/// 通信方式（TCP, UDP, Serial等）を問わず、「送る・受け取る」という最低限の機能を提供します。
/// </summary>
public interface INetworkTransport
{
    /// <summary>
    /// 生のバイナリデータを外部（デバイスなど）へ送信します。
    /// </summary>
    void Send(byte[] data);

    /// <summary>
    /// 外部（デバイスなど）からデータを受信した際に発生するイベント。
    /// </summary>
    event Action<byte[]>? DataReceived;
}
