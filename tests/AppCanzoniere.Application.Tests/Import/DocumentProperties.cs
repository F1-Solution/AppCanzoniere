using System.Text.RegularExpressions;
using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Services;
using Xunit.Abstractions;

namespace AppCanzoniere.Application.Tests.Import;

/// <summary>Shared property check: converted documents keep their lyrics, unchanged ones keep every lyric line.</summary>
internal static class DocumentProperties
{
    private static readonly Regex Chord = new(@"\[([^\]]+)\]", RegexOptions.CultureInvariant);

    public static void CheckFaithful(
        IEnumerable<GeneratedSheet> sheets, ITestOutputHelper output, int minimumConverted, Action<ChordSheetResult>? extra = null)
    {
        int converted = 0, unchanged = 0;
        foreach (var sheet in sheets)
        {
            var result = ChordSheetConverter.Convert(sheet.Text);
            if (result.WasChordPro)
            {
                unchanged++;
                AssertUnchangedKeepsLyrics(sheet, result.ChordPro);
                continue;
            }

            converted++;
            BracketSafetyTests.AssertParsedFaithfully(sheet, result.ChordPro);
            extra?.Invoke(result);
        }

        output.WriteLine($"{converted} documents converted, {unchanged} returned unchanged as ChordPro.");
        Assert.True(converted > minimumConverted, $"Only {converted} documents were converted.");
    }

    internal static void AssertUnchangedKeepsLyrics(GeneratedSheet sheet, string chordPro)
    {
        Assert.Equal(sheet.Text, chordPro);
        var parsed = new ChordProParser().Parse(chordPro);
        var texts = parsed.Lines
            .Where(l => l.Type is ChordProLineType.Verse or ChordProLineType.Chorus or ChordProLineType.Bridge)
            .Select(l => l.Text.Trim())
            .ToList();

        var index = 0;
        foreach (var line in sheet.Text.Split('\n').Select(l => l.TrimEnd('\r')))
        {
            var expected = Chord.Replace(line, string.Empty).Trim();
            if (line.Trim().Length == 0 || line[0] == '#' || expected.Length == 0 || expected.StartsWith('{'))
                continue;

            while (index < texts.Count && texts[index] != expected)
                index++;
            Assert.True(index < texts.Count, $"Lyric line '{expected}' lost from an unchanged document.");
            index++;
        }
    }
}
