using System.Text;
using System.Text.RegularExpressions;
using AppCanzoniere.Application.Interfaces;
using AppCanzoniere.Application.Music;

namespace AppCanzoniere.Application.Services;

/// <summary>Writes a parsed document back from its source lines, changing only what a requested transformation touches.</summary>
internal static class ChordProSourceRenderer
{
    public static string Render(IReadOnlyList<ChordProSourceLine> lines)
    {
        var sb = new StringBuilder();
        foreach (var line in lines)
            sb.Append(line.Raw).Append('\n');

        return sb.ToString();
    }
}
