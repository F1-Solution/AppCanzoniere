using System.Collections.ObjectModel;

namespace AppCanzoniere.Application.Layout;

/// <summary>A read-only list over a private array; the type tells later copies that the list is already ours.</summary>
internal sealed class FrozenList<T> : ReadOnlyCollection<T>
{
    public FrozenList(T[] items)
        : base(items)
    {
    }
}

internal static class Frozen
{
    public static IReadOnlyList<T> From<T>(List<T> items) => new FrozenList<T>(items.ToArray());

    public static IReadOnlyList<T> From<T>(T[] items) => new FrozenList<T>(items);

    /// <summary>A private immutable copy of a caller's list; throws for a null list or a null element.</summary>
    public static IReadOnlyList<T> Copy<T>(IReadOnlyList<T>? items, string name)
    {
        ArgumentNullException.ThrowIfNull(items, name);
        if (items is FrozenList<T>)
            return items;

        var copy = new T[items.Count];
        for (var i = 0; i < copy.Length; i++)
            copy[i] = items[i] ?? throw new ArgumentException($"{name} holds a null element at {i}.", name);

        return new FrozenList<T>(copy);
    }
}
