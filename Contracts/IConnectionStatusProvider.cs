using System;
using AvaloniaDiApp.Models.Data;

namespace AvaloniaDiApp.Contracts;

public interface IConnectionStatusProvider
{
    ConnectionStatus CurrentStatus { get; }
    event Action<ConnectionStatus>? StatusUpdated;
    void RequestUpdate();
}
