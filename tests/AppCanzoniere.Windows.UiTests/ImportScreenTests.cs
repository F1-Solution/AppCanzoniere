using FlaUI.Core.AutomationElements;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;

namespace AppCanzoniere.Windows.UiTests;

/// <summary>Drives the import screen (route "import") on the built app with the committed fixtures in tests/fixtures/import.</summary>
public class ImportScreenTests
{
    private static readonly TimeSpan ReadTimeout = TimeSpan.FromSeconds(120);

    private sealed class Status(string text)
    {
        public string Text { get; } = text;

        public string this[string key] => Text.Split(';').Select(p => p.Split('=', 2)).FirstOrDefault(p => p[0] == key)?[1] ?? string.Empty;

        public int Number(string key) => int.TryParse(this[key], out var n) ? n : -1;

        public override string ToString() => Text;
    }

    private static AutomationElement? Find(AppSession session, string id) =>
        session.Window.FindFirstDescendant(c => c.ByAutomationId(id));

    private static AutomationElement Require(AppSession session, string id) =>
        Find(session, id) ?? throw new InvalidOperationException($"Control '{id}' not found.\n{session.Dump()}");

    private static Status ReadStatus(AppSession session) => new(Find(session, "ImportStatus")?.Name ?? string.Empty);

    private static Status WaitFor(AppSession session, Func<Status, bool> condition, string what, TimeSpan? timeout = null)
    {
        var until = DateTime.UtcNow + (timeout ?? ReadTimeout);
        var last = new Status(string.Empty);
        while (DateTime.UtcNow < until)
        {
            try
            {
                last = ReadStatus(session);
                if (condition(last))
                    return last;
            }
            catch (Exception ex) when (ex is System.Runtime.InteropServices.COMException or TimeoutException or InvalidOperationException)
            {
            }

            Thread.Sleep(300);
        }

        throw new TimeoutException($"Timed out waiting for {what}; last status '{last}'.\n{session.Dump()}");
    }

    private static void Press(AppSession session, string id)
    {
        var element = Require(session, id);
        if (element.Patterns.Invoke.IsSupported)
            element.Patterns.Invoke.Pattern.Invoke();
        else
            element.Click();
    }

    private static bool IsChecked(AppSession session, string id) => Require(session, id).Patterns.Toggle.Pattern.ToggleState.Value == FlaUI.Core.Definitions.ToggleState.On;

    private static void Tick(AppSession session, string id)
    {
        var element = Require(session, id);
        if (element.Patterns.Toggle.IsSupported)
            element.Patterns.Toggle.Pattern.Toggle();
        else
            element.Click();
    }

    private static bool Enabled(AppSession session, string id) => Require(session, id).Properties.IsEnabled.Value;

    private static void WaitUntil(Func<bool> condition, string what)
    {
        var until = DateTime.UtcNow + TimeSpan.FromSeconds(15);
        while (DateTime.UtcNow < until)
        {
            if (condition())
                return;
            Thread.Sleep(200);
        }

        throw new TimeoutException($"Timed out waiting for {what}.");
    }

    [WindowsFact]
    public void SingleSongPdf_GoesStraightToReview_AndSaveNeedsTheDeclaration()
    {
        using var session = AppSession.LaunchImport(AppSession.Fixture("single-song.pdf"));

        var status = WaitFor(session, s => s["phase"] == "Result" && s["reviewing"] == "True", "the single song to open in review");
        Assert.Equal(1, status.Number("songs"));
        Assert.Equal("Text", status["origins"]);

        Press(session, "SaveCurrent");
        WaitFor(session, s => s["declaration"] == "True", "the declaration sheet", TimeSpan.FromSeconds(15));
        Assert.False(IsChecked(session, "DeclarationCheck"), "the declaration must start unticked");
        Assert.False(Enabled(session, "ConfirmSave"), "Salva must be disabled while the declaration is unticked");

        Tick(session, "DeclarationCheck");
        WaitUntil(() => Enabled(session, "ConfirmSave"), "Salva to become enabled");

        Press(session, "ConfirmSave");
        var saved = WaitFor(session, s => s.Number("saved") == 1, "the song to be saved", TimeSpan.FromSeconds(30));
        Assert.Equal("False", saved["declaration"]);
    }

