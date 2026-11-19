using System.Diagnostics;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.WindowsAPI;

namespace AppCanzoniere.Windows.UiTests;

public class TextFieldAndScreenTests
{
    [WindowsFact]
    public void KeysTypedInATextField_DoNotTurnPages()
    {
        using var session = AppSession.Launch("edit");
        var scroller = session.WaitForScroller();
        var field = session.Window.FindAllDescendants(c => c.ByControlType(ControlType.Edit)).First();
        session.EnsureForeground();
        field.Focus();
        AppSession.SettledOffset(scroller);
        var before = AppSession.Offset(scroller);

        // Down and Page Down are left out: a text box passes them to the enclosing scroller by itself, with no page turn involved.
        foreach (var key in new[] { VirtualKeyShort.SPACE, VirtualKeyShort.RIGHT })
        {
            AppSession.Tap(key);
            Thread.Sleep(300);
        }

        Assert.Equal(before, AppSession.SettledOffset(scroller), 1);
    }

    [WindowsFact]
    public void DisplayIsRequested_WhileTheSongScreenIsOpen_AndReleasedWhenMinimizedOrLeft()
    {
        if (!PowerRequests.CanRead)
            return;

        using var session = AppSession.Launch("open");
        session.WaitForScroller();
        session.EnsureForeground();

        Assert.True(PowerRequests.DisplayHeldBy(session.ProcessId), "the display is not requested on the song screen");

        session.Window.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Minimized);
        Assert.True(PowerRequests.WaitUntil(session.ProcessId, held: false), "the display request stays while the window is minimized: " + PowerRequests.Read());

        session.Window.Patterns.Window.Pattern.SetWindowVisualState(WindowVisualState.Normal);
        Assert.True(PowerRequests.WaitUntil(session.ProcessId, held: true), "the display request did not return after restoring the window");

        session.Window.FindFirstDescendant(c => c.ByAutomationId("NavigationViewBackButton"))!.AsButton().Invoke();
        Assert.True(PowerRequests.WaitUntil(session.ProcessId, held: false), "the display request stays after leaving the song screen");
    }
}

/// <summary>Reads what keeps the display on from `powercfg /requests`, which needs an elevated process.</summary>
internal static class PowerRequests
{
    private static readonly Lazy<bool> Readable = new(() => Read() is not null);

    public static bool CanRead => Readable.Value;

    public static bool DisplayHeldBy(int processId)
    {
        var text = Read() ?? string.Empty;
        var display = text.Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault(section => section.TrimStart().StartsWith("DISPLAY:", StringComparison.Ordinal)) ?? string.Empty;
        using var process = Process.GetProcessById(processId);
        return display.Contains(process.ProcessName, StringComparison.OrdinalIgnoreCase);
    }

    public static bool WaitUntil(int processId, bool held)
    {
        var until = DateTime.UtcNow.AddSeconds(15);
        while (DateTime.UtcNow < until)
        {
            if (DisplayHeldBy(processId) == held)
                return true;
            Thread.Sleep(500);
        }

        return false;
    }

    public static string? Read()
    {
        try
        {
            var info = new ProcessStartInfo("powercfg", "/requests") { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
            using var process = Process.Start(info)!;
            var output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return process.ExitCode == 0 && output.Contains("DISPLAY:", StringComparison.Ordinal) ? output.Replace("\r\n", "\n") : null;
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return null;
        }
    }
}
