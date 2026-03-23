using System;
using AvaloniaDiApp.Models.Data;

namespace AvaloniaDiApp.Contracts;

/// <summary>
/// 電源状態を提供（読み取り）するためのインターフェース。
/// View や ViewModel は、このインターフェースを通じて現在の電源情報を取得します。
/// </summary>
public interface IPowerStatusProvider
{
    /// <summary>
    /// 現在の電源情報を取得します。
    /// </summary>
    PowerStatus CurrentStatus { get; }

    /// <summary>
    /// 電源情報が更新された際に発生するイベント。
    /// </summary>
    event Action<PowerStatus>? StatusUpdated;

    /// <summary>
    /// 現在の状態を強制的に再通知（StatusUpdatedを発火）させます。
    /// </summary>
    void RequestUpdate();
}