    [WindowsFact]
    public void MiniSongbook_ListsThreeSongs_SelectNoneBlocksSave_SaveAllSavesThree()
    {
        using var session = AppSession.LaunchImport(AppSession.Fixture("mini-songbook.pdf"));

        var status = WaitFor(session, s => s["phase"] == "Result", "the songbook to be read");
        Assert.Equal(3, status.Number("songs"));
        Assert.Equal(3, status.Number("selected"));
        Assert.Equal("False", status["reviewing"]);
        Assert.NotNull(Find(session, "SummaryFound"));
        Assert.NotNull(Find(session, "SongList"));

        Press(session, "SelectNone");
        WaitFor(session, s => s.Number("selected") == 0, "no song selected", TimeSpan.FromSeconds(15));
        Assert.False(Enabled(session, "SaveSelected"), "saving must be disabled with nothing selected");

        Press(session, "SelectAll");
        WaitFor(session, s => s.Number("selected") == 3, "all songs selected", TimeSpan.FromSeconds(15));
        Assert.True(Enabled(session, "SaveSelected"));

        Press(session, "SaveSelected");
        WaitFor(session, s => s["declaration"] == "True", "the declaration sheet", TimeSpan.FromSeconds(15));
        Assert.False(IsChecked(session, "DeclarationCheck"));
        Assert.False(Enabled(session, "ConfirmSave"));

        Tick(session, "DeclarationCheck");
        WaitUntil(() => Enabled(session, "ConfirmSave"), "Salva to become enabled");
        Press(session, "ConfirmSave");

        WaitFor(session, s => s.Number("saved") == 3, "three songs to be saved", TimeSpan.FromSeconds(30));
    }

    [WindowsFact]
    public void ScannedPage_IsReadWithOnDeviceOcr_AndGivesASongFromTheScan()
    {
        using var session = AppSession.LaunchImport(AppSession.Fixture("scanned-page.pdf"));

        var status = WaitFor(session, s => s["phase"] is "Result" or "Error", "the scanned page to be read");

        Assert.True(status["phase"] == "Result", $"OCR did not produce a result: {status}");
        Assert.True(status.Number("songs") >= 1, status.ToString());
        Assert.Contains("Scan", status["origins"]);
    }

    [WindowsFact]
    public void DamagedFile_ShowsTheErrorState_AndTheAppStaysAlive()
    {
        using var session = AppSession.LaunchImport(AppSession.Fixture("damaged.pdf"));

        var status = WaitFor(session, s => s["phase"] == "Error", "the error state");

        Assert.False(string.IsNullOrWhiteSpace(status["error"]));
        Assert.False(string.IsNullOrWhiteSpace(Require(session, "ErrorTitle").Name));
        using var process = System.Diagnostics.Process.GetProcessById(session.ProcessId);
        Assert.False(process.HasExited);
        Assert.NotNull(Find(session, "ErrorChoose") ?? Find(session, "ErrorRetry"));
    }

    [WindowsFact]
    public void CancelButton_DuringReading_ReturnsToStartWithNoList()
    {
        using var session = AppSession.LaunchImport(AppSession.Fixture("mini-songbook.pdf"), readDelayMs: 8000);

        WaitFor(session, s => s["phase"] == "Reading", "the reading state", TimeSpan.FromSeconds(60));
        Press(session, "CancelImport");

        var cancelled = WaitFor(session, s => s["phase"] == "Cancelled", "the cancelled state", TimeSpan.FromSeconds(15));
        Assert.Equal(0, cancelled.Number("songs"));

        // The delayed read ends after the cancel and must not bring a list back.
        Thread.Sleep(TimeSpan.FromSeconds(10));
        var later = ReadStatus(session);
        Assert.Equal("Cancelled", later["phase"]);
        Assert.Equal(0, later.Number("songs"));
        Assert.NotNull(Find(session, "CancelledMessage"));
    }

