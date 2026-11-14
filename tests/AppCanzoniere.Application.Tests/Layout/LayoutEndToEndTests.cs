using AppCanzoniere.Application.Layout;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests.Layout;

public class LayoutEndToEndTests
{
    private const string StoredSong =
        "{title: Inno di prova}\n" +
        "{key: G}\n" +
        "[G]Santo [D]Signore\n" +
        "[Em]Dio dell'[C]universo\n" +
        "\n" +
        "{start_of_chorus}\n" +
        "[G]Osanna\n" +
        "{end_of_chorus}\n";

    private static LayoutSource ShownSource()
    {
        var parser = new ChordProParser();
        var options = new ChordProRenderOptions { TransposeSteps = 2, OutputNotation = NotationOptions.Italian };
        var shown = parser.Parse(parser.Render(parser.Parse(StoredSong), options));
        return LayoutSource.From(shown);
    }

    private static string Describe(LayoutRun run) =>
        $"{run.Kind} '{run.Text}' x={run.X} y={run.Y} w={run.Width} h={run.Height} base={run.BaselineY}";

    [Fact]
    public void ParsedTransposedItalianSong_IsLaidOutWithTheFakeMeasurer()
    {
        var flow = SongLayoutEngine.Flow(ShownSource(), new LayoutOptions { Width = 400 }, new FixedAdvanceMeasurer());

        Assert.Equal(new[] { SectionKind.Verse, SectionKind.Chorus }, flow.Sections.Select(s => s.Kind));
        var first = flow.Sections[0];
        Assert.Equal((0d, 92d), (first.Y, first.Height));
        Assert.Equal(new[] { 0d, 48 }, first.Lines.Select(l => l.Y));

        var described = first.Lines.SelectMany(l => l.Rows).Select(r => r.Runs.Select(Describe).ToArray()).ToArray();
        Assert.Equal(
            new[]
            {
                "Chord 'La' x=0 y=0 w=16 h=20 base=16",
                "Lyric 'Santo ' x=0 y=20 w=60 h=24 base=39",
                "Chord 'Mi' x=60 y=0 w=16 h=20 base=16",
                "Lyric 'Signore' x=60 y=20 w=70 h=24 base=39",
            },
            described[0]);
        Assert.Equal(
            new[]
            {
                "Chord 'Fa#m' x=0 y=48 w=32 h=20 base=64",
                "Lyric 'Dio dell'' x=0 y=68 w=90 h=24 base=87",
                "Chord 'Re' x=90 y=48 w=16 h=20 base=64",
                "Lyric 'universo' x=90 y=68 w=80 h=24 base=87",
            },
            described[1]);
    }

    [Fact]
    public void TheChorusUsesTheChorusRole_AndPagesSplitAtTheGivenHeight()
    {
        var options = new LayoutOptions { Width = 400 };
        var source = ShownSource();
        var measurer = new FixedAdvanceMeasurer();

        var flow = SongLayoutEngine.Flow(source, options, measurer);
        var chorusLyric = flow.Sections[1].Lines[0].Rows[0].Runs.Single(r => r.Kind == RunKind.Lyric);
        Assert.Equal((TextRole.ChorusLyric, "Osanna", 66d), (chorusLyric.Role, chorusLyric.Text, chorusLyric.Width));
        Assert.Equal(108, flow.Sections[1].Y);

        var tall = SongLayoutEngine.Paginate(flow, options with { PageHeight = 160 });
        var short150 = SongLayoutEngine.Paginate(flow, options with { PageHeight = 150 });

        Assert.Single(tall.Pages);
        Assert.Equal(2, short150.Pages.Count);
        Assert.Equal(0, short150.Pages[1].Sections[0].Y);
        Assert.Equal(92, short150.Pages[0].Height);
    }
}
