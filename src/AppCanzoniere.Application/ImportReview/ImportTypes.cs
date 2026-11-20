using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.ImportReview;

/// <summary>The main state of the import screen; the declaration sheet and the saved banner sit on top of Result.</summary>
public enum ImportPhase
{
    Start,
    Reading,
    Result,
    Error,
    Cancelled
}

/// <summary>The platform the import runs on, used for the platform-specific error texts and the times table.</summary>
public enum ImportPlatform
{
    Windows,
    Android
}

/// <summary>What one save action covers.</summary>
public enum SaveScope
{
    CurrentSong,
    Selected
}

/// <summary>An error shown to the user: what happened, why, and what to do.</summary>
public sealed record ImportErrorInfo(string Title, string Explanation, string Action, bool CanRetry = false);

/// <summary>The banner after a save; OpenIndex is the song to open, CanGoNext tells whether an unsaved song follows.</summary>
public sealed record ImportBanner(string Text, int? OpenIndex, bool CanGoNext);

/// <summary>How many songs a save wrote and the titles of those that failed.</summary>
public sealed record SaveOutcome(int Saved, IReadOnlyList<string> FailedTitles);

/// <summary>One song of the import session with the user's edits, selection and save state.</summary>
public sealed class ImportEntry
{
    internal ImportEntry(int index, ImportedSong song)
    {
        Index = index;
        Song = song;
        ChordPro = song.ChordPro;
        IsSelected = true;
    }

    public int Index { get; }

    /// <summary>The song as the engine proposed it.</summary>
    public ImportedSong Song { get; }

    /// <summary>The ChordPro as edited by the user.</summary>
    public string ChordPro { get; internal set; }

    public bool IsSelected { get; internal set; }

    public bool IsSaved { get; internal set; }

    public Guid? SavedSongId { get; internal set; }

    public bool WasOpened { get; internal set; }

    /// <summary>Time spent reviewing this song while the timer was on.</summary>
    public TimeSpan ReviewTime { get; internal set; }

    public bool IsModified => !string.Equals(ChordPro, Song.ChordPro, StringComparison.Ordinal);

    /// <summary>True when the engine left lines or warnings the user should look at.</summary>
    public bool NeedsReview => Song.DoubtfulLineCount > 0 || Song.Warnings.Any(w => ImportMessages.WarningLabel(w) is not null);
}
