using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>A header edit lands in the directive the parser kept for that value, and no other line changes.</summary>
public class ChordProDuplicateHeaderTests
{
    private const string Edited = "Z9";
    private const string Slot = "@V@";

    private static readonly (string Property, string[] Names)[] Fields =
    {
        ("Title", new[] { "title", "t" }),
        ("Artist", new[] { "artist", "subtitle", "st" }),
        ("Key", new[] { "key" }),
        ("Capo", new[] { "capo" }),
        ("Tempo", new[] { "tempo" }),
        ("Time", new[] { "time" })
    };

    private static readonly string[] Shapes =
    {
        "{N: " + Slot + "}",
        "{N:" + Slot + "}",
        "  { N : " + Slot + " }  "
    };

    public static IEnumerable<object[]> Cases()
    {
        foreach (var (property, names) in Fields)
        {
            foreach (var name in names)
            {
                foreach (var shape in Shapes)
                {
                    yield return new object[] { property, shape.Replace("N", name), false };
                    yield return new object[] { property, shape.Replace("N", name.ToUpperInvariant()), false };
                    yield return new object[] { property, shape.Replace("N", name), true };
                }
            }
        }
    }

    private readonly ChordProParser _parser = new();

    private static string Get(ChordProDocument document, string property) =>
        (string)typeof(ChordProDocument).GetProperty(property)!.GetValue(document)!;

    [Theory]
    [MemberData(nameof(Cases))]
    public void EditedValue_IsWrittenIntoTheDirectiveTheParserKept_AndNothingElseChanges(string property, string shape, bool duplicated)
    {
        var other = Fields.First(f => f.Property != property).Names[0];
        var before = duplicated ? new[] { shape.Replace(Slot, "First"), "{x_decoy: v}" } : Array.Empty<string>();
        var kept = shape.Replace(Slot, "Last");
        var song = string.Join("\n", before.Concat(new[] { "# keep", kept, "{" + other + ": untouched}", "[C]la\n" }));
        var document = _parser.Parse(song);

        typeof(ChordProDocument).GetProperty(property)!.SetValue(document, Edited);
        var written = _parser.Render(document);

        Assert.Equal(song.Replace(kept, shape.Replace(Slot, Edited)), written);
        Assert.Equal(Edited, Get(_parser.Parse(written), property).Trim());
    }

    [Fact]
    public void SubtitleThenArtist_EditingTheArtistChangesTheArtistLine()
    {
        var document = _parser.Parse("{subtitle: Sub}\n{artist: Art}\n[C]x\n");
        document.Artist = "New";

        Assert.Equal("{subtitle: Sub}\n{artist: New}\n[C]x\n", _parser.Render(document));
    }

    [Fact]
    public void TwoKeyLines_EditingTheKeyChangesTheLastOne()
    {
        var document = _parser.Parse("{key: C}\n[C]x\n{key: D}\n[D]y\n");
        document.Key = "E";

        var written = _parser.Render(document);

        Assert.Equal("{key: C}\n[C]x\n{key: E}\n[D]y\n", written);
        Assert.Equal("E", _parser.Parse(written).Key.Trim());
    }

    [Fact]
    public void ADuplicateTitleWithAnEmptyValueAfterIt_EditingTheTitleChangesTheLastNonEmptyOne()
    {
        var document = _parser.Parse("{title: A}\n{title: B}\n{title:}\n[C]x\n");
        document.Title = "New";

        Assert.Equal("{title: A}\n{title: New}\n{title:}\n[C]x\n", _parser.Render(document));
    }

    [Fact]
    public void WithATransposition_EveryKeyLineShiftsAndTheEditedOneStartsFromTheEditedValue()
    {
        var document = _parser.Parse("{key: C}\n[C]x\n{key: D}\n[D]y\n");
        document.Key = "E";

        var written = _parser.Render(document, new ChordProRenderOptions { TransposeSteps = 2 });

        var keys = written.Split('\n').Where(l => l.StartsWith("{key:")).ToArray();
        Assert.Equal(new[] { "{key: D}", "{key: F#}" }, keys);
    }
}
