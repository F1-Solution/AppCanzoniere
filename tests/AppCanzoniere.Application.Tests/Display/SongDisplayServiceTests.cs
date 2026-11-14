using AppCanzoniere.Application.Display;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests.Display;

public class SongDisplayServiceTests
{
    private readonly SongDisplayService _service = new(new ChordProParser());

    private SongDisplay Show(string song, int steps = 0, ChordDisplaySettings? settings = null, string? language = "en", string? fallbackKey = null) =>
        _service.Render(new SongDisplayRequest(song, steps, settings ?? ChordDisplaySettings.Default, language, fallbackKey));

    private static string Lines(params string[] lines) => string.Join("\n", lines);

    [Fact]
    public void ChordsAreShownAboveTheSyllableTheyBelongTo()
    {
        var shown = Show("[G]Santo [D]Signore");

        Assert.Equal(Lines("G     D", "Santo Signore"), shown.Text);
    }

    [Fact]
    public void ALineWithoutChords_ShowsNoEmptyChordRow()
    {
        var shown = Show("[G]Santo\nSenza accordi");

        Assert.Equal(Lines("G", "Santo", "Senza accordi"), shown.Text);
    }

    [Fact]
    public void AChordOnlyLine_ShowsOnlyTheChords()
    {
        var shown = Show("[G] [D] [Em]\n[C]Ciao");

        Assert.Equal(Lines("G D Em", "C", "Ciao"), shown.Text);
    }

    [Fact]
    public void Sections_AreSeparatedByOneBlankLine()
    {
        var shown = Show("[G]Uno\n\n{start_of_chorus}\n[C]Due\n{end_of_chorus}");

        Assert.Equal(Lines("G", "Uno", "", "C", "Due"), shown.Text);
    }

    [Fact]
    public void ACommentIsShownAsItsOwnLine()
    {
        var shown = Show("{comment: Intro}\n[G]Uno");

        Assert.Equal(Lines("Intro", "G", "Uno"), shown.Text);
    }

    [Fact]
    public void ATabBlock_IsKeptVerbatim()
    {
        var shown = Show("{start_of_tab}\ne|--0--3--|\nB|--1--0--|\n{end_of_tab}");

        Assert.Equal(Lines("e|--0--3--|", "B|--1--0--|"), shown.Text);
    }

    [Fact]
    public void ALongLine_IsNotWrapped_SoChordsStayAlignedWithTheirWords()
    {
        var lyrics = string.Join(' ', Enumerable.Repeat("parola", 60));

        var shown = Show($"[G]{lyrics} [D]fine");

        var rows = shown.Text.Split('\n');
        Assert.Equal(2, rows.Length);
        Assert.Equal(lyrics.Length + 1, rows[0].IndexOf('D'));
        Assert.Equal(lyrics.Length + 1, rows[1].IndexOf("fine", StringComparison.Ordinal));
    }

    [Fact]
    public void EmptyOrBlankChordPro_ShowsNothingAndDoesNotThrow()
    {
        Assert.Equal(string.Empty, Show(string.Empty).Text);
        Assert.Equal(string.Empty, Show("  \n\n").Text);
    }

    [Fact]
    public void UnicodeAndEmojiLyrics_AreKeptIntact()
    {
        var shown = Show("[G]Alleluia – è vivo \U0001F600 [D]ora");

        Assert.Contains("Alleluia – è vivo \U0001F600 ora", shown.Text);
    }

    [Fact]
    public void TransposeUp_MovesEveryChordTogether()
    {
        var shown = Show("{key:G}\n[G]Santo [Em]Signore", steps: 2);

        Assert.Equal(Lines("A     F#m", "Santo Signore"), shown.Text);
        Assert.Equal("A", shown.Key);
    }

