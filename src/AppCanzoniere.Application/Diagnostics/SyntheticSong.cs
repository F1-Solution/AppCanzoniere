using System.Text;
using AppCanzoniere.Domain.Entities;
using AppCanzoniere.Domain.Enums;

namespace AppCanzoniere.Application.Diagnostics;

/// <summary>An invented, several-screens-long song that device tests open directly; it never comes from the corpus.</summary>
public static class SyntheticSong
{
    public const string Title = "Test Song Long";

    public const int VerseCount = 24;

    /// <summary>The synthetic song; a title can be given, for example to list it in a debug build.</summary>
    public static Song Create(string title = Title) => new()
    {
        Title = title,
        Artist = "Self Test",
        OriginalKey = "G",
        PrimaryFormat = ContentFormat.ChordPro,
        ChordProContent = ChordPro(title)
    };

    public static string ChordPro(string title = Title)
    {
        var builder = new StringBuilder();
        builder.Append("{title:").Append(title).Append("}\n{artist:Self Test}\n{key:G}\n");
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
