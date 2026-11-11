using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

public class SongRenderingTests
{
    private readonly ChordProParser _parser = new();

    private const string Song =
        "{title:Acceptance Song}\n" +
        "{key:F}\n" +
        "[F]Morning [Dm]light\n" +
        "[Bb]Over the [C7]hill\n" +
        "\n" +
        "{start_of_tab}\n" +
        "e|--1--3--|\n" +
        "Bb|---\n" +
        "{end_of_tab}\n" +
        "[Am]Home [C/E]again";

    private string Render(string content, ChordProRenderOptions options) =>
        _parser.Render(_parser.Parse(content), options);

    // F2 acceptance: +2 semitones, flats, capo 3, then notation by language.
    [Fact]
    public void Acceptance_SongTransposedWithFlatsAndCapo_RendersInSolfegeWithLam()
    {
        var options = new ChordProRenderOptions
        {
            TransposeSteps = 2,
            Accidentals = AccidentalPreference.Flats,
            Capo = 3,
            OutputNotation = NotationOptions.ForLanguage("it")
        };

        var result = Render(Song, options);

        Assert.Equal(
            "{title:Acceptance Song}\n" +
            "{key:Sol}\n" +
            "{capo:3}\n" +
            "\n" +
            "[Mi]Morning [Rebm]light\n" +
            "[La]Over the [Si7]hill\n" +
            "\n" +
            "{start_of_tab}\n" +
            "e|--1--3--|\n" +
            "Bb|---\n" +
            "{end_of_tab}\n" +
            "[Labm]Home [Si/Mib]again\n",
            result);
    }

    [Fact]
    public void Acceptance_SameSongForEnglish_RendersInLettersWithAm()
    {
        var options = new ChordProRenderOptions
        {
            TransposeSteps = 2,
            Accidentals = AccidentalPreference.Flats,
            Capo = 3,
            OutputNotation = NotationOptions.ForLanguage("en-US")
        };

        var result = Render(Song, options);

        Assert.Equal(
            "{title:Acceptance Song}\n" +
            "{key:G}\n" +
            "{capo:3}\n" +
            "\n" +
            "[E]Morning [Dbm]light\n" +
            "[A]Over the [B7]hill\n" +
            "\n" +
            "{start_of_tab}\n" +
            "e|--1--3--|\n" +
            "Bb|---\n" +
            "{end_of_tab}\n" +
            "[Abm]Home [B/Eb]again\n",
            result);
    }

    [Fact]
    public void Acceptance_ItalianDashOverride_WritesMinorsWithAHyphen()
    {
        var options = new ChordProRenderOptions
        {
            TransposeSteps = 2,
            Accidentals = AccidentalPreference.Flats,
            Capo = 3,
            OutputNotation = NotationOptions.ForLanguage("it") with { MinorStyle = MinorStyle.Dash }
        };

        var result = Render(Song, options);

        Assert.Contains("[Mi]Morning [Reb-]light\n", result);
        Assert.Contains("[Lab-]Home [Si/Mib]again\n", result);
    }

    [Fact]
    public void Render_TabLinesAreNeverConverted_EvenWhenTheyLookLikeChords()
    {
        var content = "{sot}\nE|---\nA|---\nDo|-3-\nLam\n{eot}";
        var options = new ChordProRenderOptions
        {
            TransposeSteps = 5,
            OutputNotation = NotationOptions.Italian,
            Capo = 2
        };

        var result = Render(content, options);

        Assert.Equal("\n{start_of_tab}\nE|---\nA|---\nDo|-3-\nLam\n{end_of_tab}\n", result);
    }

    [Fact]
    public void Render_ItalianInput_IsConvertedToEnglish()
    {
        var content = "{key:Lam}\n[Lam]Casa [Sol/Si]mia [N.C.]ferma";
        var options = new ChordProRenderOptions { InputNotation = NotationOptions.Italian };

        var result = Render(content, options);

        Assert.Equal("{key:Am}\n\n[Am]Casa [G/B]mia [N.C.]ferma\n", result);
    }

