using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Services;
using Xunit;

namespace AppCanzoniere.Application.Tests;

public class ChordProParserTests
{
    private readonly ChordProParser _parser;

    public ChordProParserTests()
    {
        _parser = new ChordProParser();
    }

    [Fact]
    public void Parse_EmptyContent_ReturnsEmptyDocument()
    {
        // Act
        var result = _parser.Parse(string.Empty);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Lines);
    }

    [Fact]
    public void Parse_TitleAndArtist_ExtractsMetadata()
    {
        // Arrange
        var content = @"{title:Amazing Grace}
{artist:John Newton}
{key:G}";

        // Act
        var result = _parser.Parse(content);

        // Assert
        Assert.Equal("Amazing Grace", result.Title);
        Assert.Equal("John Newton", result.Artist);
        Assert.Equal("G", result.Key);
    }

    [Fact]
    public void Parse_SimpleLine_ExtractsChordsAndLyrics()
    {
        // Arrange
        var content = "[G]Amazing [D]grace, how [G]sweet the sound";

        // Act
        var result = _parser.Parse(content);

        // Assert
        Assert.Single(result.Lines);
        var line = result.Lines[0];
        Assert.Equal("Amazing grace, how sweet the sound", line.Text);
        Assert.Equal(3, line.Chords.Count);
        Assert.Equal("G", line.Chords[0].Chord);
        Assert.Equal("D", line.Chords[1].Chord);
        Assert.Equal("G", line.Chords[2].Chord);
    }

    [Fact]
    public void Parse_ChorusSection_IdentifiesChorus()
    {
        // Arrange
        var content = @"{start_of_chorus}
[C]This is the chorus
[G]With multiple lines
{end_of_chorus}";

        // Act
        var result = _parser.Parse(content);

        // Assert
        Assert.Equal(2, result.Lines.Count);
        Assert.All(result.Lines, line => Assert.Equal(ChordProLineType.Chorus, line.Type));
    }

    [Fact]
    public void Parse_ComplexSong_ParsesCorrectly()
    {
        // Arrange
        var content = @"{title:Test Song}
{artist:Test Artist}
{key:C}
{capo:2}

[C]Verse line [G]one
[Am]Verse line [F]two

{start_of_chorus}
[C]Chorus [G]line
{end_of_chorus}

{comment:Bridge section}
{start_of_bridge}
[Dm]Bridge [G]line
{end_of_bridge}";

        // Act
        var result = _parser.Parse(content);

        // Assert
        Assert.Equal("Test Song", result.Title);
        Assert.Equal("Test Artist", result.Artist);
        Assert.Equal("C", result.Key);
        Assert.Equal("2", result.Capo);
        Assert.True(result.Lines.Count > 0);
    }

    [Fact]
    public void Render_WithoutTranspose_PreservesOriginalChords()
    {
        // Arrange
        var document = new ChordProDocument
        {
            Title = "Test",
            Key = "C",
            Lines = new List<ChordProLine>
            {
                new ChordProLine
                {
                    Type = ChordProLineType.Verse,
                    Text = "Amazing grace",
                    Chords = new List<ChordPosition>
                    {
                        new ChordPosition { Chord = "C", Position = 0 },
                        new ChordPosition { Chord = "G", Position = 8 }
                    }
                }
            }
        };

        // Act
        var result = _parser.Render(document, 0);

        // Assert
        Assert.Contains("{title:Test}", result);
        Assert.Contains("{key:C}", result);
        Assert.Contains("[C]", result);
        Assert.Contains("[G]", result);
    }

    [Fact]
    public void Render_WithTranspose_TransposesChords()
    {
        // Arrange
        var document = new ChordProDocument
        {
            Key = "C",
            Lines = new List<ChordProLine>
            {
                new ChordProLine
                {
                    Type = ChordProLineType.Verse,
                    Text = "Test line",
                    Chords = new List<ChordPosition>
                    {
                        new ChordPosition { Chord = "C", Position = 0 }
                    }
                }
            }
        };

        // Act
        var result = _parser.Render(document, 2);

        // Assert
        Assert.Contains("{key:D}", result);
        Assert.Contains("[D]", result);
    }

    [Fact]
    public void Parse_RoundTrip_PreservesContent()
    {
        // Arrange
        var originalContent = @"{title:Test Song}
{key:G}
[G]Line [D]one
[Em]Line [C]two";

        // Act
        var document = _parser.Parse(originalContent);
        var rendered = _parser.Render(document, 0);
        var reparsed = _parser.Parse(rendered);

        // Assert
        Assert.Equal(document.Title, reparsed.Title);
        Assert.Equal(document.Key, reparsed.Key);
        // Rendered content adds empty lines, so count may differ
        Assert.True(reparsed.Lines.Count >= 2);
    }

    [Theory]
    [InlineData("{t:Title}", "Title", true)]
    [InlineData("{title:Title}", "Title", true)]
    public void Parse_DirectiveAliases_ParsesCorrectly(string directive, string expected, bool isTitle)
    {
        // Act
        var result = _parser.Parse(directive);

        // Assert
        if (isTitle)
            Assert.Equal(expected, result.Title);
    }
}
