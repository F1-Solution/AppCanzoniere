using AppCanzoniere.Application.Display;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests.Display;

public class SongScreenPresenterTests
{
    private sealed class FailingDisplay : ISongDisplayService
    {
        public SongDisplay Render(SongDisplayRequest request) => throw new InvalidOperationException("layout failed");
    }

    private sealed class RecordingDisplay : ISongDisplayService
    {
        public SongDisplayRequest? Last { get; private set; }

        public SongDisplay Render(SongDisplayRequest request)
        {
            Last = request;
            return new SongDisplay("text", "G", 0);
        }
    }

    private readonly SongScreenPresenter _presenter = new(new SongDisplayService(new ChordProParser()));

    private SongScreen Present(string? chordPro, string? other = null, int steps = 0, string? app = null, string? system = "en", ChordDisplaySettings? settings = null) =>
        _presenter.Present(new SongScreenRequest(chordPro, other, steps, settings ?? ChordDisplaySettings.Default, app, system));

    [Fact]
    public void ASongWithoutKey_ShowsNoKey_EvenWhenChordsMove()
    {
        var screen = Present("[E]Uno [G#m]due [C#m]tre [B]quattro", steps: -1);

        Assert.Equal(string.Empty, screen.Key);
        Assert.StartsWith("Eb", screen.Text);
    }

    [Fact]
    public void ASongWithAKey_ShowsTheSoundingKey()
    {
        Assert.Equal("A", Present("{key:G}\n[G]x", steps: 2).Key);
    }

    [Theory]
    [InlineData(2, "Capo 2")]
    [InlineData(0, "")]
    public void TheCapoText_NamesTheFretOnlyWhenThereIsOne(int fret, string expected)
    {
        Assert.Equal(expected, Present($"{{capo:{fret}}}\n[G]x").Capo);
    }

    [Fact]
    public void AFailingLayout_ShowsTheStoredSong_WithAnError()
    {
        var presenter = new SongScreenPresenter(new FailingDisplay());

        var screen = presenter.Present(new SongScreenRequest("[G]x", null, 0, ChordDisplaySettings.Default, null, "en"));

        Assert.Equal("[G]x", screen.Text);
        Assert.Equal(string.Empty, screen.Key);
        Assert.Equal(string.Empty, screen.Capo);
        Assert.NotEqual(string.Empty, screen.Error);
    }

    [Fact]
    public void ARenderedSong_HasNoError()
    {
        Assert.Equal(string.Empty, Present("[G]x").Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void WithoutChordPro_TheOtherContentIsShown(string? chordPro)
    {
        var screen = Present(chordPro, other: "<musicxml/>");

        Assert.Equal("<musicxml/>", screen.Text);
        Assert.Equal(string.Empty, screen.Key);
        Assert.Equal(string.Empty, screen.Error);
    }

    [Fact]
    public void WithoutAnyContent_APlaceholderIsShown()
    {
        Assert.Equal("No content available", Present(null).Text);
    }

    [Theory]
    [InlineData("it", "en", "it")]
    [InlineData("en", "it", "en")]
    [InlineData(null, "it", "it")]
    [InlineData("", "it", "it")]
    [InlineData("  ", "it", "it")]
    [InlineData(null, null, null)]
    public void TheAppLanguageWins_ThenTheDeviceLanguage(string? app, string? system, string? expected)
    {
        var display = new RecordingDisplay();

        new SongScreenPresenter(display).Present(new SongScreenRequest("[G]x", null, 0, ChordDisplaySettings.Default, app, system));

        Assert.Equal(expected, display.Last!.LanguageTag);
    }

    [Fact]
    public void TheAppLanguage_DecidesTheNotation_NotTheDevice()
    {
        Assert.StartsWith("Sol", Present("[G]x", app: "it", system: "en").Text);
        Assert.StartsWith("G", Present("[G]x", app: "en", system: "it").Text);
    }

    [Fact]
    public void ANullRequest_IsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => _presenter.Present(null!));
    }

    [Fact]
    public void ANullDisplayService_IsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new SongScreenPresenter(null!));
    }
}

public class ChordDisplayPickersTests
{
    [Fact]
    public void AnyNegativeIndex_IsNotAChoice()
    {
        Assert.False(ChordDisplayPickers.TryToSettings(-1, 0, 0, out _));
        Assert.False(ChordDisplayPickers.TryToSettings(0, -1, 0, out _));
        Assert.False(ChordDisplayPickers.TryToSettings(0, 0, -1, out _));
    }

    [Fact]
    public void AnIndexPastTheEnd_IsNotAChoice()
    {
        Assert.False(ChordDisplayPickers.TryToSettings(3, 0, 0, out _));
        Assert.False(ChordDisplayPickers.TryToSettings(0, 2, 0, out _));
        Assert.False(ChordDisplayPickers.TryToSettings(0, 0, 3, out _));
    }

    [Fact]
    public void TheIndexes_MapToTheSettings()
    {
        Assert.True(ChordDisplayPickers.TryToSettings(2, 1, 2, out var settings));

        Assert.Equal(new ChordDisplaySettings(NotationChoice.Italian, MinorStyle.Dash, AccidentalPreference.Flats), settings);
    }

    [Fact]
    public void TheDefaults_AreTheFirstEntryOfEachPicker()
    {
        Assert.Equal((0, 0, 0), ChordDisplayPickers.ToIndexes(ChordDisplaySettings.Default));
    }

    [Theory]
    [InlineData(NotationChoice.English, MinorStyle.Dash, AccidentalPreference.Sharps)]
    [InlineData(NotationChoice.Italian, MinorStyle.LetterM, AccidentalPreference.Flats)]
    public void SettingsAndIndexes_RoundTrip(NotationChoice notation, MinorStyle minor, AccidentalPreference accidentals)
    {
        var settings = new ChordDisplaySettings(notation, minor, accidentals);
        var (n, m, a) = ChordDisplayPickers.ToIndexes(settings);

        Assert.True(ChordDisplayPickers.TryToSettings(n, m, a, out var back));
        Assert.Equal(settings, back);
    }
}
