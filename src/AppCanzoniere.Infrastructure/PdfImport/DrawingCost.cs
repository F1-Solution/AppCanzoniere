using UglyToad.PdfPig;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Tokens;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>Content bytes and an upper bound on letters; Unmeasurable when the content cannot be costed safely.</summary>
internal readonly record struct Cost(long Bytes, long Letters, bool Unmeasurable = false)
{
    public static readonly Cost Unknown = new(0, 0, Unmeasurable: true);

    public Cost Plus(Cost other) =>
        Unmeasurable || other.Unmeasurable ? Unknown : new(Saturate(Bytes, other.Bytes), Saturate(Letters, other.Letters));

    public Cost Times(long count) =>
        Unmeasurable ? Unknown : new(Multiply(Bytes, count), Multiply(Letters, count));

    public bool Exceeds(long bytes, long letters) => Unmeasurable || Bytes > bytes || Letters > letters;

    private static long Saturate(long a, long b) => a > long.MaxValue - b ? long.MaxValue : a + b;

    private static long Multiply(long a, long count) => count != 0 && a > long.MaxValue / count ? long.MaxValue : a * count;
}

/// <summary>
/// Costs what each page draws before PdfPig parses it: its own content plus every form it calls, once per call,
/// nested forms multiplied through. Forms are costed once per resource context; a form that ends up calling itself
/// (which PdfPig would follow until the stack overflows) or nests deeper than any real file is unmeasurable.
/// </summary>
internal sealed class DrawingCost(PdfDocument document, long maxBytes, long maxLetters)
{
    // Real songbooks nest forms two or three deep; PdfPig recurses once per level, so the depth must stay small.
    private const int MaxFormNesting = 32;

    private readonly Dictionary<IndirectReference, DrawnContent> _contents = new();
    private readonly Dictionary<(IndirectReference Form, string Name, string Context), Cost> _forms = new();
    private readonly HashSet<IndirectReference> _drawing = new();
    private readonly Dictionary<DictionaryToken, int> _ids = new(ReferenceEqualityComparer.Instance);

    private int Cap => (int)Math.Min(maxBytes, Array.MaxLength);

    public Cost Page(PageNode page)
    {
        var content = OwnContent(page.Dictionary);
        return content is null ? Cost.Unknown : WithCalls(content, page.Resources, depth: 0, caller: null);
    }

    private DrawnContent? OwnContent(DictionaryToken page)
    {
        var key = page.TryGet(NameToken.Contents, out var entry) && entry is IndirectReferenceToken reference ? reference.Data : (IndirectReference?)null;
        if (key is { } shared && _contents.TryGetValue(shared, out var known))
            return known;

        var decoded = PageContent.Decode(document, page, Cap);
        if (decoded.Status == DecodeStatus.OverCap)
            return new DrawnContent(long.MaxValue, 0, new Dictionary<string, int>());
        if (!decoded.IsDecoded)
            return null;

        var drawn = DrawnContent.Of(decoded.Bytes);
        if (key is { } newKey)
            _contents[newKey] = drawn;
        return drawn;
    }

    // The content's own cost plus each call times the cost of the XObject the name resolves to.
    private Cost WithCalls(DrawnContent content, IReadOnlyList<DictionaryToken> resources, int depth, Caller? caller)
    {
        var total = new Cost(content.Bytes, content.Letters);
        foreach (var (name, calls) in content.Calls)
        {
            if (total.Exceeds(maxBytes, maxLetters))
                return total;
            if (DropsCallToItself(caller, name) || Target(resources, name) is not { } target)
                continue;
            total = total.Plus(Form(target, name, resources, depth + 1).Times(calls));
        }

        return total;
    }

    private Cost Form(IndirectReference reference, string name, IReadOnlyList<DictionaryToken> resources, int depth)
    {
        if (PdfObjects.Resolve(document, new IndirectReferenceToken(reference)) is not StreamToken stream
            || PdfObjects.Get<NameToken>(document, stream.StreamDictionary, NameToken.Subtype) is not { } subtype
            || !subtype.Equals(NameToken.Form))
            return new Cost(0, 0);
        if (depth > MaxFormNesting || _drawing.Contains(reference))
            return Cost.Unknown;

        var own = PdfObjects.Get<DictionaryToken>(document, stream.StreamDictionary, NameToken.Resources);
        var context = own is null ? resources : resources.Append(own).ToArray();
        var key = (reference, name, ContextKey(context));
        if (_forms.TryGetValue(key, out var known))
            return known;

        var decoded = FilterChain.Decode(document, stream, Cap);
        if (!decoded.IsDecoded)
            return decoded.Status == DecodeStatus.OverCap ? new Cost(long.MaxValue, 0) : Cost.Unknown;

        _drawing.Add(reference);
        var cost = WithCalls(DrawnContent.Of(decoded.Bytes), context, depth, new Caller(name, DirectXObjects(own)));
        _drawing.Remove(reference);
        return _forms[key] = cost;
    }

    // PdfPig's resource stack: the nearest dictionary that names the XObject wins, the form's or page's own on top.
    private IndirectReference? Target(IReadOnlyList<DictionaryToken> resources, string name)
    {
        var key = NameToken.Create(name);
        for (var i = resources.Count - 1; i >= 0; i--)
        {
            if (PdfObjects.Get<DictionaryToken>(document, resources[i], NameToken.Xobject) is { } xobjects && xobjects.TryGet(key, out var entry))
                return entry is IndirectReferenceToken target ? target.Data : null;
        }

        return null;
    }

    // PdfPig drops a form's calls to the name it was called by when that name, in the form's own XObjects and in the
    // XObjects of what it points to, is the same reference; any other self call recurses and is caught as a cycle.
    private bool DropsCallToItself(Caller? caller, string name)
    {
        var key = NameToken.Create(name);
        if (caller is not { OwnXObjects: { } own } || caller.Name != name || !own.TryGet(key, out var entry))
            return false;
        var target = PdfObjects.Resolve(document, entry) as StreamToken;
        var targetOwn = target is null ? null : PdfObjects.Get<DictionaryToken>(document, target.StreamDictionary, NameToken.Resources);
        return DirectXObjects(targetOwn) is { } next && next.TryGet(key, out var again) && again.Equals(entry);
    }

    // PdfPig's self-call check reads the XObject dictionary only when it is written in place, not by reference.
    private static DictionaryToken? DirectXObjects(DictionaryToken? resources) =>
        resources is not null && resources.TryGet(NameToken.Xobject, out var entry) ? entry as DictionaryToken : null;

    private string ContextKey(IReadOnlyList<DictionaryToken> resources) =>
        string.Join(',', resources.Select(r => _ids.TryGetValue(r, out var id) ? id : _ids[r] = _ids.Count + 1));

    // The form being costed and the name it was called by.
    private sealed record Caller(string Name, DictionaryToken? OwnXObjects);
}
