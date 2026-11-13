using AppCanzoniere.Application.Music;
using AppCanzoniere.Application.Services;

namespace AppCanzoniere.Application.Tests;

/// <summary>Shared helpers for the lossless ChordPro contract.</summary>
internal static class LosslessAssert
{
    private static readonly ChordProParser Parser = new();

    /// <summary>Final-newline rule: every rendered line, the last included, ends with one LF; empty input stays empty.</summary>
    public static string Expected(string source)
    {
        var lf = source.Replace("\r\n", "\n").Replace('\r', '\n');
        return lf.Length == 0 || lf.EndsWith('\n') ? lf : lf + "\n";
    }

    public static string RoundTrip(string source) => Parser.Render(Parser.Parse(source));

    public static string Render(string source, ChordProRenderOptions options) => Parser.Render(Parser.Parse(source), options);

    public static void AssertLossless(string source) => Assert.Equal(Expected(source), RoundTrip(source));
}
