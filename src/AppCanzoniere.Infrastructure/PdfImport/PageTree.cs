using UglyToad.PdfPig;
using UglyToad.PdfPig.Core;
using UglyToad.PdfPig.Tokens;

namespace AppCanzoniere.Infrastructure.PdfImport;

/// <summary>A leaf of the page tree and the resource dictionaries it can draw from: its own and its ancestors'.</summary>
internal sealed record PageNode(DictionaryToken Dictionary, ResourceChain Resources);

/// <summary>Walks the page tree with PdfPig 0.1.16's rules, so the pages found are the pages PdfPig will parse.</summary>
internal static class PageTree
{
    /// <summary>The pages in reading order, or null when the tree has more than maxNodes nodes.</summary>
    public static IReadOnlyList<PageNode>? Pages(PdfDocument document, int maxNodes)
    {
        var pages = new List<PageNode>();
        var catalog = document.Structure.Catalog.CatalogDictionary;
        if (PdfObjects.Get<DictionaryToken>(document, catalog, NameToken.Pages) is not { } root)
            return pages;

        var visited = new HashSet<IndirectReference>();
        if (catalog.TryGet(NameToken.Pages, out var rootEntry) && rootEntry is IndirectReferenceToken rootReference)
            visited.Add(rootReference.Data);
        var pending = new Stack<(DictionaryToken Node, ResourceChain Resources)>();
        pending.Push((root, ResourceChain.Empty()));
        var nodes = 0;
        while (pending.Count > 0)
        {
            if (++nodes > maxNodes)
                return null;
            var (node, inherited) = pending.Pop();
            var resources = WithOwn(document, node, inherited);
            if (IsPage(document, node))
            {
                pages.Add(new PageNode(node, resources));
                continue;
            }

            var kids = PdfObjects.Get<ArrayToken>(document, node, NameToken.Kids)?.Data ?? Array.Empty<IToken>();
            for (var i = kids.Count - 1; i >= 0; i--)
            {
                if (Kid(document, kids[i], visited) is { } kid)
                    pending.Push((kid, resources));
            }
        }

        return pages;
    }

    // A /Type decides, and without one a node with no kids array is a page (an unresolvable kid reads as an empty page).
    private static bool IsPage(PdfDocument document, DictionaryToken node) =>
        PdfObjects.Get<NameToken>(document, node, NameToken.Type) is { } type
            ? type.Equals(NameToken.Page)
            : PdfObjects.Get<ArrayToken>(document, node, NameToken.Kids) is null;

    // PdfPig reads each tree node once but each page as often as it is listed; a kid that is not a reference is skipped.
    private static DictionaryToken? Kid(PdfDocument document, IToken entry, HashSet<IndirectReference> visited)
    {
        if (entry is not IndirectReferenceToken reference)
            return null;
        var kid = PdfObjects.Resolve(document, reference) as DictionaryToken ?? new DictionaryToken(new Dictionary<NameToken, IToken>());
        return IsPage(document, kid) || visited.Add(reference.Data) ? kid : null;
    }

    private static ResourceChain WithOwn(PdfDocument document, DictionaryToken node, ResourceChain inherited) =>
        PdfObjects.Get<DictionaryToken>(document, node, NameToken.Resources) is { } own ? inherited.With(own) : inherited;
}
