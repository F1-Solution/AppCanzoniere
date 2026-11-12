using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

public class ChordProRoundTripTests
{
    private readonly ChordProParser _parser = new();

    private string RoundTrip(string content) => _parser.Render(_parser.Parse(content), 0);

    [Fact]
    public void Parse_ChordPositions_AreOffsetsInTheLyricsWithoutMarkers()
    {
        var line = _parser.Parse("[G]Line [D]one [Em]two").Lines[0];

        Assert.Equal("Line one two", line.Text);
        Assert.Equal(new[] { 0, 5, 9 }, line.Chords.Select(c => c.Position));
    }

    [Theory]
    [InlineData("[G]Line [D]one")]
    [InlineData("Ama[C]zing gra[G]ce")]
    [InlineData("[C][G]word")]
    [InlineData("[Am]Lyrics with [F]chord at the end[C]")]
    [InlineData("Plain lyrics only")]
    public void RoundTrip_KeepsChordsAtTheirOriginalPlaceInTheLine(string line)
    {
        Assert.Contains(line + "\n", RoundTrip(line));
    }

    [Fact]
    public void Render_UsesLineFeedOnEveryPlatform()
    {
        var rendered = RoundTrip("{title:T}\r\n[C]One\r\n\r\n[G]Two");

        Assert.DoesNotContain("\r", rendered);
        Assert.Equal("{title:T}\n\n[C]One\n\n[G]Two\n", rendered);
    }

    [Fact]
    public void Render_ChorusFollowedByBridge_ClosesTheChorusBeforeOpeningTheBridge()
    {
        var rendered = RoundTrip("{soc}\n[C]Chorus\n{eoc}\n{sob}\n[G]Bridge\n{eob}");

        Assert.Equal("{start_of_chorus}\n[C]Chorus\n{end_of_chorus}\n{start_of_bridge}\n[G]Bridge\n{end_of_bridge}\n", rendered);
    }

    [Fact]
    public void Render_TransposedLine_KeepsTheChordPositions()
    {
        var document = _parser.Parse("[C]Line [G]one");

        Assert.Contains("[D]Line [A]one\n", _parser.Render(document, 2));
    }

    [Fact]
    public void Render_DoesNotModifyTheDocument()
    {
        var document = _parser.Parse("[C]Line [G]one");

        _parser.Render(document, 2);

        Assert.Equal("C", document.Lines[0].Chords[0].Chord);
        Assert.Equal("Line one", document.Lines[0].Text);
    }
}