    [Fact]
    public void TransposeByAFullOctaveEitherWay_ShowsTheOriginalText()
    {
        const string song = "{key:G}\n[G]Santo [Em]Signore\n[C]Dio [D7]vivo";
        var original = Show(song);

        Assert.Equal(original, Show(song, steps: 0));
        Assert.Equal(original.Text, Show(song, steps: 12).Text);
        Assert.Equal(original.Text, Show(song, steps: -12).Text);
        Assert.Equal(original.Key, Show(song, steps: 12).Key);
    }

    [Fact]
    public void TheFirstDisplay_ShowsTheStoredChordsUnchanged()
    {
        var shown = Show("{key:D}\n[D]Alleluia [A7]alleluia");

        Assert.Equal(Lines("D         A7", "Alleluia alleluia"), shown.Text);
        Assert.Equal("D", shown.Key);
    }

    [Fact]
    public void Italian_IsTheDefaultForAnItalianLanguage_WithMinorAsLam()
    {
        var shown = Show("{key:G}\n[G]Santo [Em]Signore", steps: 2, language: "it-IT");

        Assert.Equal(Lines("La     Fa#m", "Santo  Signore"), shown.Text);
        Assert.Equal("La", shown.Key);
    }

    [Fact]
    public void English_IsTheDefaultForAnyOtherLanguage()
    {
        Assert.Equal("Am", Show("[Am]x", language: "fr").Text.Split('\n')[0]);
        Assert.Equal("Am", Show("[Am]x", language: null).Text.Split('\n')[0]);
    }

    [Fact]
    public void TheUserChoice_BeatsTheLanguage_BothWays()
    {
        var english = Show("[Lam]x", language: "it", settings: new ChordDisplaySettings(NotationChoice.English));
        var italian = Show("[Am]x", language: "en", settings: new ChordDisplaySettings(NotationChoice.Italian));

        Assert.Equal("Am", english.Text.Split('\n')[0]);
        Assert.Equal("Lam", italian.Text.Split('\n')[0]);
    }

    [Fact]
    public void MinorWrittenWithADash_WhenTheUserAsksForIt()
    {
        var settings = new ChordDisplaySettings(NotationChoice.Italian, MinorStyle.Dash);

        var shown = Show("{key:C}\n[Am]x [Dm7]y", settings: settings);

        Assert.Equal("La- Re-7", shown.Text.Split('\n')[0]);
    }

    [Fact]
    public void ASongStoredInItalianNotation_IsReadAndShownInEnglishWhenAsked()
    {
        var shown = Show("[Sol]Santo [Mim]Signore", settings: new ChordDisplaySettings(NotationChoice.English));

        Assert.Equal(Lines("G     Em", "Santo Signore"), shown.Text);
    }

    [Fact]
    public void ASongStoredInItalianNotation_IsTransposedInItalian()
    {
        var shown = Show("{key:Sol}\n[Sol]Santo [Mim]Signore", steps: 2, language: "it");

        Assert.Equal(Lines("La     Fa#m", "Santo  Signore"), shown.Text);
        Assert.Equal("La", shown.Key);
    }

    [Theory]
    [InlineData(AccidentalPreference.Sharps, "C#")]
    [InlineData(AccidentalPreference.Flats, "Db")]
    public void SharpsOrFlats_DecideTheSpellingAfterTransposing(AccidentalPreference preference, string expected)
    {
        var shown = Show("[C]x", steps: 1, settings: new ChordDisplaySettings(Accidentals: preference));

        Assert.Equal(expected, shown.Text.Split('\n')[0]);
    }

    [Fact]
    public void AutomaticSpelling_FollowsTheKeyReached()
    {
        var shown = Show("{key:C}\n[C]x [F]y", steps: 5);

        Assert.Equal("F Bb", shown.Text.Split('\n')[0]);
    }

    [Fact]
    public void ACapoInTheFile_KeepsTheShapes_AndTheKeyIsTheOneThatSounds()
    {
        var shown = Show("{key:G}\n{capo:2}\n[G]Santo [Em]Signore");

        Assert.Equal(Lines("G     Em", "Santo Signore"), shown.Text);
        Assert.Equal(2, shown.Capo);
        Assert.Equal("A", shown.Key);
    }

