using System;
using System.Threading.Tasks;
using AvaloniaDiApp.Contracts;

namespace AvaloniaDiApp.Models.Logic;

/// <summary>
/// デバイス初期設定サービス（手順書をプログラミング化したもの）。
/// 低レイヤーの通信の詳細を隠蔽し、一本道（手続き型）の設定シーケンスを提供します。
/// </summary>
public class DeviceSetupService : IDeviceSetupService
{
    private readonly INetworkClient _client;

    public DeviceSetupService(INetworkClient client)
    {
        _client = client;
    }

    /// <summary>
    /// 特定の「一連の手順（シナリオ）」を完遂します。
    /// </summary>
    public async Task RunSetupAsync(IProgress<string> progress)
    {
        progress.Report("--- 機器設定シーケンスを開始します ---");

        try 
        {
            // 魔法のクライアント (INetworkClient) により、「送る→返信待ち」が単純な await に集約さる
            
            // 1. デバイスに「初期化要求(0x10)」を出し、「完了通知(0x11)」を待つ
            progress.Report("1/2: デバイスへ初期化コマンド(0x10)を送信中...");
            var res1 = await _client.SendRequestAsync(new byte[] { 0x10 }, 0x11, timeoutMs: 3000);
            progress.Report($"初期化成功！ (ステータス: 0x{res1[1]:X2})");

            await Task.Delay(1000); // 演出用の待機

            // 2. 「設定要求(0x20)」を出し、「設定完了(0x21)」を待つ
            progress.Report("2/2: パラメータ設定コマンド(0x20)を送信中...");
            var res2 = await _client.SendRequestAsync(new byte[] { 0x20 }, 0x21, timeoutMs: 3000);
            progress.Report($"パラメータ設定完了！ (戻り値: 0x{res2[1]:X2})");

            progress.Report("--- 全てのデバイス設定が正常に完了しました ---");
        }
        catch (TimeoutException ex)
        {
            progress.Report($"[エラー] デバイスとの応答がタイムアウトしました。: {ex.Message}");
            throw; // エラー自体は上位（UI側など）でも扱えるように再スロー
        }
    }
}
