using System;
using AvaloniaDiApp.Models.Data;
using AvaloniaDiApp.Services;

namespace AvaloniaDiApp.Models.Services;

public class SettingsService : ISettingsService
{
    private Settings _settings = new("AvaloniaDiApp", true);

    public Settings CurrentSettings => _settings;

    public void UpdateSettings(Settings settings)
    {
        _settings = settings;
        Console.WriteLine($"Settings updated: {settings.AppName}");
    }
}
