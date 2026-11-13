using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Layout;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests.Layout;

/// <summary>Invariants of the layout over 300 random songs, widths, modes and page heights from a fixed seed.</summary>
public class LayoutPropertyTests
{
    private const int Seed = 20260513;
    private const int SongCount = 300;
    private const double Epsilon = 1e-6;

    private sealed record Case(string Song, LayoutOptions Options, ChordProDocument Document, FlowLayout Flow, SongLayout Layout);

    private static IEnumerable<Case> Cases()
    {
        var random = new Random(Seed);
        var parser = new ChordProParser();
        for (var i = 0; i < SongCount; i++)
        {
            var song = SongGenerator.Song(random);
            var options = new LayoutOptions
            {
                Width = random.Next(40, 500),
                PageHeight = random.Next(3) == 0 ? null : random.Next(60, 400),
                LineNumbers = random.Next(2) == 0,
                ChordStyle = random.Next(2) == 0 ? ChordBoxStyle.Plain : ChordBoxStyle.Pill,
                DisplayMode = random.Next(5) == 0 ? LayoutDisplayMode.LyricsOnly : LayoutDisplayMode.ChordsAndLyrics,
            };
            var document = parser.Parse(song);
            var flow = SongLayoutEngine.Flow(LayoutSource.From(document), options, new NonUniformMeasurer());
            yield return new Case(song, options, document, flow, SongLayoutEngine.Paginate(flow, options));
        }
    }

    private static string Describe(Case c) => $"width {c.Options.Width}, page {c.Options.PageHeight}, song:\n{c.Song}";

    private static IEnumerable<LayoutSection> Sections(SongLayout layout) => layout.Pages.SelectMany(p => p.Sections);

    private static IEnumerable<LayoutRow> Rows(IEnumerable<LayoutSection> sections) =>
        sections.SelectMany(s => s.Lines).SelectMany(l => l.Rows);

    [Fact]
    public void TheGeneratedSongs_ExerciseWrappingOverflowPaginationAndSectionComments()
    {
        var cases = Cases().ToArray();
        var rows = cases.SelectMany(c => Rows(Sections(c.Layout))).ToArray();

        Assert.Contains(cases, c => c.Layout.Pages.Count > 1);
        Assert.Contains(rows, r => r.Index > 0);
        Assert.Contains(rows, r => r.Overflow);
        Assert.Contains(cases, c => Sections(c.Layout).Any(s => s.Kind == SectionKind.Chorus && s.Lines.Any(l => l.IsComment)));
        Assert.Contains(cases, c => Sections(c.Layout).Any(s => s.Kind == SectionKind.Tab));
    }

    [Fact]
    public void NoRowExceedsTheWidth_UnlessItIsFlagged()
    {
        foreach (var c in Cases())
        {
            foreach (var section in Sections(c.Layout).Where(s => s.Kind != SectionKind.Tab))
            {
                foreach (var row in section.Lines.SelectMany(l => l.Rows))
                {
                    var right = row.Runs.Where(r => r.Kind != RunKind.LineNumber).Select(r => r.X + r.Width).DefaultIfEmpty(0).Max();
                    Assert.True(row.Overflow || right <= c.Options.Width + Epsilon, $"right {right}\n{Describe(c)}");
                }
            }

            foreach (var tab in Sections(c.Layout).Where(s => s.Kind == SectionKind.Tab))
            {
                var right = tab.Lines.SelectMany(l => l.Rows).SelectMany(r => r.Runs).Select(r => r.X + r.Width).DefaultIfEmpty(0).Max();
                Assert.True(tab.TabClipped || right <= c.Options.Width + Epsilon, Describe(c));
            }
        }
    }

    [Fact]
    public void ChordsOnARow_NeverOverlap()
    {
        foreach (var c in Cases())
        {
            foreach (var row in Rows(Sections(c.Layout)))
            {
                var chords = row.Runs.Where(r => r.Kind is RunKind.Chord or RunKind.Annotation).OrderBy(r => r.X).ToArray();
                for (var i = 1; i < chords.Length; i++)
                    Assert.True(chords[i].X >= chords[i - 1].X + chords[i - 1].Width - Epsilon, Describe(c));
            }
        }
    }

