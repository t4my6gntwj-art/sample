using System;
using System.Threading.Tasks;

namespace AvaloniaDiApp.Contracts;

/// <summary>
/// 通信の「リクエスト・レスポンス」を非同期で扱うための同期化クライアント。
/// 生の通信イベントを、await で待機可能な形式にラップします。
/// </summary>
public interface INetworkClient
{
    /// <summary>
    /// リクエストを送信し、特定の ID のレスポンスが返るまで非同期で待機します。
    /// </summary>
    /// <param name="request">送信するバイナリデータ</param>
    /// <param name="expectedResponseId">期待するレスポンスのメッセージID</param>
    /// <param name="timeoutMs">タイムアウト時間（ミリ秒）</param>
    /// <param name="retryCount">タイムアウト時の最大試行回数</param>
    /// <returns>受信したレスポンスのバイナリデータ</returns>
    Task<byte[]> SendRequestAsync(byte[] request, byte expectedResponseId, int timeoutMs = 2000, int retryCount = 3);
}
