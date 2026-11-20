using AppCanzoniere.Application.Import;
using AppCanzoniere.Application.ImportReview;
using AppCanzoniere.Application.PdfImport;
using AppCanzoniere.Application.Tests.PdfImport;

namespace AppCanzoniere.Application.Tests.ImportReview;

public sealed class ImportSessionTests
{
    private readonly ManualTime _time = new();

    private ImportSession NewSession(bool timer = false) => new(_time, ImportPlatform.Windows) { TimerEnabled = timer };

    private static ImportedSong Song(string title, int page, bool warn = false) => new(
        title, "{title: " + title + "}\n[Do]La", "Do\nLa", page, page,
        warn ? new[] { SheetWarning.AmbiguousSingleToken } : Array.Empty<SheetWarning>(),
        1, warn ? 1 : 0, SongOrigin.Text, warn ? new[] { 2 } : Array.Empty<int>());

    private static SongbookImportResult Ok(params ImportedSong[] songs) => new(
        SongbookImportStatus.Success, "", SongbookImportFailure.None, songs, 10, 10,
        Array.Empty<int>(), Array.Empty<UnreadPage>(), 0, Array.Empty<int>(), null);

    private ImportSession WithSongs(int count, bool timer = false)
    {
        var s = NewSession(timer);
        s.BeginReading("a.pdf");
        s.Complete(Ok(Enumerable.Range(1, count).Select(i => Song("Canto " + i, i, i == 2)).ToArray()));
        return s;
    }

    private static Task<Guid> Save(ImportEntry _, CancellationToken __) => Task.FromResult(Guid.NewGuid());

    [Fact]
    public void Starts_in_start_phase()
    {
        Assert.Equal(ImportPhase.Start, NewSession().Phase);
    }

    [Fact]
    public void Reading_then_many_songs_shows_list_with_all_selected()
    {
        var s = WithSongs(3);

        Assert.Equal(ImportPhase.Result, s.Phase);
        Assert.False(s.IsReviewing);
        Assert.Equal(3, s.SelectedCount);
    }

    [Fact]
    public void A_single_song_goes_straight_to_review()
    {
        var s = WithSongs(1);

        Assert.True(s.IsReviewing);
        Assert.Equal(0, s.CurrentIndex);
    }

    [Fact]
    public void Cancel_discards_everything()
    {
        var s = WithSongs(3);
        s.Cancel();

        Assert.Equal(ImportPhase.Cancelled, s.Phase);
        Assert.Empty(s.Entries);
    }

    [Theory]
    [InlineData(SongbookImportFailure.ProtectedOrDamaged, "Non riesco a leggere questo PDF")]
    [InlineData(SongbookImportFailure.LowResolution, "La scansione è troppo sgranata")]
    [InlineData(SongbookImportFailure.NoText, "Il PDF non contiene testo leggibile")]
    [InlineData(SongbookImportFailure.OcrUnavailable, "Il riconoscimento del testo non è disponibile")]
    public void Failures_become_error_with_italian_title(SongbookImportFailure failure, string title)
    {
        var s = NewSession();
        s.BeginReading("a.pdf");
        s.Complete(SongbookImportResult.Failed("x", failure));

        Assert.Equal(ImportPhase.Error, s.Phase);
        Assert.Equal(title, s.Error!.Title);
    }

    [Fact]
    public void No_songs_found_is_an_error()
    {
        var s = NewSession();
        s.BeginReading("a.pdf");
        s.Complete(Ok());

        Assert.Equal(ImportPhase.Error, s.Phase);
        Assert.Equal("Non ho trovato canti", s.Error!.Title);
    }

    [Fact]
    public void Unexpected_exception_becomes_retryable_error()
    {
        var s = NewSession();
        s.BeginReading("a.pdf");
        s.Fail(new IOException("boom"));

        Assert.Equal(ImportPhase.Error, s.Phase);
        Assert.True(s.Error!.CanRetry);
        Assert.DoesNotContain("boom", s.Error.Explanation);
    }

    [Fact]
    public void Selection_all_none_and_toggle()
    {
        var s = WithSongs(3);
        s.SelectNone();
        Assert.Equal(0, s.SelectedCount);
        s.SetSelected(1, true);
        Assert.Equal(1, s.SelectedCount);
        s.SelectAll();
        Assert.Equal(3, s.SelectedCount);
    }

    [Fact]
    public void Only_doubtful_filters_visible_entries()
    {
        var s = WithSongs(3);
        s.OnlyDoubtful = true;

        Assert.Single(s.VisibleEntries);
        Assert.Equal("Canto 2", s.VisibleEntries[0].Song.Title);
    }

    [Fact]
    public void Navigation_stays_in_range()
    {
        var s = WithSongs(3);
        s.Open(0);
        s.Previous();
        Assert.Equal(0, s.CurrentIndex);
        s.Next();
        s.Next();
        s.Next();
        Assert.Equal(2, s.CurrentIndex);
        s.CloseReview();
        Assert.False(s.IsReviewing);
    }

    [Fact]
    public void Edit_marks_modified_and_restore_reverts()
    {
        var s = WithSongs(3);
        s.Open(0);
        s.Edit("changed");

        Assert.True(s.Entries[0].IsModified);
        Assert.Equal(1, s.ModifiedUnsavedCount);

        s.Restore();
        Assert.False(s.Entries[0].IsModified);
        Assert.Equal(0, s.ModifiedUnsavedCount);
    }

