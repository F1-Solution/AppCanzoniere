using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>In a notation tie only a key that the input notation cannot read is frozen; a key written in it moves with the chords.</summary>
public class KeyTieInputNotationTests
{
    private const string TiedSong = "{key: G}\n[G]a [D]b\n[Sol]c [Re]d";

    private readonly ChordProParser _parser = new();

    private static ChordProRenderOptions Up(int steps) => new() { TransposeSteps = steps };

    [Fact]
    public void ATieWithAKeyInTheInputNotation_MovesTheKeyWithTheChords()
    {
        var result = _parser.Render(_parser.Parse(TiedSong), Up(2));

        Assert.Equal("{key: A}\n[A]a [E]b\n[Sol]c [Re]d\n", result);
    }

    [Fact]
    public void ATieWithAKeyOnlyReadableInTheOtherNotation_FreezesTheKey()
    {
        var result = _parser.Render(_parser.Parse("{key: Do}\n[Do]x [A]y"), Up(2));

        Assert.Equal("{key: Do}\n[Do]x [B]y\n", result);
    }

    [Fact]
    public void ATieWithAKeyInTheOtherNotationsWords_FreezesTheKey()
    {
        var result = _parser.Render(_parser.Parse("{key: La minore}\n[Am] [Re]"), Up(2));

        Assert.Equal("{key: La minore}\n[Bm] [Re]\n", result);
    }

    [Fact]
    public void ATieWithAKeyInTheInputNotation_WrittenAndSoundingKeysMove()
    {
        var document = _parser.Parse(TiedSong);

        Assert.Equal("A", ChordProKeys.WrittenKey(document, Up(2)));
        Assert.Equal("A", ChordProKeys.SoundingKey(document, Up(2)));
    }

    [Fact]
    public void ATieWithAKeyOnlyReadableInTheOtherNotation_HasNoWrittenKey()
    {
        var document = _parser.Parse("{key: Do}\n[Do]x [A]y");

        Assert.Null(ChordProKeys.WrittenKey(document, Up(2)));
        Assert.Null(ChordProKeys.SoundingKey(document, Up(2)));
    }
}