    [Fact]
    public void Render_LyricsAndHyphens_AreNeverTouched()
    {
        var options = new ChordProRenderOptions { InputNotation = NotationOptions.Italian };

        var result = Render("[La-]la-la-la, Re- Mi- [Do]fa", options);

        Assert.Equal("\n[Am]la-la-la, Re- Mi- [C]fa\n", result);
    }

    [Fact]
    public void Render_NoOptions_KeepsTheSpellingAsWritten()
    {
        var result = Render("{key:Db}\n[Bb]Line [C#m]two", new ChordProRenderOptions());

        Assert.Equal("{key:Db}\n\n[Bb]Line [C#m]two\n", result);
    }

    [Fact]
    public void Render_ExplicitFlatsWithoutShift_RespellsTheSong()
    {
        var options = new ChordProRenderOptions { Accidentals = AccidentalPreference.Flats };

        var result = Render("{key:C#}\n[A#]Line [D#m]two", options);

        Assert.Equal("{key:Db}\n\n[Bb]Line [Ebm]two\n", result);
    }

    [Fact]
    public void Render_AutoSpelling_FollowsTheTargetKey()
    {
        var options = new ChordProRenderOptions { TransposeSteps = 5, Accidentals = AccidentalPreference.Auto };

        var result = Render("{key:C}\n[C]Line [F]two [G7]three", options);

        Assert.Equal("{key:F}\n\n[F]Line [Bb]two [C7]three\n", result);
    }

    [Fact]
    public void Render_DefaultShift_UsesSharps()
    {
        var result = Render("[C]Line [F]two", new ChordProRenderOptions { TransposeSteps = 1 });

        Assert.Equal("\n[C#]Line [F#]two\n", result);
    }

    [Fact]
    public void Render_CapoOverride_ReplacesTheDocumentCapo()
    {
        // Shapes written for capo 2: G sounds as A.
        var doc = "{key:A}\n{capo:2}\n[G]Line";

        Assert.Equal("{key:A}\n\n[A]Line\n", Render(doc, new ChordProRenderOptions { Capo = 0 }));
        Assert.Equal("{key:A}\n{capo:4}\n\n[F]Line\n", Render(doc, new ChordProRenderOptions { Capo = 4 }));
    }

    [Fact]
    public void Render_NoCapoOption_KeepsTheDocumentCapoAndChords()
    {
        var result = Render("{capo:2}\n[G]Line", new ChordProRenderOptions());

        Assert.Equal("{capo:2}\n\n[G]Line\n", result);
    }

    [Fact]
    public void Render_CapoWithTransposition_CombinesBoth()
    {
        var options = new ChordProRenderOptions { TransposeSteps = 2, Capo = 2 };

        Assert.Equal("{key:D}\n{capo:2}\n\n[C]Line\n", Render("{key:C}\n[C]Line", options));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(12)]
    public void Render_CapoOutOfRange_IsRejected(int capo)
    {
        var document = _parser.Parse("[C]Line");

        Assert.Throws<ArgumentOutOfRangeException>(() => _parser.Render(document, new ChordProRenderOptions { Capo = capo }));
    }

    [Fact]
    public void Render_DoesNotModifyTheOptionsOrTheDocument()
    {
        var document = _parser.Parse("{key:C}\n[C]Line");
        var options = new ChordProRenderOptions { TransposeSteps = 2, Capo = 1, OutputNotation = NotationOptions.Italian };

        _parser.Render(document, options);

        Assert.Equal("C", document.Key);
        Assert.Equal("C", document.Lines[0].Chords[0].Chord);
        Assert.Equal(2, options.TransposeSteps);
    }

    [Fact]
    public void Render_UnrecognisedKey_IsLeftAsWritten()
    {
        var result = Render("{key:Dorian}\n[C]Line", new ChordProRenderOptions { TransposeSteps = 2 });

        Assert.Equal("{key:Dorian}\n\n[D]Line\n", result);
    }
}
