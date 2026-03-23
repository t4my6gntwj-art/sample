using System;
using System.Threading.Tasks;

namespace AvaloniaDiApp.Contracts;

/// <summary>
/// デバイスの初期化や設定変更など、一連の複雑な手順（シナリオ）を管理するビジネスロジック。
/// </summary>
public interface IDeviceSetupService
{
    /// <summary>
    /// デバイスの設定シーケンスを実行します。
    /// </summary>
    /// <param name="progress">進捗状況を報告するためのオブジェクト（UIへの通知などに使用）</param>
    Task RunSetupAsync(IProgress<string> progress);
}
