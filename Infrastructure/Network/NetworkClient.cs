using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaDiApp.Contracts;

namespace AvaloniaDiApp.Infrastructure.Network;

/// <summary>
/// 通信の「同期化ラッパー（魔法の杖）」。
/// 非同期な通信イベントを await で待機可能なTaskに変換し、リトライやタイムアウトの制御をカプセル化します。
/// </summary>
public class NetworkClient : INetworkClient
{
    private readonly INetworkTransport _transport;
    
    // 「今、どのレスポンス ID を待っているか」のチケット（TCS）リスト
    private readonly ConcurrentDictionary<byte, TaskCompletionSource<byte[]>> _pendingRequests 
        = new ConcurrentDictionary<byte, TaskCompletionSource<byte[]>>();

    public NetworkClient(INetworkTransport transport, NetworkMessageDispatcher dispatcher)
    {
        _transport = transport;
        
        // 配送センターから「仕分け済みデータ」を受け取り、待っている人がいれば「ハンコ」を押す
        dispatcher.MessageReady += HandleDataReceived;
    }

    private void HandleDataReceived(byte[] data)
    {
        if (data == null || data.Length == 0) return;

        // プロトコル：1バイト目をメッセージIDとして解釈
        byte msgId = data[0]; 

        // このIDを「待っている」人がリストにいるか確認
        if (_pendingRequests.TryRemove(msgId, out var tcs))
        {
            // 待機中の Task にデータを流し込んで完了させる（これでawaitが再開される）
            tcs.TrySetResult(data);
        }
    }

    /// <summary>
    /// 通信シーケンスの対話を実行します（リトライ付）。
    /// </summary>
    public async Task<byte[]> SendRequestAsync(byte[] request, byte expectedResponseId, int timeoutMs = 2000, int retryCount = 3)
    {
        for (int i = 0; i < retryCount; i++)
        {
            try
            {
                return await SendAndReceiveOneShotAsync(request, expectedResponseId, timeoutMs);
            }
            catch (TimeoutException)
            {
                if (i == retryCount - 1) throw; // 最終試行でもダメなら例外

                await Task.Delay(500); // 再試行までのウェイト
                Console.WriteLine($"[NetworkClient] 応答なし。再試行中... ({i + 1}/{retryCount})");
            }
        }
        throw new Exception("Unexpected error in retry loop");
    }

    private async Task<byte[]> SendAndReceiveOneShotAsync(byte[] request, byte expectedResponseId, int timeoutMs)
    {
        var tcs = new TaskCompletionSource<byte[]>();
        _pendingRequests[expectedResponseId] = tcs;

        try
        {
            // 下位のトランスポートに送信を指示
            _transport.Send(request);

            // TaskCompletionSource か タイムアウト の早い方を待機
            using (var cts = new CancellationTokenSource(timeoutMs))
            {
                using (cts.Token.Register(() => tcs.TrySetCanceled()))
                {
                    try 
                    {
                        return await tcs.Task;
                    }
                    catch (TaskCanceledException)
                    {
                        throw new TimeoutException($"メッセージ(ID:0x{expectedResponseId:X2}) の応答が {timeoutMs}ms 以内にありませんでした。");
                    }
                }
            }
        }
        finally
        {
            _pendingRequests.TryRemove(expectedResponseId, out _);
        }
    }
}
