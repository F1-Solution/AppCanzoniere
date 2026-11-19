using FlaUI.Core.WindowsAPI;
using Xunit.Abstractions;

namespace AppCanzoniere.Windows.UiTests;

public class SongScreenFocusTests(ITestOutputHelper output)
{
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint pid);

    [WindowsFact]
    public void KeysSentRightAfterTheSongOpens_StillTurnThePage()
    {
        using var session = AppSession.Launch("open");
        var scroller = session.WaitForScroller();
        session.EnsureForeground();
        GetWindowThreadProcessId(GetForegroundWindow(), out var foregroundPid);
        var focusedElement = session.Window.Automation.FocusedElement();
        output.WriteLine($"foreground pid {foregroundPid} app pid {session.ProcessId}; focused element: {focusedElement?.Properties.ClassName.ValueOrDefault} / {focusedElement?.Properties.Name.ValueOrDefault}");

        // No click and no focus change: this is the pedal pressed as soon as the page shows.
        AppSession.Tap(VirtualKeyShort.NEXT);

        var moved = AppSession.SettledOffset(scroller);
        output.WriteLine($"first press moved to {moved}");
        if (moved <= 1)
        {
            Thread.Sleep(3000);
            AppSession.Tap(VirtualKeyShort.NEXT);
            output.WriteLine($"second press (after 3 s) moved to {AppSession.SettledOffset(scroller)}");
        }

        Assert.True(moved > AppSession.Viewport(scroller) * 0.55, $"the page did not turn (offset {moved})");
    }
}
