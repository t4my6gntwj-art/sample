using System;
using AvaloniaDiApp.Models.Data;

namespace AvaloniaDiApp.Services;

public interface IPowerStatusService
{
    PowerStatus CurrentStatus { get; }
    event Action<PowerStatus>? StatusUpdated;
    void RequestUpdate();
}