    [Fact]
    public void YIncreasesStrictlyWithoutOverlap_ForSectionsLinesAndRows()
    {
        foreach (var c in Cases())
        {
            AssertStacked(c.Flow.Sections.Select(s => (s.Y, s.Height)), c);
            foreach (var page in c.Layout.Pages)
                AssertStacked(page.Sections.Select(s => (s.Y, s.Height)), c);

            foreach (var section in Sections(c.Layout))
            {
                AssertStacked(section.Lines.Select(l => (l.Y, l.Height)), c);
                foreach (var line in section.Lines)
                    AssertStacked(line.Rows.Select(r => (r.Y, r.Height)), c);
            }
        }
    }

    private static void AssertStacked(IEnumerable<(double Y, double Height)> boxes, Case c)
    {
        var list = boxes.ToArray();
        for (var i = 1; i < list.Length; i++)
        {
            Assert.True(list[i].Y > list[i - 1].Y, Describe(c));
            Assert.True(list[i].Y >= list[i - 1].Y + list[i - 1].Height - Epsilon, Describe(c));
        }
    }

    [Fact]
    public void EveryLyricAndCommentCharacter_AppearsExactlyOnceInOrder()
    {
        foreach (var c in Cases())
        {
            var shown = Sections(c.Layout).SelectMany(s => s.Lines)
                .GroupBy(l => l.SourceLineIndex)
                .ToDictionary(g => g.Key, g => string.Concat(g.SelectMany(l => l.Rows).SelectMany(r => r.Runs)
                    .Where(r => r.Kind is RunKind.Lyric or RunKind.Comment).Select(r => r.Text)));

            foreach (var (index, text) in shown)
            {
                var line = c.Document.Lines[index];
                if (line.Type == ChordProLineType.Tab)
                    continue;

                Assert.Equal(WithoutSpaces(line.Text), WithoutSpaces(text));
            }

            foreach (var (line, index) in c.Document.Lines.Select((l, i) => (l, i))
                .Where(x => x.l.Type is ChordProLineType.Verse or ChordProLineType.Chorus or ChordProLineType.Bridge && x.l.Text.Trim().Length > 0))
            {
                Assert.True(shown.ContainsKey(index), $"line {index} is missing\n{Describe(c)}");
            }
        }
    }

    private static string WithoutSpaces(string text) => string.Concat(text.Where(ch => !char.IsWhiteSpace(ch)));

    [Fact]
    public void EveryPageFits_UnlessItHoldsASingleRowAndIsFlagged()
    {
        foreach (var c in Cases().Where(x => x.Options.PageHeight is not null))
        {
            foreach (var page in c.Layout.Pages)
            {
                var rows = Rows(page.Sections).Count();
                Assert.True(page.Height <= c.Options.PageHeight + Epsilon || (page.Overflow && rows == 1), $"page {page.Index} height {page.Height}\n{Describe(c)}");
            }
        }
    }

    [Fact]
    public void Spreads_CoverEveryPage()
    {
        foreach (var pageCount in Cases().Select(c => c.Layout.Pages.Count).Distinct())
        {
            var full = Enumerable.Range(0, SpreadCalculator.TurnCount(pageCount, SpreadMode.Full))
                .SelectMany(t => Shown(SpreadCalculator.At(t, pageCount, SpreadMode.Full))).ToArray();
            Assert.Equal(Enumerable.Range(0, pageCount), full.OrderBy(p => p));

            var half = Enumerable.Range(0, SpreadCalculator.TurnCount(pageCount, SpreadMode.HalfTurn))
                .SelectMany(t => Shown(SpreadCalculator.At(t, pageCount, SpreadMode.HalfTurn))).ToHashSet();
            Assert.Equal(Enumerable.Range(0, pageCount).ToHashSet(), half);
        }
    }

    private static IEnumerable<int> Shown((int Left, int? Right) spread) =>
        spread.Right is { } right ? new[] { spread.Left, right } : new[] { spread.Left };
}
