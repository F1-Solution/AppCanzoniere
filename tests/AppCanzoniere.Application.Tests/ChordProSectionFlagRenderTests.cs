using System.Text;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>Rebuilding text from the typed lines follows the section flags the parser sets.</summary>
public class ChordProSectionFlagRenderTests
{
    private readonly ChordProParser _parser = new();

    private static ChordProLine Line(ChordProLineType type, string text, bool starts = false, bool inOpen = false) =>
        new() { Type = type, Text = text, StartsSection = starts, InOpenSection = inOpen };

    private string Render(params ChordProLine[] lines) =>
        _parser.Render(new ChordProDocument { Lines = lines.ToList() });

    [Fact]
    public void TwoAdjacentChoruses_StayTwoChoruses()
    {
        var text = Render(Line(ChordProLineType.Chorus, "a", starts: true), Line(ChordProLineType.Chorus, "b", starts: true));

        Assert.Equal("{start_of_chorus}\na\n{end_of_chorus}\n{start_of_chorus}\nb\n{end_of_chorus}\n", text);
    }

    [Fact]
    public void ACommentInsideAnOpenSection_DoesNotCloseAndReopenIt()
    {
        var text = Render(
            Line(ChordProLineType.Chorus, "a", starts: true),
            Line(ChordProLineType.Comment, "x", inOpen: true),
            Line(ChordProLineType.Chorus, "b"));

        Assert.Equal("{start_of_chorus}\na\n{comment:x}\nb\n{end_of_chorus}\n", text);
    }

    [Fact]
    public void ATrailingCommentInsideASection_StaysInsideIt()
    {
        var text = Render(
            Line(ChordProLineType.Chorus, "a", starts: true),
            Line(ChordProLineType.Comment, "x", inOpen: true),
            Line(ChordProLineType.Verse, "c"));

        Assert.Equal("{start_of_chorus}\na\n{comment:x}\n{end_of_chorus}\nc\n", text);
    }

    [Fact]
    public void ABlankLineInsideASection_StaysInsideIt()
    {
        var text = Render(
            Line(ChordProLineType.Bridge, "a", starts: true),
            Line(ChordProLineType.Empty, string.Empty, inOpen: true),
            Line(ChordProLineType.Bridge, "b"));

        Assert.Equal("{start_of_bridge}\na\n\nb\n{end_of_bridge}\n", text);
    }

    [Fact]
    public void ParsedTwoChoruses_EditedInAVerse_KeepTheirTwoChoruses()
    {
        var document = _parser.Parse("{soc}\n[C]a\n{eoc}\n{soc}\n[C]b\n{eoc}\nverso\n");
        document.Lines[^1].Text = "Verso";

        Assert.Equal("{start_of_chorus}\n[C]a\n{end_of_chorus}\n{start_of_chorus}\n[C]b\n{end_of_chorus}\nVerso\n", _parser.Render(document));
    }

    private static string RandomSong(Random random)
    {
        var pieces = new[] { "[C]uno due", "tre [G]quattro", "{soc}", "{eoc}", "{sob}", "{eob}", "{c: nota}", "", "{sot: T}\n-0-1-\n{eot}", "{sot}\n-2-\n{eot}", "[Am]cinque" };
        var sb = new StringBuilder();
        var count = random.Next(1, 14);
        for (var i = 0; i < count; i++)
            sb.Append(pieces[random.Next(pieces.Length)]).Append('\n');

        return sb.ToString();
    }

    private static List<(ChordProLineType, bool, bool, string)> Shape(ChordProDocument document) =>
        document.Lines.Select(l => (l.Type, l.StartsSection, l.InOpenSection, l.Text)).ToList();

    [Fact]
    public void EditingOneLyricCharacter_KeepsEverySectionBoundary_ForGeneratedDocuments()
    {
        var random = new Random(20260514);
        var edited = 0;
        for (var n = 0; n < 4000; n++)
        {
            var song = RandomSong(random);
            var document = _parser.Parse(song);
            var verses = document.Lines.Where(l => l.Type == ChordProLineType.Verse && l.Text.Length > 0).ToList();
            if (verses.Count == 0)
                continue;

            var target = verses[random.Next(verses.Count)];
            var expected = Shape(document);
            var index = document.Lines.IndexOf(target);
            expected[index] = (expected[index].Item1, expected[index].Item2, expected[index].Item3, "Z" + target.Text[1..]);

            target.Text = "Z" + target.Text[1..];
            var written = _parser.Render(document);

            Assert.True(expected.SequenceEqual(Shape(_parser.Parse(written))), $"Song:\n{song}\nWritten:\n{written}");
            edited++;
        }

        Assert.True(edited > 1000);
    }
}
