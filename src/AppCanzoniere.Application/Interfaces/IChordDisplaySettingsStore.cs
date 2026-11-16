using AppCanzoniere.Application.Display;

namespace AppCanzoniere.Application.Interfaces;

/// <summary>Device-local storage of the chord display settings.</summary>
public interface IChordDisplaySettingsStore
{
    ChordDisplaySettings Load();

    void Save(ChordDisplaySettings settings);
}

/// <summary>Minimal string key-value storage on the device.</summary>
public interface IPreferenceStorage
{
    string? Get(string key);

    void Set(string key, string value);
}
