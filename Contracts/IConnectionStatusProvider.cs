using System;
using AvaloniaDiApp.Models.Data;

namespace AvaloniaDiApp.Contracts;

/// <summary>
/// 接続状態を提供（読み取り）するためのインターフェース。
/// </summary>
public interface IConnectionStatusProvider
{
    ConnectionStatus CurrentStatus { get; }
    event Action<ConnectionStatus>? StatusUpdated;
    void RequestUpdate();
}
