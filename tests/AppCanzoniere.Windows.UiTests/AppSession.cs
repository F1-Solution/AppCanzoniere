using System.Diagnostics;
using FlaApp = FlaUI.Core.Application;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;

namespace AppCanzoniere.Windows.UiTests;

/// <summary>One launched copy of the app, always killed (with its child processes) on dispose.</summary>
internal sealed class AppSession : IDisposable
{
    private const string SongVariable = "APPCANZONIERE_UITEST_SONG";
    private readonly FlaApp _app;
    private readonly UIA3Automation _automation;

    private AppSession(FlaApp app, UIA3Automation automation, Window window)
    {
        _app = app;
        _automation = automation;
        Window = window;
    }

    public Window Window { get; }

    public int ProcessId => _app.ProcessId;

    /// <summary>The Windows output of the app: APPCANZONIERE_APP_DIR, or the debug build next to the repository root.</summary>
    public static string AppDirectory
    {
        get
        {
            if (Environment.GetEnvironmentVariable("APPCANZONIERE_APP_DIR") is { Length: > 0 } fromEnvironment)
                return fromEnvironment;

            var directory = new DirectoryInfo(AppContext.BaseDirectory);
            while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "AppCanzoniere.Core.slnx")))
                directory = directory.Parent;

            return Path.Combine(
                directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found."),
                "src", "AppCanzoniere.MAUI", "bin", "Debug", "net10.0-windows10.0.19041.0", "win-x64");
        }
    }

    public static string Executable => Path.Combine(AppDirectory, "AppCanzoniere.MAUI.exe");

    /// <summary>Starts the app; a song mode ("open" or "edit") makes the debug build open the synthetic song.</summary>
    public static AppSession Launch(string? songMode = "open")
    {
        var info = new ProcessStartInfo(Executable) { WorkingDirectory = AppDirectory, UseShellExecute = false };
        if (songMode is not null)
            info.Environment[SongVariable] = songMode;

        return Start(info);
    }

    /// <summary>
    /// Starts the app on the import screen with a PDF read straight away (no file picker); the database lives in a throwaway
    /// folder so the tests leave the library alone, and a delay keeps the reading state on screen long enough to cancel it.
    /// </summary>
    public static AppSession LaunchImport(string pdfPath, int? readDelayMs = null)
    {
        var data = Path.Combine(Path.GetTempPath(), "appcanzoniere-uitest-" + Guid.NewGuid().ToString("N"));
        var info = new ProcessStartInfo(Executable) { WorkingDirectory = AppDirectory, UseShellExecute = false };
        info.Environment["APPCANZONIERE_UITEST_IMPORT"] = pdfPath;
        info.Environment["APPCANZONIERE_UITEST_DATA"] = data;
        info.Environment.Remove(SongVariable);
        if (readDelayMs is { } delay)
            info.Environment["APPCANZONIERE_UITEST_IMPORT_DELAY_MS"] = delay.ToString();

        return Start(info, data);
    }

    /// <summary>Starts the app on its first screen (the songs list) with no debug hook and a throwaway database.</summary>
    public static AppSession LaunchPlain()
    {
        var data = Path.Combine(Path.GetTempPath(), "appcanzoniere-uitest-" + Guid.NewGuid().ToString("N"));
        var info = new ProcessStartInfo(Executable) { WorkingDirectory = AppDirectory, UseShellExecute = false };
        info.Environment["APPCANZONIERE_UITEST_DATA"] = data;
        info.Environment.Remove(SongVariable);
        info.Environment.Remove("APPCANZONIERE_UITEST_IMPORT");
        return Start(info, data);
    }

    private static AppSession Start(ProcessStartInfo info, string? dataFolder = null)
    {
        var app = FlaApp.Launch(info);
        var automation = new UIA3Automation();
        try
        {
            var window = WaitForWindow(app, automation);
            return new AppSession(app, automation, window) { _dataFolder = dataFolder };
        }
        catch
        {
            Kill(app);
            automation.Dispose();
            throw;
        }
    }

    private string? _dataFolder;

    /// <summary>The committed import PDFs under tests/fixtures/import.</summary>
    public static string Fixture(string name)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "AppCanzoniere.Core.slnx")))
            directory = directory.Parent;

        var path = Path.Combine(directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found."), "tests", "fixtures", "import", name);
        return File.Exists(path) ? path : throw new FileNotFoundException("Import fixture missing.", path);
    }

    // The window exists before the app answers UI Automation calls, so a timed-out call is retried.
    private static Window WaitForWindow(FlaApp app, UIA3Automation automation)
    {
        var until = DateTime.UtcNow + TimeSpan.FromSeconds(120);
        Exception? last = null;
        while (DateTime.UtcNow < until)
        {
            try
            {
                if (app.GetMainWindow(automation, TimeSpan.FromSeconds(5)) is { } window)
                    return window;
            }
            catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or TimeoutException or System.Runtime.InteropServices.COMException)
            {
                last = ex;
            }

            Thread.Sleep(500);
        }

        throw new InvalidOperationException("The app window did not appear.", last);
    }

    /// <summary>The vertically scrollable element that holds the song, once the song screen is up.</summary>
    public AutomationElement WaitForScroller(TimeSpan? timeout = null)
    {
        var until = DateTime.UtcNow + (timeout ?? TimeSpan.FromSeconds(60));
        while (DateTime.UtcNow < until)
        {
            var found = FindScroller();
            if (found is not null)
                return found;
            Thread.Sleep(250);
        }

        throw new TimeoutException("The song screen never showed a vertical scroller.\n" + Dump());
    }

    public AutomationElement? FindScroller() =>
        Window.FindAllDescendants()
            .FirstOrDefault(e => e.Patterns.Scroll.IsSupported && e.Patterns.Scroll.PatternOrDefault is { VerticallyScrollable.ValueOrDefault: true });

    /// <summary>Current scroll offset in pixels, from the percentages the scroller reports.</summary>
    public static double Offset(AutomationElement scroller)
    {
        var pattern = scroller.Patterns.Scroll.Pattern;
        var viewport = scroller.BoundingRectangle.Height;
        var content = viewport / (pattern.VerticalViewSize.Value / 100.0);
        return Math.Max(0, pattern.VerticalScrollPercent.Value) / 100.0 * (content - viewport);
    }

    public static double Viewport(AutomationElement scroller) => scroller.BoundingRectangle.Height;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint pid);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hwnd);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern void keybd_event(byte key, byte scan, uint flags, UIntPtr extra);

    /// <summary>Brings the app to the front and fails before any key is sent when another program keeps the foreground.</summary>
    public void EnsureForeground()
    {
        var handle = Window.Properties.NativeWindowHandle.ValueOrDefault;
        for (var attempt = 0; attempt < 20; attempt++)
        {
            GetWindowThreadProcessId(GetForegroundWindow(), out var pid);
            if (pid == (uint)ProcessId)
                return;

            // A tap on Alt lets this process take the foreground from the program that holds it.
            keybd_event(0x12, 0, 0, UIntPtr.Zero);
            keybd_event(0x12, 0, 2, UIntPtr.Zero);
            SetForegroundWindow(handle);
            Thread.Sleep(250);
        }

        throw new InvalidOperationException("The app could not take the foreground; no key was sent.");
    }

    public static void Tap(VirtualKeyShort key)
    {
        Keyboard.Press(key);
        Keyboard.Release(key);
    }

    /// <summary>Holds a key down: the first press is followed by auto-repeat key-downs, then one release.</summary>
    public static void Hold(VirtualKeyShort key, int repeats)
    {
        for (var i = 0; i < repeats; i++)
        {
            Keyboard.Press(key);
            Thread.Sleep(30);
        }

        Keyboard.Release(key);
    }

    /// <summary>Polls until the offset stops changing, so a scroll animation does not skew a comparison.</summary>
    public static double SettledOffset(AutomationElement scroller)
    {
        var last = Offset(scroller);
        for (var i = 0; i < 20; i++)
        {
            Thread.Sleep(150);
            var now = Offset(scroller);
            if (Math.Abs(now - last) < 0.5)
                return now;
            last = now;
        }

        return last;
    }

    public string Dump()
    {
        var text = new System.Text.StringBuilder();
        foreach (var element in Window.FindAllDescendants())
        {
            text.AppendLine($"{element.Properties.ControlType.ValueOrDefault} name='{element.Properties.Name.ValueOrDefault}' id='{element.Properties.AutomationId.ValueOrDefault}' class='{element.Properties.ClassName.ValueOrDefault}' scroll={element.Patterns.Scroll.IsSupported}");
        }

        return text.ToString();
    }

    public void Dispose()
    {
        Kill(_app);
        _app.Dispose();
        _automation.Dispose();
        try
        {
            if (_dataFolder is not null && Directory.Exists(_dataFolder))
                Directory.Delete(_dataFolder, recursive: true);
        }
        catch (IOException)
        {
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private static void Kill(FlaApp app)
    {
        try
        {
            using var process = Process.GetProcessById(app.ProcessId);
            process.Kill(entireProcessTree: true);
            process.WaitForExit(10000);
        }
        catch (ArgumentException)
        {
        }
        catch (InvalidOperationException)
        {
        }
    }
}
