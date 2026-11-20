using FlaUI.Core.AutomationElements;
using FlaUI.Core.WindowsAPI;

namespace AppCanzoniere.Windows.UiTests;

/// <summary>Shares one launched app between the tests that only need a song screen already in use.</summary>
public sealed class SongAppFixture : IDisposable
{
    internal AppSession Session { get; } = AppSession.Launch("open");

    public void Dispose() => Session.Dispose();
}

public class SongScreenTests(SongAppFixture fixture) : IClassFixture<SongAppFixture>
{
    private AppSession Session => fixture.Session;

    // Back to the top with the window in front, ready to take keys.
    private AutomationElement Prepare()
    {
        var scroller = Session.WaitForScroller();
        Session.EnsureForeground();
        scroller.Patterns.Scroll.Pattern.SetScrollPercent(-1, 0);
        AppSession.SettledOffset(scroller);
        return scroller;
    }

    [WindowsFact]
    public void SongScreen_OpensOnASongAtLeastTwoScreensLong()
    {
        var scroller = Session.WaitForScroller();

        var content = AppSession.Viewport(scroller) / (scroller.Patterns.Scroll.Pattern.VerticalViewSize.Value / 100.0);
        Assert.True(content >= 2 * AppSession.Viewport(scroller), $"content {content} is shorter than two screens of {AppSession.Viewport(scroller)}");
    }

    [WindowsTheory]
    [InlineData(VirtualKeyShort.NEXT)]
    [InlineData(VirtualKeyShort.SPACE)]
    [InlineData(VirtualKeyShort.DOWN)]
    [InlineData(VirtualKeyShort.RIGHT)]
    public void ForwardKeys_MoveOneScreenMinusOneRow(VirtualKeyShort key)
    {
        var scroller = Prepare();
        var viewport = AppSession.Viewport(scroller);

        AppSession.Tap(key);

        var moved = AppSession.SettledOffset(scroller);
        Assert.InRange(moved, viewport * 0.55, viewport * 0.99);
    }

    [WindowsTheory]
    [InlineData(VirtualKeyShort.PRIOR)]
    [InlineData(VirtualKeyShort.UP)]
    [InlineData(VirtualKeyShort.LEFT)]
    public void BackKeys_MoveBackTheSameDistance(VirtualKeyShort key)
    {
        var scroller = Prepare();
        AppSession.Tap(VirtualKeyShort.NEXT);
        AppSession.SettledOffset(scroller);
        AppSession.Tap(VirtualKeyShort.NEXT);
        var far = AppSession.SettledOffset(scroller);

        AppSession.Tap(key);

        var back = AppSession.SettledOffset(scroller);
        var viewport = AppSession.Viewport(scroller);
        Assert.InRange(far - back, viewport * 0.55, viewport * 0.99);
    }

    [WindowsFact]
    public void HoldingAKey_TurnsOnlyOnePage()
    {
        var scroller = Prepare();
        var viewport = AppSession.Viewport(scroller);

        AppSession.Hold(VirtualKeyShort.NEXT, repeats: 12);

        var moved = AppSession.SettledOffset(scroller);
        Assert.InRange(moved, viewport * 0.55, viewport * 0.99);
    }
}
