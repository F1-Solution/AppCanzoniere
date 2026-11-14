using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Display;

/// <summary>Keeps the chord display settings in device preferences; unreadable values fall back to the defaults.</summary>
public sealed class ChordDisplaySettingsStore : IChordDisplaySettingsStore
{
    private const string NotationKey = "chord.notation";
    private const string MinorKey = "chord.minor";
    private const string AccidentalsKey = "chord.accidentals";

    private readonly IPreferenceStorage _storage;

    public ChordDisplaySettingsStore(IPreferenceStorage storage)
    {
        ArgumentNullException.ThrowIfNull(storage);
        _storage = storage;
    }

    public ChordDisplaySettings Load()
    {
        var defaults = ChordDisplaySettings.Default;
        return new ChordDisplaySettings(
            Read(NotationKey, defaults.Notation),
            Read(MinorKey, defaults.MinorStyle),
            Read(AccidentalsKey, defaults.Accidentals));
    }

    public void Save(ChordDisplaySettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        _storage.Set(NotationKey, settings.Notation.ToString());
        _storage.Set(MinorKey, settings.MinorStyle.ToString());
        _storage.Set(AccidentalsKey, settings.Accidentals.ToString());
    }

    // Enum.IsDefined rejects numbers that parse but name no member.
    private T Read<T>(string key, T fallback) where T : struct, Enum =>
        Enum.TryParse<T>(_storage.Get(key), out var value) && Enum.IsDefined(value) ? value : fallback;
}
