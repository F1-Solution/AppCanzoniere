using UglyToad.PdfPig.Tokens;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>A stack of resource dictionaries, one object per distinct stack, so pages and forms drawing from the same dictionaries share it.</summary>
internal sealed class ResourceChain
{
    private readonly ResourceChain? _below;
    private readonly DictionaryToken? _top;
    private readonly Dictionary<DictionaryToken, ResourceChain> _above = new(ReferenceEqualityComparer.Instance);

    private ResourceChain(ResourceChain? below, DictionaryToken? top) => (_below, _top, Depth) = (below, top, below is null ? 0 : below.Depth + 1);

    /// <summary>The number of dictionaries in the stack.</summary>
    public int Depth { get; }

    /// <summary>An empty stack, the root of every stack built on it.</summary>
    public static ResourceChain Empty() => new(null, null);

    /// <summary>This stack with the dictionary on top, the same object each time it is asked for.</summary>
    public ResourceChain With(DictionaryToken top)
    {
        if (!_above.TryGetValue(top, out var chain))
            _above[top] = chain = new ResourceChain(this, top);
        return chain;
    }

    /// <summary>The dictionaries from the top of the stack down.</summary>
    public IEnumerable<DictionaryToken> TopDown()
    {
        for (var chain = this; chain._top is not null; chain = chain._below!)
            yield return chain._top;
    }
}