    [Fact]
    public void Declaration_is_never_preticked_and_resets_on_each_open()
    {
        var s = WithSongs(3);
        s.RequestSaveSelected();
        Assert.True(s.DeclarationOpen);
        Assert.False(s.DeclarationAccepted);
        Assert.False(s.CanConfirmSave);

        s.SetDeclaration(true);
        Assert.True(s.CanConfirmSave);
        s.CancelDeclaration();

        s.RequestSaveSelected();
        Assert.False(s.DeclarationAccepted);
    }

    [Fact]
    public async Task Save_without_declaration_is_refused_and_saves_nothing()
    {
        var s = WithSongs(3);
        s.RequestSaveSelected();
        var calls = 0;

        var outcome = await s.SaveAsync((_, _) => { calls++; return Task.FromResult(Guid.NewGuid()); });

        Assert.Null(outcome);
        Assert.Equal(0, calls);
    }

    [Fact]
    public async Task Save_selected_marks_saved_and_sets_banner_and_resets_declaration()
    {
        var s = WithSongs(3);
        s.SetSelected(2, false);
        s.RequestSaveSelected();
        s.SetDeclaration(true);

        var outcome = await s.SaveAsync(Save);

        Assert.Equal(2, outcome!.Saved);
        Assert.True(s.Entries[0].IsSaved && s.Entries[1].IsSaved && !s.Entries[2].IsSaved);
        Assert.False(s.DeclarationOpen);
        Assert.False(s.DeclarationAccepted);
        Assert.Contains("2 canti salvati", s.Banner!.Text);
        Assert.Contains("1 canto", s.Banner.Text);
        Assert.Equal(0, s.SelectedCount);
    }

    [Fact]
    public async Task A_failing_song_does_not_stop_the_others()
    {
        var s = WithSongs(3);
        s.RequestSaveSelected();
        s.SetDeclaration(true);

        var outcome = await s.SaveAsync((e, _) => e.Index == 1 ? throw new IOException() : Task.FromResult(Guid.NewGuid()));

        Assert.Equal(2, outcome!.Saved);
        Assert.Equal(new[] { "Canto 2" }, outcome.FailedTitles);
        Assert.False(s.Entries[1].IsSaved);
        Assert.Contains("Canto 2", s.Banner!.Text);
    }

    [Fact]
    public async Task Saving_one_song_banner_offers_open_and_next()
    {
        var s = WithSongs(3);
        s.Open(0);
        s.RequestSaveCurrent();
        s.SetDeclaration(true);

        await s.SaveAsync(Save);

        Assert.Equal(0, s.Banner!.OpenIndex);
        Assert.True(s.Banner.CanGoNext);
        s.OpenNextUnsaved();
        Assert.Equal(1, s.CurrentIndex);
    }

    [Fact]
    public async Task Saved_songs_cannot_be_selected_or_saved_again()
    {
        var s = WithSongs(2);
        s.RequestSaveSelected();
        s.SetDeclaration(true);
        await s.SaveAsync(Save);

        s.SetSelected(0, true);

        Assert.False(s.Entries[0].IsSelected);
        s.RequestSaveSelected();
        Assert.False(s.DeclarationOpen);
    }

    [Fact]
    public void Progress_is_reported_with_text()
    {
        var s = NewSession();
        s.BeginReading("a.pdf");
        s.Report(new SongbookImportProgress(SongbookImportStage.Recognizing, 12, 48));

        Assert.Equal("Riconoscimento del testo (scansione)", s.ProgressPhaseText);
        Assert.Equal("Pagina 12 di 48", s.ProgressPageText);
        Assert.Equal(0.25, s.ProgressFraction, 3);
    }

    [Fact]
    public void Timer_counts_only_while_a_song_is_open_and_not_paused()
    {
        var s = WithSongs(3, timer: true);
        s.Open(0);
        _time.Advance(TimeSpan.FromSeconds(30));
        s.Next();
        _time.Advance(TimeSpan.FromSeconds(100));
        s.Previous();
        _time.Advance(TimeSpan.FromSeconds(10));

        Assert.Equal(TimeSpan.FromSeconds(40), s.ElapsedOfCurrent);
        s.PauseTimer();
        _time.Advance(TimeSpan.FromSeconds(500));
        Assert.Equal(TimeSpan.FromSeconds(40), s.ElapsedOfCurrent);
        s.ResumeTimer();
        _time.Advance(TimeSpan.FromSeconds(5));
        Assert.Equal(TimeSpan.FromSeconds(45), s.ElapsedOfCurrent);
        Assert.Equal(TimeSpan.FromSeconds(100), s.Entries[1].ReviewTime);
    }

    [Fact]
    public void Timer_disabled_counts_nothing()
    {
        var s = WithSongs(3);
        s.Open(0);
        _time.Advance(TimeSpan.FromSeconds(30));

        Assert.Equal(TimeSpan.Zero, s.ElapsedOfCurrent);
    }

    [Fact]
    public void Copy_times_lists_one_line_per_song()
    {
        var s = WithSongs(2, timer: true);
        s.Open(0);
        _time.Advance(TimeSpan.FromSeconds(75));
        s.CloseReview();

        var text = s.TimesTable();

        Assert.Contains("Canto 1", text);
        Assert.Contains("1:15", text);
        Assert.Contains("Windows", text);
    }

    [Fact]
    public void Reset_returns_to_start()
    {
        var s = WithSongs(2);
        s.Reset();

        Assert.Equal(ImportPhase.Start, s.Phase);
        Assert.Empty(s.Entries);
    }
}
