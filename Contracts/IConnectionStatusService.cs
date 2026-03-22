using System;
using AvaloniaDiApp.Models.Data;

namespace AvaloniaDiApp.Contracts;

public interface IConnectionStatusService
{
    ConnectionStatus CurrentStatus { get; }
    event Action<ConnectionStatus>? StatusUpdated;
    void RequestUpdate();
}
