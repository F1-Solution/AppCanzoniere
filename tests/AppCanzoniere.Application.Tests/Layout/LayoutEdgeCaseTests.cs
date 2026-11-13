using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Layout;
using static AppCanzoniere.Application.Tests.Layout.LayoutKit;

namespace AppCanzoniere.Application.Tests.Layout;

/// <summary>Edge cases of the layout contract: overflow flags, boxes, ordinals, offsets, indexes and non-finite coordinates.</summary>
public class LayoutEdgeCaseTests
{
    private sealed class RoleMetricsMeasurer : ITextMeasurer
    {
        private readonly FixedAdvanceMeasurer _inner = new();
        private readonly TextRole _role;
        private readonly double _height;
        private readonly double _ascent;

        public RoleMetricsMeasurer(TextRole role, double height, double ascent)
        {
            _role = role;
            _height = height;
            _ascent = ascent;
        }

        public TextMetrics Measure(string text, TextRole role)
        {
            var metrics = _inner.Measure(text, role);
            return role == _role ? metrics with { LineHeight = _height, Ascent = _ascent } : metrics;
        }
    }

    private sealed class HugeMeasurer : ITextMeasurer
    {
        public TextMetrics Measure(string text, TextRole role) => new(text.Length == 0 ? 0 : double.MaxValue / 2, 10, 8);
    }

    private static LayoutSourceLine Line(string text, int documentIndex = -1, params LayoutChord[] chords) =>
        new(ChordProLineType.Verse, text, chords, false, string.Empty, documentIndex);

    [Fact]
    public void TheLineNumberBox_StaysInsideItsRowAtTheBottomToo()
    {
        var options = Options() with { LineNumbers = true };
        var flow = Flow("[C]Uno", options, new RoleMetricsMeasurer(TextRole.LineNumber, 24, 5));

        var row = OnlyRow(OnlySection(flow).Lines[0]);
        var number = Single(row, RunKind.LineNumber);
        Assert.True(number.Y >= row.Y);
        Assert.True(number.Y + number.Height <= row.Y + row.Height + 1e-9);
    }

    [Fact]
    public void ALineNumberTallerThanItsRow_IsAlignedToTheRowTop()
    {
        var options = Options() with { LineNumbers = true };
        var flow = Flow("Uno", options, new RoleMetricsMeasurer(TextRole.LineNumber, 60, 40));

        var row = OnlyRow(OnlySection(flow).Lines[0]);
        Assert.Equal(row.Y, Single(row, RunKind.LineNumber).Y);
    }

    [Fact]
    public void TheChordOrdinal_IsTheIndexInTheLineChordsAsGiven_NotAfterSorting()
    {
        var source = new LayoutSource(new[] { Line("Voce", -1, new LayoutChord("G", 3), new LayoutChord("C", 0)) });

        var flow = SongLayoutEngine.Flow(source, Options(), new FixedAdvanceMeasurer());

        var chords = Of(OnlyRow(OnlySection(flow).Lines[0]), RunKind.Chord);
        Assert.Equal(new[] { ("C", 1), ("G", 0) }, chords.Select(c => (c.Text, c.ChordIndex)));
    }

    [Fact]
    public void TheChordOrdinal_OfAHandBuiltDocumentLine_FollowsItsChordsList()
    {
        var document = new ChordProDocument();
        document.Lines.Add(new ChordProLine
        {
            Type = ChordProLineType.Verse,
            Text = "Voce",
            Chords = { new ChordPosition { Chord = "G", Position = 3 }, new ChordPosition { Chord = "C", Position = 0 } }
        });

        var flow = SongLayoutEngine.Flow(LayoutSource.From(document), Options(), new FixedAdvanceMeasurer());

        Assert.Equal(new[] { 1, 0 }, Of(OnlyRow(OnlySection(flow).Lines[0]), RunKind.Chord).Select(c => c.ChordIndex));
    }

    [Theory]
    [InlineData(400)]
    [InlineData(100)]
    public void ACommentRunOffset_IsMeasuredInTheCommentTextAsParsed(double width)
    {
        var source = Source("{c:    Ripetere due volte}");
        var flow = SongLayoutEngine.Flow(source, Options(width), new FixedAdvanceMeasurer());

        var runs = OnlySection(flow).Lines.SelectMany(l => l.Rows).SelectMany(r => r.Runs).Where(r => r.Kind == RunKind.Comment).ToArray();
        Assert.Equal(4, runs[0].SourceOffset);
        Assert.All(runs, r => Assert.Equal(r.Text, source.Lines[0].Text.Substring(r.SourceOffset, r.Text.Length)));
    }

    [Fact]
    public void AClippedTabRowWiderThanTheWidth_HasItsOverflowFlagSet()
    {
        var wide = new string('-', 100);
        var row = OnlyRow(OnlySection(Flow($"{{sot}}\n{wide}\n{{eot}}", Options(100))).Lines[0]);

        Assert.True(row.Overflow);
        Assert.False(OnlyRow(OnlySection(Flow("{sot}\ne|-0-|\n{eot}", Options(100))).Lines[0]).Overflow);
    }

    [Fact]
    public void DuplicateDocumentIndexes_AreRejected()
    {
        Assert.Throws<ArgumentException>(() => new LayoutSource(new[] { Line("a", 3), Line("b", 3) }));
        Assert.Throws<ArgumentException>(() => new LayoutSource(new[] { Line("a"), Line("b", 0) }));
    }

    [Fact]
    public void ANegativeDocumentIndex_IsRejected()
    {
        Assert.Throws<ArgumentException>(() => Line("a", -5));
    }

    [Fact]
    public void ASpacingSoLargeThatAYOverflows_RaisesInvalidOperation()
    {
        var options = Options() with { LineSpacing = double.MaxValue };

        Assert.Throws<InvalidOperationException>(() => Flow("a\nb\nc", options));
    }

    [Fact]
    public void AMeasurerSoLargeThatAnXOverflows_RaisesInvalidOperation()
    {
        var options = Options(double.MaxValue);

        Assert.Throws<InvalidOperationException>(() => Flow("[C]ab[G]cd", options, new HugeMeasurer()));
    }
}
