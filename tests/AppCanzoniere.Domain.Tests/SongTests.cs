using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Enums;

namespace AppCanzoniere.Domain.Tests;

public class SongTests
{
    [Fact]
    public void Song_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var song = new Song();

        // Assert
        Assert.NotEqual(Guid.Empty, song.Id);
        Assert.Equal(string.Empty, song.Title);
        Assert.Equal(string.Empty, song.Artist);
        Assert.Equal("en", song.Language);
        Assert.Equal("C", song.OriginalKey);
        Assert.Equal(ContentFormat.ChordPro, song.PrimaryFormat);
        Assert.Equal(1, song.Version);
        Assert.False(song.IsSynced);
        Assert.False(song.IsDownloadedForOffline);
    }

    [Fact]
    public void Song_ShouldSetPropertiesCorrectly()
    {
        // Arrange & Act
        var song = new Song
        {
            Title = "Amazing Grace",
            Artist = "John Newton",
            Language = "en",
            Type = SongType.ChurchSong,
            OriginalKey = "G",
            ChordProContent = "{title: Amazing Grace}",
            IsPublic = true
        };

        // Assert
        Assert.Equal("Amazing Grace", song.Title);
        Assert.Equal("John Newton", song.Artist);
        Assert.Equal("en", song.Language);
        Assert.Equal(SongType.ChurchSong, song.Type);
        Assert.Equal("G", song.OriginalKey);
        Assert.NotNull(song.ChordProContent);
        Assert.True(song.IsPublic);
    }

    [Fact]
    public void Song_NavigationProperties_ShouldInitializeAsEmpty()
    {
        // Arrange & Act
        var song = new Song();

        // Assert
        Assert.NotNull(song.AlbumSongs);
        Assert.Empty(song.AlbumSongs);
        Assert.NotNull(song.SongTags);
        Assert.Empty(song.SongTags);
        Assert.NotNull(song.BibleReferences);
        Assert.Empty(song.BibleReferences);
        Assert.NotNull(song.YouTubeLinks);
        Assert.Empty(song.YouTubeLinks);
    }
}
