using System.ComponentModel;
using AppCanzoniere.Application.ImportReview;
using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.MAUI.ViewModels;

/// <summary>One line of the import list; a thin view over an import entry.</summary>
public sealed class ImportRow : INotifyPropertyChanged
{
    private readonly ImportSession _session;
    private readonly Action<ImportRow, bool> _select;

    public ImportRow(ImportEntry entry, ImportSession session, Action<ImportRow, bool> select)
    {
        Entry = entry;
        _session = session;
        _select = select;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ImportEntry Entry { get; }

    public string Title => Entry.Song.Title;

    public string PagesText => ImportMessages.PageRange(Entry.Song.FirstPage, Entry.Song.LastPage);

    public string? BadgeText => ImportMessages.Badge(Entry.Song);

    public bool HasBadge => BadgeText is not null;

    public bool IsScan => Entry.Song.Origin != SongOrigin.Text;

    public bool IsModified => Entry.IsModified && !Entry.IsSaved;

    public bool IsSaved => Entry.IsSaved;

    public bool CanSelect => !Entry.IsSaved;

    public bool IsActive => ReferenceEquals(_session.Current, Entry) && _session.IsReviewing;

    public bool ShowChevron => !Entry.IsSaved;

    public string StatusText => !Entry.IsSaved
        ? string.Empty
        : _session.TimerEnabled ? $"Salvato · {(Entry.WasOpened ? ImportMessages.Duration(Entry.ReviewTime) : "0:00, non rivisto")}" : "Salvato";

    public bool HasStatus => Entry.IsSaved;

    public bool IsSelected
    {
        get => Entry.IsSelected;
        set
        {
            if (value != Entry.IsSelected)
                _select(this, value);
        }
    }

    /// <summary>The row read as one sentence for a screen reader.</summary>
    public string Description
    {
        get
        {
            var parts = new List<string> { Title, Entry.Song.FirstPage == Entry.Song.LastPage ? $"pagina {Entry.Song.FirstPage}" : $"pagine {Entry.Song.FirstPage} e {Entry.Song.LastPage}" };
            if (IsScan) parts.Add("scansione");
            if (BadgeText is { } badge) parts.Add(badge.TrimStart('!', ' '));
            if (IsModified) parts.Add("modificato");
            parts.Add(Entry.IsSaved ? "salvato" : Entry.IsSelected ? "selezionato" : "non selezionato");
            return string.Join(", ", parts);
        }
    }

    public void Refresh() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
}

/// <summary>One line of the original text, with the marker for lines the engine was unsure about.</summary>
public sealed record OriginalLine(int Number, string Text, bool IsDoubtful)
{
    public string Marker => IsDoubtful ? "?" : string.Empty;
}
