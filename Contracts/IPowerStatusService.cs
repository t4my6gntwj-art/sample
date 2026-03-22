using System;
using AvaloniaDiApp.Models.Data;

namespace AvaloniaDiApp.Contracts;

public interface IPowerStatusService
{
    PowerStatus CurrentStatus { get; }
    event Action<PowerStatus>? StatusUpdated;
    void RequestUpdate();
}
