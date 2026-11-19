using System.Text;
using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Enums;

namespace AppCanzoniere.Application.Diagnostics;

/// <summary>An invented, several-screens-long song that device tests open directly; it never comes from the corpus.</summary>
public static class SyntheticSong
{
    public const string Title = "Test Song Long";

    public const int VerseCount = 24;

    public static Song Create() => new()
    {
        Title = Title,
        Artist = "Self Test",
        OriginalKey = "G",
        PrimaryFormat = ContentFormat.ChordPro,
        ChordProContent = ChordPro()
    };

    public static string ChordPro()
    {
        var builder = new StringBuilder();
        builder.Append("{title:").Append(Title).Append("}\n{artist:Self Test}\n{key:G}\n");
        for (var verse = 1; verse <= VerseCount; verse++)
        {
            builder.Append("\n{start_of_verse}\n");
            builder.Append($"[G]Verse {verse} line one of the [C]invented song\n");
            builder.Append($"[Em]Verse {verse} line two goes on and [D]on\n");
            builder.Append($"[G]Verse {verse} line three has a few [Am]more words\n");
            builder.Append($"[C]Verse {verse} line four closes [D]the verse [G]here\n");
            builder.Append("{end_of_verse}\n");
        }

        return builder.ToString();
    }
}
