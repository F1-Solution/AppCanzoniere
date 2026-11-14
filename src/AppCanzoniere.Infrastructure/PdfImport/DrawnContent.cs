using UglyToad.PdfPig;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Graphics;
using UglyToad.PdfPig.Graphics.Operations;
using UglyToad.PdfPig.Graphics.Operations.TextShowing;
using UglyToad.PdfPig.Logging;
using UglyToad.PdfPig.Parser;
using UglyToad.PdfPig.Tokens;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>
/// What one content stream draws, read with PdfPig's own content parser so that the forms it calls are the forms
/// PdfPig will draw: its size, an upper bound on its letters (one per string byte) and how often it calls each XObject.
/// </summary>
internal sealed record DrawnContent(long Bytes, long Letters, IReadOnlyDictionary<string, int> Calls)
{
    // PdfPig's default nesting limit for arrays and dictionaries in a content stream.
    private const int MaxParserDepth = 256;

    private static readonly ILog Log = new ParsingOptions().Logger;

    public static DrawnContent Of(ReadOnlyMemory<byte> content)
    {
        IReadOnlyList<IGraphicsStateOperation> operations;
        try
        {
            // A parser per call: its depth guard counts as it goes and must not be shared between threads.
            var parser = new PageContentParser(ReflectionGraphicsStateOperationFactory.Instance, new StackDepthGuard(MaxParserDepth), useLenientParsing: true);
            operations = parser.Parse(0, new MemoryInputBytes(content), Log);
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            // PdfPig parses the whole stream before drawing any of it, so a stream it cannot parse draws nothing.
            return new DrawnContent(content.Length, 0, new Dictionary<string, int>());
        }

        long letters = 0;
        var calls = new Dictionary<string, int>();
        foreach (var operation in operations)
        {
            letters += LettersOf(operation);
            if (operation is InvokeNamedXObject call)
                calls[call.Name.Data] = calls.GetValueOrDefault(call.Name.Data) + 1;
        }

        return new DrawnContent(content.Length, letters, calls);
    }

    // Every glyph takes at least one byte of its string, so the string length bounds the letters drawn.
    private static long LettersOf(IGraphicsStateOperation operation) => operation switch
    {
        ShowText show => Math.Max(show.Bytes.Length, show.Text?.Length ?? 0),
        MoveToNextLineShowText show => Math.Max(show.Bytes.Length, show.Text?.Length ?? 0),
        MoveToNextLineShowTextWithSpacing show => Math.Max(show.Bytes.Length, show.Text?.Length ?? 0),
        ShowTextsWithPositioning show => show.Array.Sum(StringLength),
        _ => 0,
    };

    private static long StringLength(IToken token) => token switch
    {
        StringToken text => Math.Max(text.Data.Length, text.GetBytes().Length),
        HexToken hex => hex.Bytes.Length,
        _ => 0,
    };
}
