using AvaloniaDiApp.Models.Data;

namespace AvaloniaDiApp.Contracts;

public interface ISettingsService
{
    Settings CurrentSettings { get; }
    void UpdateSettings(Settings settings);
}
