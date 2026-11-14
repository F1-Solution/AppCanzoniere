using System.Text.RegularExpressions;

namespace AppCanzoniere.Application.Services;

/// <summary>Makes a header value safe to write on one directive line.</summary>
internal static partial class HeaderText
{
    // Line feed, carriage return, next line, line separator and paragraph separator.
    [GeneratedRegex("[\\n\\r\\u0085\\u2028\\u2029]+")]
    private static partial Regex LineBreaks();

    /// <summary>The value with every run of line breaks replaced by one space, then trimmed.</summary>
    public static string Clean(string? value) => LineBreaks().Replace(value ?? string.Empty, " ").Trim();
}
