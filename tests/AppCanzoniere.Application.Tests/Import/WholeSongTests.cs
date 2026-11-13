using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;
using static AppCanzoniere.Application.Tests.Import.Sheet;

namespace AppCanzoniere.Application.Tests.Import;

// Spec 9.2 whole songs (invented lyrics, exact output) and the 9.3 acceptance round trip.
public class WholeSongTests
{
    public static readonly string[] Song1 =
    {
        "Resta con noi", "", "Do        Sol  Lam", "Resta con noi, Signore,", "Fa      Sol   Do", "la sera ormai discende.", "",
        "Rit.", "Fa      Do", "Luce tu sei,", "Rem      Sol", "pace del cuore.", "", "2. Resta con noi, Signore,",
        "la notte non ci vinca.", "", "Rit."
    };

    public static readonly string[] Song2 =
    {
        "Title: Morning Light", "Key: G", "Capo 2", "", "Verse 1", "G             C        G", "Morning light upon the hill,",
        "Em      D", "all the world is calm and still.", "", "Chorus", "C        G          D", "Sing, my soul, sing out,",
        "C        D       G", "sing the morning in.", "", "Verse 2", "Every bird and every tree",
        "wakes to praise in harmony.", "", "Chorus"
    };

    public static readonly string[] Song3 =
    {
        "Vieni, Spirito", "Tonalità: Re", "Capotasto: II", "", "Intro: Re - La - Si- - Sol", "", "Re     La",
        "Vieni, Spirito di luce,", "Si-    Sol   La", "scendi su di noi.", "", "RIT:", "Sol     La       Re",
        "Soffia, vento di Dio,", "Si-        Mi-      La", "rinnova la terra."
    };

    public static readonly string[] Song4 =
    {
        "Canto di lode", "", "| Do  | Fa  | Sol | Do  |  x2", "", "Do        Fa", "Lodate il Signore,", "Sol           Do",
        "popoli tutti, lodate.", "N.C.            Sol", "Grande è il suo amore,", "Fa     Sol    Do", "eterna la sua fedeltà."
    };

    public static readonly string[] Song5 =
    {
        "          SALMO 22", "", "     Re           Sol    Re", "     Il Signore è il mio pastore,",
        "     Si-         Mi-", "     non manco di nulla."
    };

    public static readonly string[] Song6 =
    {
        "Quiet Waters", "", "Verse 1", "D              A", "Lead me to the water,", "Bm           G", "quiet and so clear.", "",
        "Bridge", "G        A    (Bm)", "Hold me, hold me,", "G         A", "never let me go.", "", "N.C.", "Lead me to the water."
    };

    public static readonly string[] Song7 =
    {
        "Pane del cielo", "", "1. (Re)Pane del (La)cielo,", "(Si-)dono di (Sol)vita,", "(Re)resta con (La)noi.", "",
        "Rit. (Sol)Alleluia, (Re)alleluia!"
    };

    public static readonly string[] Song8 =
    {
        "Mi fido di te", "", "Do     Sol", "La mia vita è tua,", "Lam    Fa", "Si, lo voglio,", "Do         Sol",
        "Mi fido di te,", "Fa  Sol    Do", "Sol per te canto.", "La la la la"
    };

    public static IEnumerable<string[]> AllSongs => new[] { Song1, Song2, Song3, Song4, Song5, Song6, Song7, Song8 };

    [Fact]
    public void Song1_ItalianChorusRecallAndUnchordedVerse() =>
        Assert.Equal(
            Out(
                "{title:Resta con noi}", "", "[Do]Resta con [Sol]noi, [Lam]Signore,", "[Fa]la sera [Sol]ormai [Do]discende.", "",
                "{start_of_chorus}", "[Fa]Luce tu [Do]sei,", "[Rem]pace del [Sol]cuore.", "{end_of_chorus}", "",
                "{start_of_verse:2.}", "Resta con noi, Signore,", "la notte non ci vinca.", "{end_of_verse}", "", "{chorus}"),
            Run(Song1).ChordPro);

    [Fact]
    public void Song2_EnglishMetadataAndLabelledVerses() =>
        Assert.Equal(
            Out(
                "{title:Morning Light}", "{key:G}", "{capo:2}", "", "{start_of_verse:Verse 1}",
                "[G]Morning light [C]upon the [G]hill,", "[Em]all the [D]world is calm and still.", "{end_of_verse}", "",
                "{start_of_chorus}", "[C]Sing, my [G]soul, sing [D]out,", "[C]sing the [D]morning [G]in.", "{end_of_chorus}", "",
                "{start_of_verse:Verse 2}", "Every bird and every tree", "wakes to praise in harmony.", "{end_of_verse}", "",
                "{chorus}"),
            Run(Song2).ChordPro);

