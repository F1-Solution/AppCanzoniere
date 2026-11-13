using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>A key moves only when the document's chords are not written in the other notation than the one requested.</summary>
public class KeyNotationConsistencyTests
{
    private const string ItalianSong = "{key: La}\n[La]Alleluia [Re]amen";

    private readonly ChordProParser _parser = new();

    private string Render(string song, ChordProRenderOptions options) => _parser.Render(_parser.Parse(song), options);

    private static ChordProRenderOptions Italian(int steps) => new()
    {
        TransposeSteps = steps,
        InputNotation = NotationOptions.Italian,
        OutputNotation = NotationOptions.Italian
    };

    [Fact]
    public void ItalianSongRenderedWithEnglishInput_StaysByteIdentical()
    {
        Assert.Equal(LosslessAssert.Expected(ItalianSong), Render(ItalianSong, new ChordProRenderOptions { TransposeSteps = 2 }));
        Assert.Equal(LosslessAssert.Expected(ItalianSong), _parser.Render(_parser.Parse(ItalianSong), 2));
    }

    [Fact]
    public void ItalianSongRenderedWithItalianInput_MovesKeyAndChordsTogether()
    {
        Assert.Equal("{key: Si}\n[Si]Alleluia [Mi]amen\n", Render(ItalianSong, Italian(2)));
    }

    [Fact]
    public void MoreChordsInTheOtherNotation_LeaveTheKeyAlone()
    {
        var result = Render("{key: La minore}\n[Am] [Re] [Sol]", new ChordProRenderOptions { TransposeSteps = 2 });

        Assert.Equal("{key: La minore}\n[Bm] [Re] [Sol]\n", result);
    }

    [Fact]
    public void EqualCountsOnBothSides_HonourTheWordedKey()
    {
        var result = Render("{key: La minore}\n[Am] [Re]", new ChordProRenderOptions { TransposeSteps = 2 });

        Assert.Equal("{key: Si minore}\n[Bm] [Re]\n", result);
    }

    [Fact]
    public void MoreChordsInTheRequestedNotation_HonourTheKeyInTheOtherNotation()
    {
        var result = Render("{key:La minore}\n[Am] [Dm] [Re]", new ChordProRenderOptions { TransposeSteps = 2 });

        Assert.Equal("{key:Si minore}\n[Bm] [Em] [Re]\n", result);
    }

    [Fact]
    public void WordedKeyOverEnglishChords_IsHonoured()
    {
        Assert.Equal("{key:Si minore}\n[Bm] [Em]\n", Render("{key:La minore}\n[Am] [Dm]", new ChordProRenderOptions { TransposeSteps = 2 }));
    }

    [Fact]
    public void DocumentWithoutChords_HonoursTheKey()
    {
        Assert.Equal("{key:Si minore}\nsolo testo\n", Render("{key:La minore}\nsolo testo", new ChordProRenderOptions { TransposeSteps = 2 }));
    }

    [Fact]
    public void ChordsInATabBlock_AreNotCounted()
    {
        var document = new ChordProDocument { Key = "La minore" };
        document.Lines.Add(new ChordProLine { Type = ChordProLineType.Verse, Text = "x", Chords = { new ChordPosition { Chord = "Am", Position = 0 } } });
        document.Lines.Add(new ChordProLine
        {
            Type = ChordProLineType.Tab,
            Text = "y",
            Chords = { new ChordPosition { Chord = "Re", Position = 0 }, new ChordPosition { Chord = "Sol", Position = 0 } }
        });

        Assert.Equal("Si minore", ChordProKeys.WrittenKey(document, new ChordProRenderOptions { TransposeSteps = 2 }));
    }

    [Fact]
    public void Helpers_ReturnNullWhenTheDocumentIsInTheOtherNotation()
    {
        var document = _parser.Parse(ItalianSong);
        var english = new ChordProRenderOptions { TransposeSteps = 2 };

        Assert.Null(ChordProKeys.WrittenKey(document, english));
        Assert.Null(ChordProKeys.SoundingKey(document, english));
        Assert.Equal("Si", ChordProKeys.WrittenKey(document, Italian(2)));
        Assert.Equal("Si", ChordProKeys.SoundingKey(document, Italian(2)));
    }

    [Fact]
    public void HandBuiltDocumentInTheOtherNotation_FollowsTheSameRule()
    {
        var document = new ChordProDocument { Key = "La" };
        document.Lines.Add(new ChordProLine { Type = ChordProLineType.Verse, Text = "x", Chords = { new ChordPosition { Chord = "La", Position = 0 } } });

        Assert.Equal("{key:La}\n[La]x\n", _parser.Render(document, new ChordProRenderOptions { TransposeSteps = 2 }));
        Assert.Equal("{key:Si}\n[Si]x\n", _parser.Render(document, Italian(2)));
    }
}
