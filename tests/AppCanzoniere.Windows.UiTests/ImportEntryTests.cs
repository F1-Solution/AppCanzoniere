using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;

namespace AppCanzoniere.Windows.UiTests;

/// <summary>Reaches the import screen the way a user does: songs list, "+", "Da PDF", then Back to the list.</summary>
public class ImportEntryTests
{
    private static readonly TimeSpan Wait = TimeSpan.FromSeconds(60);

    private static AutomationElement? Poll(AppSession session, Func<AutomationElement?> find, TimeSpan? timeout = null)
    {
        var until = DateTime.UtcNow + (timeout ?? Wait);
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

    private static void Press(AutomationElement element)
    {
        if (element.Patterns.Invoke.IsSupported)
            element.Patterns.Invoke.Pattern.Invoke();
        else
            element.Click();
    }

    private static AutomationElement? ByIdOrName(AppSession session, string id, string name) =>
        session.Window.FindFirstDescendant(c => c.ByAutomationId(id)) ?? session.Window.FindFirstDescendant(c => c.ByName(name));

    [WindowsFact]
    public void SongsList_Plus_DaPdf_OpensImport_AndBackReturnsToTheList()
    {
        using var session = AppSession.LaunchPlain();

        var add = Poll(session, () => ByIdOrName(session, "AddSongButton", "Aggiungi canto"))
            ?? throw new InvalidOperationException("The \"+\" action was not found on the songs list.\n" + session.Dump());
        Press(add);

        var fromPdf = Poll(session, () => session.Window.FindFirstDescendant(c => c.ByName("Da PDF")), TimeSpan.FromSeconds(15))
            ?? throw new InvalidOperationException("\"Da PDF\" was not offered.\n" + session.Dump());
        Assert.NotNull(session.Window.FindFirstDescendant(c => c.ByName("Nuovo canto")));
        Press(fromPdf);

        Assert.NotNull(Poll(session, () => session.Window.FindFirstDescendant(c => c.ByAutomationId("ChoosePdf"))));
        Assert.Null(session.Window.FindFirstDescendant(c => c.ByName("Importa").And(c.ByControlType(ControlType.TabItem))));

        var back = Poll(session, () => session.Window.FindFirstDescendant(c => c.ByAutomationId("NavigationViewBackButton")))
            ?? throw new InvalidOperationException("Back button not found.\n" + session.Dump());
        Press(back);

        Assert.NotNull(Poll(session, () => ByIdOrName(session, "AddSongButton", "Aggiungi canto")));
        Assert.Null(session.Window.FindFirstDescendant(c => c.ByAutomationId("ChoosePdf")));
    }
}
