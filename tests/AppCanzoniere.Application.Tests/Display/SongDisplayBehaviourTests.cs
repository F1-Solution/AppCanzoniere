using AppCanzoniere.Application.Display;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests.Display;

// Key naming, spelling, notation, choruses, tabs and column collisions on the song screen.
public class SongDisplayBehaviourTests
{
    private readonly SongDisplayService _service = new(new ChordProParser());

    private SongDisplay Show(string song, int steps = 0, ChordDisplaySettings? settings = null, string? language = "en") =>
        _service.Render(new SongDisplayRequest(song, steps, settings ?? ChordDisplaySettings.Default, language));

    private static string Lines(params string[] lines) => string.Join("\n", lines);

    private static ChordDisplaySettings Pref(AccidentalPreference preference) => new(Accidentals: preference);

    private static ChordDisplaySettings Italian(MinorStyle style = MinorStyle.LetterM) => new(NotationChoice.Italian, style);

    private static ChordDisplaySettings English(MinorStyle style = MinorStyle.LetterM) => new(NotationChoice.English, style);

    private string FirstRow(string song, int steps = 0, ChordDisplaySettings? settings = null) =>
        Show(song, steps, settings).Text.Split('\n')[0];

    // DEC 16 and the key without a {key}.

    [Theory]
    [InlineData(AccidentalPreference.Flats)]
    [InlineData(AccidentalPreference.Sharps)]
    [InlineData(AccidentalPreference.Auto)]
    public void WithNoTransposition_StoredChordsAreShownAsWritten_WhateverThePreference(AccidentalPreference preference)
    {
        var shown = Show("{key:E}\n[E]Uno [G#m]due [C#m]tre [B]quattro", settings: Pref(preference));

        Assert.Equal(new[] { "E", "G#m", "C#m", "B" }, Chords(shown));
        Assert.Equal("E", shown.Key);
    }

    [Fact]
    public void WithNoTransposition_ASharpPreferenceDoesNotRespellFlatChords()
    {
        var shown = Show("{key:F}\n[F]Uno [Bb]due [C]tre [Dm]ora", settings: Pref(AccidentalPreference.Sharps));

        Assert.Equal(new[] { "F", "Bb", "C", "Dm" }, Chords(shown));
    }

    private static string[] Chords(SongDisplay shown) =>
        shown.Text.Split('\n')[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);

