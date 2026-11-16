using AppCanzoniere.Application.Display;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Tests.Display;

public class ChordDisplaySettingsTests
{
    private sealed class MemoryStorage : IPreferenceStorage
    {
        public Dictionary<string, string> Values { get; } = new();

        public string? Get(string key) => Values.TryGetValue(key, out var value) ? value : null;

        public void Set(string key, string value) => Values[key] = value;
    }

    [Theory]
    [InlineData("it", Notation.Italian)]
    [InlineData("it-IT", Notation.Italian)]
    [InlineData("en-US", Notation.English)]
    [InlineData("de", Notation.English)]
    [InlineData(null, Notation.English)]
    [InlineData("", Notation.English)]
    public void Default_FollowsTheLanguage(string? language, Notation expected)
    {
        Assert.Equal(expected, ChordDisplaySettings.Default.ToNotationOptions(language).Notation);
    }

    [Theory]
    [InlineData(NotationChoice.English, "it", Notation.English)]
    [InlineData(NotationChoice.Italian, "en", Notation.Italian)]
    [InlineData(NotationChoice.Italian, null, Notation.Italian)]
    public void AnExplicitChoice_BeatsTheLanguage(NotationChoice choice, string? language, Notation expected)
    {
        var settings = new ChordDisplaySettings(choice);

        Assert.Equal(expected, settings.ToNotationOptions(language).Notation);
    }

    [Theory]
    [InlineData(MinorStyle.LetterM)]
    [InlineData(MinorStyle.Dash)]
    public void TheMinorStyle_IsCarriedIntoTheNotationOptions(MinorStyle style)
    {
        var options = new ChordDisplaySettings(NotationChoice.Italian, style).ToNotationOptions("en");

        Assert.Equal(new NotationOptions(Notation.Italian, style), options);
    }

    [Fact]
    public void NothingStored_LoadsTheDefaults()
    {
        var store = new ChordDisplaySettingsStore(new MemoryStorage());

        Assert.Equal(ChordDisplaySettings.Default, store.Load());
    }

    [Fact]
    public void SavedSettings_AreLoadedBackByANewStoreOnTheSameStorage()
    {
        var storage = new MemoryStorage();
        var chosen = new ChordDisplaySettings(NotationChoice.Italian, MinorStyle.Dash, AccidentalPreference.Flats);

        new ChordDisplaySettingsStore(storage).Save(chosen);

        Assert.Equal(chosen, new ChordDisplaySettingsStore(storage).Load());
    }

    [Theory]
    [InlineData("garbage", "", "42")]
    [InlineData("99", "Dash", "Flats")]
    public void UnreadableStoredValues_FallBackPerSettingToTheDefault(string notation, string minor, string accidentals)
    {
        var storage = new MemoryStorage();
        storage.Values["chord.notation"] = notation;
        storage.Values["chord.minor"] = minor;
        storage.Values["chord.accidentals"] = accidentals;

        var loaded = new ChordDisplaySettingsStore(storage).Load();

        Assert.Equal(NotationChoice.FollowLanguage, loaded.Notation);
        Assert.Equal(minor == "Dash" ? MinorStyle.Dash : MinorStyle.LetterM, loaded.MinorStyle);
        Assert.Equal(accidentals == "Flats" ? AccidentalPreference.Flats : AccidentalPreference.Auto, loaded.Accidentals);
    }

    [Fact]
    public void ANullStorage_IsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new ChordDisplaySettingsStore(null!));
    }

    [Fact]
    public void ANullSettingsValue_IsRejectedOnSave()
    {
        var store = new ChordDisplaySettingsStore(new MemoryStorage());

        Assert.Throws<ArgumentNullException>(() => store.Save(null!));
    }
}
