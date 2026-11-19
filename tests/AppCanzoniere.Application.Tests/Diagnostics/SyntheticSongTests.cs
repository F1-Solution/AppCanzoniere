using AppCanzoniere.Application.Diagnostics;
using AppCanzoniere.Application.Services;
using Xunit;

namespace AppCanzoniere.Application.Tests.Diagnostics;

public class SyntheticSongTests
{
    [Fact]
    public void Create_ReturnsChordProSongWithTitle()
    {
        var song = SyntheticSong.Create();

        Assert.Equal(SyntheticSong.Title, song.Title);
        Assert.False(string.IsNullOrWhiteSpace(song.ChordProContent));
    }

    [Fact]
    public void ChordPro_ParsesToSeveralScreensOfLines()
    {
        var document = new ChordProParser().Parse(SyntheticSong.ChordPro());

        Assert.Equal(SyntheticSong.Title, document.Title);
        Assert.True(document.Lines.Count >= SyntheticSong.VerseCount * 4);
    }

    [Fact]
    public void ChordPro_ContainsChordsOnEveryVerse()
    {
        var text = SyntheticSong.ChordPro();

        Assert.Equal(SyntheticSong.VerseCount, text.Split("{start_of_verse}").Length - 1);
        Assert.Contains("[Em]", text);
    }
}