    [Fact]
    public void Song3_DashMinorsRomanCapoIntroAndChordPastTheEnd() =>
        Assert.Equal(
            Out(
                "{title:Vieni, Spirito}", "{key:Re}", "{capo:2}", "", "{comment:Intro}", "[Re] - [La] - [Si-] - [Sol]", "",
                "[Re]Vieni, [La]Spirito di luce,", "[Si-]scendi [Sol]su di [La]noi.", "", "{start_of_chorus}",
                "[Sol]Soffia, [La]vento di [Re]Dio,", "[Si-]rinnova la [Mi-]terra. [La]", "{end_of_chorus}"),
            Run(Song3).ChordPro);

    [Fact]
    public void Song4_BarsRepeatBareChordsAndNoChord()
    {
        var result = Run(Song4);

        Assert.Equal(
            Out(
                "{title:Canto di lode}", "", "| [Do] | [Fa] | [Sol] | [Do] | x2", "", "[Do]Lodate il [Fa]Signore,",
                "[Sol]popoli tutti, [Do]lodate.", "[*N.C.]Grande è il suo [Sol]amore,", "[Fa]eterna [Sol]la sua [Do]fedeltà."),
            result.ChordPro);
        Assert.Equal(Notation.Italian, result.DetectedNotation);
    }

    [Fact]
    public void Song5_PdfTextLayoutWithIndentationAndDrift()
    {
        var result = Run(Song5);

        Assert.Equal(
            Out("{title:SALMO 22}", "", "[Re]Il Signore è [Sol]il mio [Re]pastore,", "[Si-]non manco di [Mi-]nulla."),
            result.ChordPro);
        Assert.Contains(SheetWarning.SnappedColumn, result.Diag(5).Warnings);
    }

    [Fact]
    public void Song6_EnglishBridgeParenthesisedChordAndNoChordLine() =>
        Assert.Equal(
            Out(
                "{title:Quiet Waters}", "", "{start_of_verse:Verse 1}", "[D]Lead me to the [A]water,", "[Bm]quiet and so [G]clear.",
                "{end_of_verse}", "", "{start_of_bridge}", "[G]Hold me, [A]hold [Bm]me,", "[G]never let [A]me go.",
                "{end_of_bridge}", "", "[*N.C.]Lead me to the water."),
            Run(Song6).ChordPro);

    [Fact]
    public void Song7_InlineParenthesisedChordsNumberedVerseAndLabelWithRest() =>
        Assert.Equal(
            Out(
                "{title:Pane del cielo}", "", "{start_of_verse:1.}", "[Re]Pane del [La]cielo,", "[Si-]dono di [Sol]vita,",
                "[Re]resta con [La]noi.", "{end_of_verse}", "", "{start_of_chorus}", "[Sol]Alleluia, [Re]alleluia!",
                "{end_of_chorus}"),
            Run(Song7).ChordPro);

    [Fact]
    public void Song8_LyricLinesStartingWithNoteNames() =>
        Assert.Equal(
            Out(
                "{title:Mi fido di te}", "", "[Do]La mia [Sol]vita è tua,", "[Lam]Si, lo [Fa]voglio,", "[Do]Mi fido di [Sol]te,",
                "[Fa]Sol [Sol]per te [Do]canto.", "La la la la"),
            Run(Song8).ChordPro);

    [Fact]
    public void Acceptance_ConvertedItalianSong_RendersWithTranspositionFlatsAndCapo()
    {
        var parser = new ChordProParser();
        var converted = Run(Song1).ChordPro;
        var options = new ChordProRenderOptions
        {
            TransposeSteps = 2,
            Accidentals = AccidentalPreference.Flats,
            InputNotation = NotationOptions.Italian,
            OutputNotation = NotationOptions.Italian,
            Capo = 3
        };

        var rendered = parser.Render(parser.Parse(converted), options);

        Assert.Contains("{title:Resta con noi}", rendered);
        Assert.Contains("noi, ", rendered);
        Assert.DoesNotContain("[Rem]", rendered);
    }

    [Fact]
    public void Acceptance_ConvertedEnglishSong_RendersWithTranspositionFlatsAndCapo()
    {
        var parser = new ChordProParser();
        var options = new ChordProRenderOptions
        {
            TransposeSteps = 2,
            Accidentals = AccidentalPreference.Flats,
            InputNotation = NotationOptions.English,
            OutputNotation = NotationOptions.English,
            Capo = 3
        };

        var rendered = parser.Render(parser.Parse(Run(Song2).ChordPro), options);

        Assert.Contains("{title:Morning Light}", rendered);
        Assert.Contains("Morning light", rendered);
        Assert.DoesNotContain("[Em]", rendered);
    }
}
