using AppCanzoniere.Application.Interfaces;

namespace AppCanzoniere.Application.Display;

/// <summary>Keeps the chord display settings in device preferences; unreadable values fall back to the defaults.</summary>
public sealed class ChordDisplaySettingsStore : IChordDisplaySettingsStore
{
    public ChordDisplaySettingsStore(IPreferenceStorage storage) => throw new NotImplementedException();

    public ChordDisplaySettings Load() => throw new NotImplementedException();

    public void Save(ChordDisplaySettings settings) => throw new NotImplementedException();
}
