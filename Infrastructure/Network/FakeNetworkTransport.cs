using System;
using AvaloniaDiApp.Contracts;

namespace AvaloniaDiApp.Infrastructure.Network;

/// <summary>
/// 模擬的なネットワーク転送クラス（シミュレーター）。
/// 物理的な回線がない環境でも、メモリ上で受信をシミュレートしたり、ループバック通信を行ったりします。
/// </summary>
public class FakeNetworkTransport : INetworkTransport
{
    /// <summary>
    /// データを受信した際に通知されるイベント。
    /// </summary>
    public event Action<byte[]>? DataReceived;

    /// <summary>
    /// 外部への送信。デバッグ用として「送ったものを自分で受信する（ループバック）」挙動をします。
    /// </summary>
    public void Send(byte[] data)
    {
        // 開発の便宜上、送信したものをそのまま受信イベントへ流す
        DataReceived?.Invoke(data);
    }

    /// <summary>
    /// 外部（デバッグ画面など）から「データを受信したことにする」ためのシミュレーションメソッド。
    /// 玄関（DataReceived）から入れることで、正規の配送ルートを通るようにします。
    /// </summary>
    public void SimulateReceive(byte[] data)
    {
        DataReceived?.Invoke(data);
    }
}
