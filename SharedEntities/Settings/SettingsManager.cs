using System.Text.Json;

namespace SharedEntities.Settings;

public class SettingsManager
{
    private static readonly object _lock = new();

    private static readonly string _defaultSettingsLocationPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "VKDrive",
        "Settings.json");

    // CA1869
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    /// <summary>
    /// Save settings to the file in a thread-safe manner.
    /// </summary>
    public static void SaveSettings(Settings settings)
    {
        lock (_lock)
        {
            WriteSettingsToFile(settings);
        }
    }

    /// <summary>
    /// Load settings from the file in a thread-safe manner.
    /// </summary>
    public static Settings LoadSettings()
    {
        lock (_lock)
        {
            var settingsDirectory = Path.GetDirectoryName(_defaultSettingsLocationPath);

            if (!Directory.Exists(settingsDirectory))
            {
                Directory.CreateDirectory(settingsDirectory);
            }

            if (File.Exists(_defaultSettingsLocationPath))
            {
                var json = File.ReadAllText(_defaultSettingsLocationPath);
                return JsonSerializer.Deserialize<Settings>(json);
            }
            else
            {
                var settings = new Settings();
                WriteSettingsToFile(settings);

                return settings;
            }
        }
    }

    private static void WriteSettingsToFile(Settings settings)
    {
        var defaultJson = JsonSerializer.Serialize(settings, _jsonOptions);
        File.WriteAllText(_defaultSettingsLocationPath, defaultJson);
    }
}