    [WindowsFact]
    public void Escape_DuringReading_CancelsTheImport()
    {
        using var session = AppSession.LaunchImport(AppSession.Fixture("mini-songbook.pdf"), readDelayMs: 20000);

        WaitFor(session, s => s["phase"] == "Reading", "the reading state", TimeSpan.FromSeconds(60));
        session.EnsureForeground();
        AppSession.Tap(VirtualKeyShort.ESCAPE);

        var cancelled = WaitFor(session, s => s["phase"] == "Cancelled", "Esc to cancel the reading", TimeSpan.FromSeconds(15));
        Assert.Equal(0, cancelled.Number("songs"));
    }

    [WindowsFact]
    public void CtrlS_InReview_OpensTheDeclaration_AndEscapeClosesItWithoutSaving()
    {
        using var session = AppSession.LaunchImport(AppSession.Fixture("single-song.pdf"));
        WaitFor(session, s => s["phase"] == "Result" && s["reviewing"] == "True", "the single song to open in review");
        session.EnsureForeground();

        Keyboard.TypeSimultaneously(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_S);
        WaitFor(session, s => s["declaration"] == "True", "Ctrl+S to open the declaration", TimeSpan.FromSeconds(15));
        Assert.False(IsChecked(session, "DeclarationCheck"));

        AppSession.Tap(VirtualKeyShort.ESCAPE);
        var closed = WaitFor(session, s => s["declaration"] == "False", "Esc to close the declaration", TimeSpan.FromSeconds(15));
        Assert.Equal(0, closed.Number("saved"));
        Assert.Equal("True", closed["reviewing"]);
    }

    [WindowsFact]
    public void RealClickOnTheDeclarationBox_TicksItOnce()
    {
        using var session = AppSession.LaunchImport(AppSession.Fixture("single-song.pdf"));
        WaitFor(session, s => s["phase"] == "Result" && s["reviewing"] == "True", "the single song to open in review");
        Press(session, "SaveCurrent");
        WaitFor(session, s => s["declaration"] == "True", "the declaration sheet", TimeSpan.FromSeconds(15));

        session.EnsureForeground();
        session.Window.SetForeground();
        Thread.Sleep(500);
        var rect = Require(session, "DeclarationCheck").BoundingRectangle;
        var glyph = new System.Drawing.Point(rect.X + 14, rect.Y + rect.Height / 2);
        Mouse.MoveTo(glyph);
        Mouse.Click(glyph);

        // A second toggle (from a gesture on the row) would untick it again, so it must stay ticked.
        Thread.Sleep(1000);

        WaitUntil(() => Enabled(session, "ConfirmSave"), "Salva to become enabled after one real click");
        Assert.True(IsChecked(session, "DeclarationCheck"));
    }

    [WindowsFact]
    public void DeclarationSheet_IsModal_PageBehindIsDisabled_AndTabStaysInsideIt()
    {
        using var session = AppSession.LaunchImport(AppSession.Fixture("mini-songbook.pdf"));
        WaitFor(session, s => s["phase"] == "Result", "the songbook to be read");
        session.EnsureForeground();
        session.Window.SetForeground();
        Thread.Sleep(500);
        Press(session, "SaveSelected");
        WaitFor(session, s => s["declaration"] == "True", "the declaration sheet", TimeSpan.FromSeconds(15));

        WaitUntil(() => !Enabled(session, "AnotherPdf") && !Enabled(session, "SelectNone"), "the page behind to be disabled");
        var inside = new[] { "DeclarationCheck", "DeclarationCancel", "ConfirmSave" };
        for (var i = 0; i < 8; i++)
        {
            AppSession.Tap(VirtualKeyShort.TAB);
            Thread.Sleep(200);
            var focused = session.Window.Automation.FocusedElement()?.Properties.AutomationId.ValueOrDefault;
            Assert.True(focused is null || inside.Contains(focused), $"Tab reached '{focused}' behind the sheet");
        }

        var status = ReadStatus(session);
        Assert.Equal("True", status["declaration"]);
        Assert.Equal(0, status.Number("saved"));
    }

