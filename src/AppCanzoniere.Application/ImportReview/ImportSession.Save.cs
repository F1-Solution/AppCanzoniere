namespace AppCanzoniere.Application.ImportReview;

public sealed partial class ImportSession
{
    private SaveScope _scope;
    private List<ImportEntry> _frozen = new();

    public bool DeclarationOpen { get; private set; }

    /// <summary>The declaration box; false at every opening and never remembered.</summary>
    public bool DeclarationAccepted { get; private set; }

    public bool IsSaving { get; private set; }

    public int SaveDone { get; private set; }

    public int SaveTotal { get; private set; }

    public bool CanConfirmSave => DeclarationOpen && DeclarationAccepted && !IsSaving;

    /// <summary>How many songs the open declaration covers.</summary>
    public int DeclarationCount => DeclarationOpen || IsSaving ? _frozen.Count : TargetEntries(_scope).Count;

    /// <summary>True while the declaration sheet is open or a save runs: the covered songs must not change.</summary>
    public bool IsLocked => DeclarationOpen || IsSaving;

    public void RequestSaveCurrent() => OpenDeclaration(SaveScope.CurrentSong);

    public void RequestSaveSelected() => OpenDeclaration(SaveScope.Selected);

    public void SetDeclaration(bool accepted)
    {
        if (!DeclarationOpen || IsSaving)
            return;

        DeclarationAccepted = accepted;
        Raise();
    }

    public void CancelDeclaration()
    {
        if (IsSaving)
            return;

        DeclarationOpen = false;
        DeclarationAccepted = false;
        Raise();
    }

    public void DismissBanner()
    {
        Banner = null;
        Raise();
    }

    /// <summary>Saves the covered songs one by one; null when the declaration is not given. A failing song does not stop the rest.</summary>
    public async Task<SaveOutcome?> SaveAsync(Func<ImportEntry, CancellationToken, Task<Guid>> saveOne, CancellationToken cancellationToken = default)
    {
        if (!CanConfirmSave)
            return null;

        var targets = _frozen;
        _clock.Stop(Current);
        IsSaving = true;
        SaveDone = 0;
        SaveTotal = targets.Count;
        Raise();

        var failed = new List<string>();
        try
        {
            foreach (var entry in targets)
            {
                await SaveOneAsync(entry, saveOne, failed, cancellationToken);
                SaveDone++;
                Raise();
            }
        }
        catch (OperationCanceledException)
        {
            IsSaving = false;
            DeclarationOpen = false;
            DeclarationAccepted = false;
            Raise();
            throw;
        }

        var saved = targets.Count - failed.Count;
        IsSaving = false;
        DeclarationOpen = false;
        DeclarationAccepted = false;
        Banner = BuildBanner(targets, saved, failed);
        Raise();
        return new SaveOutcome(saved, failed);
    }

    private async Task SaveOneAsync(ImportEntry entry, Func<ImportEntry, CancellationToken, Task<Guid>> saveOne, List<string> failed, CancellationToken ct)
    {
        try
        {
            entry.SavedSongId = await saveOne(entry, ct);
            entry.IsSaved = true;
            entry.IsSelected = false;
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            failed.Add(entry.Song.Title);
        }
    }

    private void OpenDeclaration(SaveScope scope)
    {
        if (Phase != ImportPhase.Result || IsLocked)
            return;

        var targets = TargetEntries(scope);
        if (targets.Count == 0)
            return;

        _scope = scope;
        _frozen = targets;
        DeclarationOpen = true;
        DeclarationAccepted = false;
        Raise();
    }

    private List<ImportEntry> TargetEntries(SaveScope scope) => scope == SaveScope.CurrentSong
        ? Current is { IsSaved: false } c ? new List<ImportEntry> { c } : new List<ImportEntry>()
        : _entries.Where(e => e.IsSelected && !e.IsSaved).ToList();

    private ImportBanner BuildBanner(List<ImportEntry> targets, int saved, List<string> failed)
    {
        var failure = failed.Count > 0 ? $" Non salvati: {string.Join(", ", failed)}." : string.Empty;
        var open = targets.Count == 1 && saved == 1 ? targets[0].Index : (int?)null;
        var hasNext = _entries.Any(e => !e.IsSaved && e.Index > (open ?? -1));
        if (open is { } index)
            return new ImportBanner($"\"{targets[0].Song.Title}\" salvato.", index, hasNext && _scope == SaveScope.CurrentSong);

        var rest = _entries.Count(e => !e.IsSaved);
        var left = rest == 0
            ? string.Empty
            : rest == 1 ? $" {ImportMessages.Songs(rest)} resta da salvare." : $" {ImportMessages.Songs(rest)} restano da salvare.";
        var head = saved switch
        {
            0 => "Nessun canto salvato.",
            1 => "1 canto salvato nella libreria.",
            _ => $"{saved} canti salvati nella libreria."
        };
        return new ImportBanner(head + left + failure, null, false);
    }
}
