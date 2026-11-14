using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>Editing only a header value of a parsed document changes that directive in place and keeps every other source line.</summary>
public class ChordProHeaderEditTests
{
    private const string Song =
        "# comment\n{title: Alleluja}\n{artist:Anonimo}\n{key: C}\n{capo: 2}\n{tempo: 100}\n{time:4/4}\n" +
        "{define: Am base-fret 1 frets x 0 2 2 1 0}\n{soc: Ritornello}\n[C]uno\n{chorus}\n{eoc}\n{ci: piano}\n{x_custom: v}\n{new_page}\n[F]due\n";

    private readonly ChordProParser _parser = new();

    private string Render(ChordProDocument document, ChordProRenderOptions? options = null) =>
        _parser.Render(document, options ?? new ChordProRenderOptions());

    [Fact]
    public void FixingATitleTypo_ChangesOnlyTheTitleLine()
    {
        var document = _parser.Parse(Song);
        document.Title = "Alleluia";

        Assert.Equal(Song.Replace("{title: Alleluja}", "{title: Alleluia}"), Render(document));
    }

    [Theory]
    [InlineData("{subtitle:Anonimo}", "Artist", "Ignoto", "{subtitle:Ignoto}")]
    [InlineData("{Artist : Anonimo }", "Artist", "Ignoto", "{Artist : Ignoto }")]
    [InlineData("{capo: 2}", "Capo", "3", "{capo: 3}")]
    [InlineData("{tempo: 100}", "Tempo", "90", "{tempo: 90}")]
    [InlineData("{time:4/4}", "Time", "3/4", "{time:3/4}")]
    public void EachHeaderValue_IsWrittenIntoItsOwnDirectiveKeepingNameAndPadding(string original, string field, string value, string expected)
    {
        var song = "# keep\n{title:T}\n" + original + "\n[C]x\n{x_custom: v}\n";
        var document = _parser.Parse(song);
        typeof(ChordProDocument).GetProperty(field)!.SetValue(document, value);

        Assert.Equal(song.Replace(original, expected), Render(document));
    }

    [Fact]
    public void EditedKey_IsTransposedLikeTheOriginalWouldBe()
    {
        var document = _parser.Parse("# keep\n{key: C}\n[C]x\n");
        document.Key = "G";

        Assert.Equal("# keep\n{key: A}\n[D]x\n", Render(document, new ChordProRenderOptions { TransposeSteps = 2 }));
    }

    [Fact]
    public void MissingDirectives_AreInsertedAfterTheTitleLine()
    {
        var document = _parser.Parse("# keep\n{title:T}\n[C]x\n{x_custom: v}\n");
        document.Tempo = "90";
        document.Time = "3/4";

        Assert.Equal("# keep\n{title:T}\n{tempo:90}\n{time:3/4}\n[C]x\n{x_custom: v}\n", Render(document));
    }

    [Fact]
    public void MissingDirectivesWithoutATitleLine_AreInsertedAtTheTop()
    {
        var document = _parser.Parse("# keep\n[C]x\n");
        document.Artist = "A";
        document.Tempo = "90";

        Assert.Equal("{artist:A}\n{tempo:90}\n# keep\n[C]x\n", Render(document));
    }

    [Fact]
    public void ATitleAddedToAFileWithoutOne_GoesFirst()
    {
        var document = _parser.Parse("# keep\n[C]x\n");
        document.Title = "Nuovo";

        Assert.Equal("{title:Nuovo}\n# keep\n[C]x\n", Render(document));
    }

    [Fact]
    public void HeaderEditTogetherWithABodyEdit_FallsBackToTheTypedMembers()
    {
        var document = _parser.Parse("# keep\n{title:T}\n[C]x\n");
        document.Title = "U";
        document.Lines[1].Text = "y";

        Assert.DoesNotContain("# keep", Render(document));
    }
}