    [WindowsFact]
    public void CancelButton_DuringARealRecognition_StopsAndLeavesNoList()
    {
        using var session = AppSession.LaunchImport(AppSession.Fixture("scanned-page.pdf"));

        WaitFor(session, s => s["phase"] == "Reading" && s["stage"] == "Recognizing", "the recognition to start", TimeSpan.FromSeconds(60));
        Press(session, "CancelImport");

        var cancelled = WaitFor(session, s => s["phase"] is "Cancelled" or "Result", "the cancelled state", TimeSpan.FromSeconds(15));
        Assert.True(cancelled["phase"] == "Cancelled", $"the recognition finished before the cancel: {cancelled}");
        Thread.Sleep(TimeSpan.FromSeconds(8));
        var later = ReadStatus(session);
        Assert.Equal("Cancelled", later["phase"]);
        Assert.Equal(0, later.Number("songs"));
        using var process = System.Diagnostics.Process.GetProcessById(session.ProcessId);
        Assert.False(process.HasExited);
    }

    private static void Back(AppSession session) => Press(session, "NavigationViewBackButton");

    private static AutomationElement? WaitElement(AppSession session, Func<AutomationElement?> find, TimeSpan? timeout = null)
    {
        var until = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(30));
        while (DateTime.UtcNow < until)
        {
            try
            {
                if (find() is { } found)
                    return found;
            }
            catch (Exception ex) when (ex is System.Runtime.InteropServices.COMException or TimeoutException or InvalidOperationException)
            {
            }

            Thread.Sleep(300);
        }

        return null;
    }

    [WindowsFact]
    public void Back_DuringReading_CancelsLeavesAndReEntryStartsClean()
    {
        using var session = AppSession.LaunchImport(AppSession.Fixture("mini-songbook.pdf"), readDelayMs: 20000);
        WaitFor(session, s => s["phase"] == "Reading", "the reading state", TimeSpan.FromSeconds(60));

        // The first Back cancels the reading in the page, the second leaves it.
        Back(session);
        WaitFor(session, s => s["phase"] == "Cancelled", "Back to cancel the reading", TimeSpan.FromSeconds(15));
        Back(session);
        Assert.NotNull(WaitElement(session, () => session.Window.FindFirstDescendant(c => c.ByAutomationId("AddSongButton"))));
        if (PowerRequests.CanRead)
            Assert.True(PowerRequests.WaitUntil(session.ProcessId, held: false), "the screen stays on after leaving the import");

        Press(session, "AddSongButton");
        Press(WaitElement(session, () => session.Window.FindFirstDescendant(c => c.ByName("Da PDF")))!);
        Assert.NotNull(WaitElement(session, () => Find(session, "ChoosePdf")));

        // Re-entry shows a clean start state: no reading runs behind the page.
        var status = WaitFor(session, s => s["phase"] == "Start", "the start state after re-entry", TimeSpan.FromSeconds(15));
        Thread.Sleep(2000);
        Assert.Equal("Start", ReadStatus(session)["phase"]);
        Assert.Equal(0, status.Number("songs"));
    }

    [WindowsFact]
    public void Back_WithReadButUnsavedSongs_AsksBeforeLeaving()
    {
        using var session = AppSession.LaunchImport(AppSession.Fixture("mini-songbook.pdf"));
        WaitFor(session, s => s["phase"] == "Result", "the songbook to be read");

        Back(session);
        _ = WaitElement(session, () => session.Window.FindFirstDescendant(c => c.ByName("Esci")))
            ?? throw new InvalidOperationException("No confirmation was asked." + session.Dump());
        Assert.NotNull(session.Window.FindFirstDescendant(c => c.ByName("Annulla")));

        Press(session.Window.FindFirstDescendant(c => c.ByName("Annulla"))!);
        Assert.Equal("Result", ReadStatus(session)["phase"]);

        Back(session);
        Press(WaitElement(session, () => session.Window.FindFirstDescendant(c => c.ByName("Esci")))!);
        Assert.NotNull(WaitElement(session, () => session.Window.FindFirstDescendant(c => c.ByAutomationId("AddSongButton"))));
    }

    private static void Press(AutomationElement element)
    {
        if (element.Patterns.Invoke.IsSupported)
            element.Patterns.Invoke.Pattern.Invoke();
        else
            element.Click();
    }
}
