using AvaloniaDiApp.Models.Data;

namespace AvaloniaDiApp.Services;

public interface ISettingsService
{
    Settings CurrentSettings { get; }
    void UpdateSettings(Settings settings);
}