    [Fact]
    public void ACapoInTheFile_ShiftsShapesAndSoundingKeyTogetherWhenTransposed()
    {
        var shown = Show("{key:G}\n{capo:2}\n[G]Santo [Em]Signore", steps: 2);

        Assert.Equal(Lines("A     F#m", "Santo Signore"), shown.Text);
        Assert.Equal(2, shown.Capo);
        Assert.Equal("B", shown.Key);
    }

    [Fact]
    public void ACapoInTheFile_WithItalianNotation_WritesTheSoundingKeyInItalian()
    {
        var shown = Show("{key:G}\n{capo:2}\n[G]x", language: "it");

        Assert.Equal(2, shown.Capo);
        Assert.Equal("La", shown.Key);
        Assert.Equal("Sol", shown.Text.Split('\n')[0]);
    }

    [Theory]
    [InlineData("[G]x")]
    [InlineData("{capo:0}\n[G]x")]
    [InlineData("{capo:99}\n[G]x")]
    [InlineData("{capo:abc}\n[G]x")]
    public void WithoutAUsableCapo_TheCapoIsZero(string song)
    {
        Assert.Equal(0, Show(song).Capo);
    }

    [Fact]
    public void WithoutAKeyAnywhere_TheKeyIsEmpty()
    {
        Assert.Equal(string.Empty, Show("[G]x").Key);
    }

    [Fact]
    public void WhenTheFileHasNoKey_TheFallbackKeyIsTransposedAndWrittenInTheUserNotation()
    {
        var shown = Show("[G]x", steps: 2, language: "it", fallbackKey: "G");

        Assert.Equal("La", shown.Key);
        Assert.Equal("La", shown.Text.Split('\n')[0]);
    }

    [Fact]
    public void TheKeyOfTheFile_WinsOverTheFallbackKey()
    {
        Assert.Equal("D", Show("{key:D}\n[D]x", fallbackKey: "G").Key);
    }

    [Fact]
    public void AnUnreadableFallbackKey_IsIgnored()
    {
        Assert.Equal(string.Empty, Show("[G]x", fallbackKey: "not a key").Key);
    }

    [Fact]
    public void ALyricsLineThatLooksLikeADirective_IsStillShown()
    {
        var shown = Show("[G]Canta {forte} ora");

        Assert.Equal(Lines("G", "Canta {forte} ora"), shown.Text);
    }

    [Fact]
    public void TheStoredSong_IsNotChangedByRendering()
    {
        const string song = "{key:G}\n[G]Santo";
        var parser = new ChordProParser();
        var before = parser.Render(parser.Parse(song));

        Show(song, steps: 5);

        Assert.Equal(before, parser.Render(parser.Parse(song)));
    }

    [Fact]
    public void ANullRequest_IsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => _service.Render(null!));
    }

    [Fact]
    public void ANullChordProText_IsRejectedWithAClearError()
    {
        var request = new SongDisplayRequest(null!, 0, ChordDisplaySettings.Default, "en");

        Assert.Throws<ArgumentException>(() => _service.Render(request));
    }

    [Fact]
    public void ANullServiceDependency_IsRejected()
    {
        Assert.Throws<ArgumentNullException>(() => new SongDisplayService(null!));
    }

    [Fact]
    public void ALargeSong_IsRenderedInReasonableTime()
    {
        var song = string.Join("\n", Enumerable.Repeat("[G]Santo [D]Signore [Em]Dio [C]vivo", 10_000));
        var clock = System.Diagnostics.Stopwatch.StartNew();

        var shown = Show(song, steps: 3);

        Assert.True(clock.Elapsed < TimeSpan.FromSeconds(30), clock.Elapsed.ToString());
        Assert.Equal(20_000, shown.Text.Split('\n').Length);
    }
}