    [Fact]
    public void AnUnambiguousKeyReached_IsSpelledByItsSignature_EvenWithTheOppositePreference()
    {
        var shown = Show("{key:G}\n[G]a [B7]b [Em]c", steps: 7, settings: Pref(AccidentalPreference.Flats));

        Assert.Equal(new[] { "D", "F#7", "Bm" }, shown.Text.Split('\n')[0].Split(' ', StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal("D", shown.Key);
    }

    [Theory]
    [InlineData(AccidentalPreference.Flats, "Db", "Bbm", "Gb", "Ab")]
    [InlineData(AccidentalPreference.Sharps, "C#", "A#m", "F#", "G#")]
    [InlineData(AccidentalPreference.Auto, "Db", "Bbm", "Gb", "Ab")]
    public void AnAmbiguousKeyReached_IsNamedAndSpelledByThePreference(AccidentalPreference preference, params string[] expected)
    {
        var shown = Show("{key:C}\n[C]x [Am]y [F]z [G]w", steps: 1, settings: Pref(preference));

        Assert.Equal(expected, shown.Text.Split('\n')[0].Split(' ', StringSplitOptions.RemoveEmptyEntries));
        Assert.Equal(expected[0], shown.Key);
    }

    [Theory]
    [InlineData(AccidentalPreference.Sharps, "Db")]
    [InlineData(AccidentalPreference.Flats, "Db")]
    [InlineData(AccidentalPreference.Auto, "Db")]
    public void WithoutTranspositionOrCapo_TheKeyIsShownAsWritten_LikeTheChords(AccidentalPreference preference, string expectedKey)
    {
        var shown = Show("{key:Db}\n[Db]x [Gb]y [Ab]z", settings: Pref(preference));

        Assert.Equal(expectedKey, shown.Key);
        Assert.Equal(new[] { "Db", "Gb", "Ab" }, shown.Text.Split('\n')[0].Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    [Fact]
    public void WithoutTranspositionOrCapo_AnItalianKeyIsOnlyRenamed_NotRespelled()
    {
        var shown = Show("{key:Reb}\n[Reb]x [Solb]y [Lab]z", settings: Pref(AccidentalPreference.Sharps), language: "it");

        Assert.Equal("Reb", shown.Key);
        Assert.Equal(new[] { "Reb", "Solb", "Lab" }, shown.Text.Split('\n')[0].Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    [Fact]
    public void WithoutTranspositionOrCapo_AKeyInTheOtherNotationIsOnlyRenamed_NotRespelled()
    {
        var shown = Show("{key:Db}\n[Db]x [Gb]y", settings: Pref(AccidentalPreference.Sharps), language: "it");

        Assert.Equal("Reb", shown.Key);
    }

    [Fact]
    public void WithACapo_ThePreferenceNamesTheSoundingKey_AndTheShapesKeepTheirOwnSpelling()
    {
        var shown = Show("{key:G}\n{capo:2}\n[G]x [Em]y", steps: 0, settings: Pref(AccidentalPreference.Flats));

        Assert.Equal("A", shown.Key);
        Assert.Equal(new[] { "G", "Em" }, shown.Text.Split('\n')[0].Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    [Fact]
    public void WithoutAKey_TheChordsAreSpelledByTheFirstChord_AsTheCoreDoes()
    {
        var shown = Show("[E]Uno [G#m]due [C#m]tre [B]quattro", steps: -1);

        Assert.Equal(string.Empty, shown.Key);
        Assert.Equal(new[] { "Eb", "Gm", "Cm", "Bb" }, shown.Text.Split('\n')[0].Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }

    // Chords of the other notation move with the rest (item 6).

    [Fact]
    public void ChordsOfTheOtherNotation_AreTransposedToo_AndWrittenInEnglish()
    {
        var row = FirstRow("[Do] [Sol] [Lam] [Fa] [C] [G]", steps: 2, settings: English());

        Assert.Equal("D A Bm G D A", row);
    }

    [Fact]
    public void ChordsOfTheOtherNotation_AreTransposedToo_AndWrittenInItalian()
    {
        var row = FirstRow("[Do] [Sol] [Lam] [Fa] [C] [G]", steps: 2, settings: Italian());

        Assert.Equal("Re La Sim Sol Re La", row);
    }

    [Theory]
    [InlineData(NotationChoice.Italian, "Mi Si Fa# Si")]
    [InlineData(NotationChoice.English, "E B F# B")]
    public void ATieBetweenTheNotations_TransposesEveryChord(NotationChoice choice, string expected)
    {
        Assert.Equal(expected, FirstRow("[Re] [La] [E] [A]", steps: 2, settings: new ChordDisplaySettings(choice)));
    }

    [Fact]
    public void OnATie_TheFirstChordThatOnlyOneNotationReadsDecides_ItalianFirst()
    {
        // Italian input written as Italian keeps the typed minor marker; other chords are converted.
        Assert.Equal("Remin Mi", FirstRow("[Remin] [E]", settings: Italian()));
    }

    [Fact]
    public void OnATie_TheFirstChordThatOnlyOneNotationReadsDecides_EnglishFirst()
    {
        // English input written as Italian is converted whole, so the marker is rewritten.
        Assert.Equal("Mi Rem", FirstRow("[E] [Remin]", settings: Italian()));
    }

    [Fact]
    public void TheNotationWithMoreChordsDecides_English()
    {
        Assert.Equal("Amin Dmin E", FirstRow("[Amin] [Dmin] [Mi]", settings: English()));
    }

    [Fact]
    public void TheNotationWithMoreChordsDecides_Italian()
    {
        Assert.Equal("Lamin Remin Mi", FirstRow("[Lamin] [Remin] [E]", settings: Italian()));
    }

    [Fact]
    public void ASongWithNoChordAtAll_ShowsItsLyrics()
    {
        Assert.Equal("Solo testo", Show("Solo testo").Text);
    }

    // The minor style follows the choice, whatever was typed (item 9).

    [Fact]
    public void ADashMinorInTheFile_IsWrittenAsLam_WhenTheUserChoseLam()
    {
        Assert.Equal("Lam Re Mi", FirstRow("[La-] [Re] [Mi]", settings: Italian()));
    }

    [Fact]
    public void AnMMinorInTheFile_IsWrittenWithADash_WhenTheUserChoseTheDash()
    {
        Assert.Equal("La- Re", FirstRow("[Lam] [Re]", settings: Italian(MinorStyle.Dash)));
    }

    [Fact]
    public void AMixedFile_IsWrittenInOneMinorStyle()
    {
        Assert.Equal("Am Dm", FirstRow("[Am] [D-]", settings: English()));
        Assert.Equal("A- D-", FirstRow("[Am] [D-]", settings: English(MinorStyle.Dash)));
    }

    [Fact]
    public void AMinorMarkerTheUserDidNotContradict_IsKeptAsTyped()
    {
        Assert.Equal("Amin D", FirstRow("[Amin] [D]", settings: English()));
    }

    // A tab inside a lyric counts as the columns it covers (item 9).

    [Fact]
    public void ATabInALyric_ExpandsToTheNextTabStop_SoChordsStayAligned()
    {
        var rows = Show("[G]Santo\t[D]Signore").Text.Split('\n');

        Assert.Equal("Santo   Signore", rows[1]);
        Assert.Equal(8, rows[0].IndexOf('D'));
    }

    [Fact]
    public void ATabAfterAChord_DoesNotCountTheChordLetters()
    {
        var rows = Show("a[Gmaj7]\tb").Text.Split('\n');

        Assert.Equal("a   b", rows[1]);
    }

    // Choruses are marked (item 7).

    [Fact]
    public void ALabelledChorus_ShowsItsLabel()
    {
        var shown = Show("[G]Uno\n\n{start_of_chorus: Rit.}\n[C]Due\n{end_of_chorus}");

        Assert.Equal(Lines("G", "Uno", "", "Rit.", "C", "Due"), shown.Text);
    }

    [Fact]
    public void TheShortDirectiveAndTheQuotedLabelAreRead()
    {
        Assert.Equal(Lines("Ritornello", "C", "Due"), Show("{soc: label=\"Ritornello\"}\n[C]Due\n{eoc}").Text);
    }

    [Theory]
    [InlineData("en", "Chorus")]
    [InlineData("it", "Rit.")]
    [InlineData("it-IT", "Rit.")]
    public void AnUnlabelledChorus_IsMarkedWithTheWordOfTheLanguage(string language, string word)
    {
        var shown = Show("{soc}\n[C]Due\n{eoc}", language: language);

        Assert.Equal(Lines(word, language.StartsWith("it") ? "Do" : "C", "Due"), shown.Text);
    }

    [Fact]
    public void AChorusThatAlreadyHasACommentLabel_IsNotMarkedTwice()
    {
        Assert.Equal(Lines("Rit.", "G", "due"), Show("{comment: Rit.}\n{soc}\n[G]due\n{eoc}").Text);
    }

    [Theory]
    [InlineData("{chorus}", "Chorus")]
    [InlineData("{chorus: Rit.}", "Rit.")]
    public void AChorusRecall_IsShownAsALine(string directive, string expected)
    {
        var shown = Show($"[G]Uno\n{directive}\n[G]due");

        Assert.Equal(Lines("G", "Uno", "", expected, "G", "due"), shown.Text);
    }

    [Fact]
    public void AChorusDirectiveInsideATabBlock_IsLeftAlone()
    {
        var shown = Show("{start_of_tab}\n{soc}\ne|--0--|\n{end_of_tab}");

        Assert.Contains("{soc}", shown.Text);
        Assert.DoesNotContain("Chorus", shown.Text);
    }

    // Chords that would overlap are pushed apart (mutants of the projector).

    [Fact]
    public void TwoChordsAtTheSamePlace_AreSeparatedByOneSpace()
    {
        var rows = Show("[Gmaj7][D]Santo").Text.Split('\n');

        Assert.Equal("Gmaj7 D", rows[0]);

    }

    [Fact]
    public void ALongChordOverAShortWord_PushesTheNextChordRightOfItself()
    {
        var rows = Show("[Gmaj7]Si[D]gnore").Text.Split('\n');

        Assert.Equal("Gmaj7 D", rows[0]);
        Assert.Equal("Si    gnore", rows[1]);
    }

    [Fact]
    public void AChordEndingExactlyWhereTheNextStarts_StillGetsAGap()
    {
        var rows = Show("[Am]Ci[D]ao").Text.Split('\n');

        Assert.Equal("Am D", rows[0]);
        Assert.Equal("Ci ao", rows[1]);
    }

    [Fact]
    public void ARenderingDoesNotRemember_ThePreviousTransposition()
    {
        const string song = "{key:G}\n[G]Santo";
        var fresh = Show(song).Text;

        Show(song, steps: 5);

        Assert.Equal(fresh, Show(song).Text);
        Assert.Equal(Lines("G", "Santo"), fresh);
    }
}
