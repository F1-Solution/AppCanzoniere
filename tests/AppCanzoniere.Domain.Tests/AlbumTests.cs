using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Enums;

namespace AppCanzoniere.Domain.Tests;

public class AlbumTests
{
    [Fact]
    public void Album_ShouldInitializeWithDefaultValues()
    {
        // Arrange & Act
        var album = new Album();

        // Assert
        Assert.NotEqual(Guid.Empty, album.Id);
        Assert.Equal(string.Empty, album.Name);
        Assert.Equal(AlbumType.Manual, album.Type);
        Assert.False(album.IsPublic);
        Assert.NotNull(album.AlbumSongs);
        Assert.Empty(album.AlbumSongs);
    }

    [Fact]
    public void Album_ShouldSetPropertiesCorrectly()
    {
        // Arrange & Act
        var album = new Album
        {
            Name = "Christmas Songs",
            Description = "A collection of Christmas hymns",
            Type = AlbumType.Manual,
            IsPublic = true
        };

        // Assert
        Assert.Equal("Christmas Songs", album.Name);
        Assert.Equal("A collection of Christmas hymns", album.Description);
        Assert.Equal(AlbumType.Manual, album.Type);
        Assert.True(album.IsPublic);
    }
}
