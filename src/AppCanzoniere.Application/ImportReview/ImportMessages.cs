using System.Globalization;
using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.PdfImport;

namespace AppCanzoniere.Application.ImportReview;

/// <summary>Italian texts of the import screen: errors, warning labels, counts and durations.</summary>
public static class ImportMessages
{
    public const string DeclarationText =
        "Dichiaro di poter usare questi canti: sono di pubblico dominio, li ho scritti io, oppure ho il libretto originale o il permesso di chi ne ha i diritti.";

    public static ImportErrorInfo ErrorFor(SongbookImportFailure failure, ImportPlatform platform) => failure switch
    {
        SongbookImportFailure.ProtectedOrDamaged or SongbookImportFailure.OverLimits or SongbookImportFailure.TimedOut =>
            new("Non riesco a leggere questo PDF", "Il file è protetto da password, danneggiato o troppo grande.", "Scegli un altro PDF"),
        SongbookImportFailure.LowResolution =>
            new("La scansione è troppo sgranata", "Il testo è troppo piccolo per essere riconosciuto. Serve una scansione ad almeno 200 punti per pollice.", "Scegli un altro PDF"),
        SongbookImportFailure.NoText =>
            new("Il PDF non contiene testo leggibile", "Le pagine sono vuote, solo immagini non riconosciute, o ruotate.", "Scegli un altro PDF"),
        SongbookImportFailure.OcrUnavailable =>
            new("Il riconoscimento del testo non è disponibile",
                platform == ImportPlatform.Windows
                    ? "Installa la lingua italiana in Impostazioni > Ora e lingua > Lingua."
                    : "Il componente di riconoscimento non risponde. Riavvia l'app.",
                "Scegli un altro PDF"),
        _ => Unexpected
    };

    public const string LeaveTitle = "Uscire dall'import?";
    public const string ReplaceTitle = "Scegliere un altro PDF?";

    /// <summary>The question asked before leaving the import, or null when nothing is lost; a running reading is cancelled without asking.</summary>
    public static string? LeavePrompt(int unsavedRead) =>
        unsavedRead == 1 ? "C'è 1 canto letto e non salvato: uscire lo perde."
        : unsavedRead > 1 ? $"Ci sono {unsavedRead} canti letti e non salvati: uscire li perde."
        : null;

    /// <summary>The question asked before replacing the current reading with another PDF, or null when no read song is unsaved.</summary>
    public static string? ReplacePrompt(int unsavedRead) =>
        unsavedRead == 1 ? "C'è 1 canto letto e non salvato: andrà perso."
        : unsavedRead > 1 ? $"Ci sono {unsavedRead} canti letti e non salvati: andranno persi."
        : null;

    public static ImportErrorInfo NoSongs { get; } =
        new("Non ho trovato canti", "Il testo non sembra un canto con accordi.", "Scegli un altro PDF");

    public static ImportErrorInfo Unexpected { get; } =
        new("Qualcosa è andato storto", "Nessun canto è stato salvato.", "Riprova", CanRetry: true);

    /// <summary>The visible text of a warning, or null for the informative one that is not shown.</summary>
    public static string? WarningLabel(SheetWarning warning) => warning switch
    {
        SheetWarning.AmbiguousSingleToken => "Nota da sola su una riga: accordo o testo?",
        SheetWarning.AmbiguousNoteWords => "Riga di testo con parole simili ad accordi",
        SheetWarning.StrayWordOnChordLine => "Parola tenuta come nota su una riga di accordi",
        SheetWarning.OtherNotation => "Accordi in un'altra notazione, tenuti come testo",
        SheetWarning.ParenthesesRemoved => "Accordo tra parentesi riscritto senza",
        SheetWarning.SnappedColumn => "Accordo spostato all'inizio della parola",
        SheetWarning.ChordInsideWord => null,
        SheetWarning.TabExpanded => "Tabulazioni convertite in spazi",
        SheetWarning.ChorusRecallWithoutChorus => "Ritornello richiamato ma non scritto prima",
        SheetWarning.KeyAndCapo => "Trovati tonalità e capotasto: controlla la tonalità",
        SheetWarning.UnreadableMetadata => "Riga di intestazione tenuta come commento",
        SheetWarning.EscapedLyric => "Riga di testo che inizia con #",
        SheetWarning.BracketsReplaced => "Parentesi quadre o graffe sostituite",
        SheetWarning.ChordLineKeptAlone => "Riga di accordi tenuta separata",
        _ => "Da controllare"
    };

    /// <summary>The visible warning labels of a song, without repeats.</summary>
    public static IReadOnlyList<string> WarningLabels(ImportedSong song) =>
        song.Warnings.Select(WarningLabel).OfType<string>().Distinct().ToList();

    /// <summary>The badge text of a song, or null when there is nothing to check.</summary>
    public static string? Badge(ImportedSong song)
    {
        if (song.DoubtfulLineCount > 0)
            return song.DoubtfulLineCount == 1 ? "! 1 riga da controllare" : $"! {song.DoubtfulLineCount} righe da controllare";

        var labels = WarningLabels(song).Count;
        return labels == 0 ? null : labels == 1 ? "! 1 avviso" : $"! {labels} avvisi";
    }

    public static string PageRange(int first, int last) => last > first ? $"p. {first}–{last}" : $"p. {first}";

    public static string Songs(int count) => count == 1 ? "1 canto" : $"{count} canti";

    public static string Duration(TimeSpan time) =>
        string.Create(CultureInfo.InvariantCulture, $"{(int)time.TotalMinutes}:{time.Seconds:00}");

    public static string StageText(SongbookImportStage stage) => stage switch
    {
        SongbookImportStage.Recognizing => "Riconoscimento del testo (scansione)",
        SongbookImportStage.Converting => "Conversione dei canti",
        _ => "Lettura delle pagine di testo"
    };
}
