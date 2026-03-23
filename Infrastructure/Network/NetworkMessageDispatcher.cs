using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using AvaloniaDiApp.Contracts;

namespace AvaloniaDiApp.Infrastructure.Network;

/// <summary>
/// ネットワーク配送センター（ディスパッチャー）。
/// 通信回線からの入力を単一の並列安全なキューに集約し、
/// 専用のバックグラウンドスレッドで順番に放流（ブロードキャスト）します。
/// </summary>
public class NetworkMessageDispatcher : BackgroundService
{
    // 非同期・並列処理のためのキュー（高スループット設計）
    private readonly Channel<byte[]> _channel = Channel.CreateUnbounded<byte[]>();
    private readonly IEnumerable<INetworkTransport> _transports;

    /// <summary>
    /// キューから取り出された「処理の準備ができたデータ」を全方位に配送するためのイベント。
    /// 各リポジトリや通信クライアントがこれを購読します。
    /// </summary>
    public event Action<byte[]>? MessageReady;

    public NetworkMessageDispatcher(IEnumerable<INetworkTransport> transports)
    {
        _transports = transports;

        // 全てのトランスポート回線の入力を監視し、キューへ誘導する
        foreach (var t in _transports)
        {
            t.DataReceived += EnqueueLog;
        }
    }

    /// <summary>
    /// 受信したデータをキューに投入します。
    /// </summary>
    private void EnqueueLog(byte[] data)
    {
        _channel.Writer.TryWrite(data);
    }

    /// <summary>
    /// バックグラウンドワーカーのメインループ。
    /// 通信量が増えても、ここ（1つのスレッド）で順序を守って 1 つずつ処理します。
    /// </summary>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // キューにデータが入るまで非同期待機し、届いた順に配送
        await foreach (var data in _channel.Reader.ReadAllAsync(stoppingToken))
        {
            try 
            {
                // ここで「配送（放流）」を実行
                MessageReady?.Invoke(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Dispatcher Error] 配送中にエラーが発生しました: {ex.Message}");
            }
        }
    }
}
