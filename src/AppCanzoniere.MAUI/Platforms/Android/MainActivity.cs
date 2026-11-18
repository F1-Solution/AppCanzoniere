using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AppCanzoniere.Application.PageTurning;

namespace AppCanzoniere.MAUI;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    public override bool DispatchKeyEvent(KeyEvent? e)
    {
        try
        {
            if (e is not null && CurrentFocus is not EditText && NameOf(e.KeyCode) is { } name)
            {
                var source = IPlatformApplication.Current?.Services.GetService<IHardwareKeySource>();
                if (source is not null)
                {
                    // Down presses turn the page; the matching Up is consumed too so the system never sees half a key.
                    if (e.Action == KeyEventActions.Down)
                    {
                        if (e.RepeatCount == 0 ? source.OnKey(name, e.IsCtrlPressed, e.IsAltPressed) : PageTurnKeys.Map(name, e.IsCtrlPressed, e.IsAltPressed) != PageTurnDirection.None)
                            return true;
                    }
                    else if (e.Action == KeyEventActions.Up && PageTurnKeys.Map(name, e.IsCtrlPressed, e.IsAltPressed) != PageTurnDirection.None && source.IsListening)
                    {
                        return true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Page-turn key handling failed: {ex}");
        }

        return base.DispatchKeyEvent(e);
    }

    private static string? NameOf(Keycode code) => code switch
    {
        Keycode.PageDown => "PageDown",
        Keycode.DpadDown => "ArrowDown",
        Keycode.DpadRight => "ArrowRight",
        Keycode.Space => "Space",
        Keycode.VolumeDown => "VolumeDown",
        Keycode.PageUp => "PageUp",
        Keycode.DpadUp => "ArrowUp",
        Keycode.DpadLeft => "ArrowLeft",
        Keycode.VolumeUp => "VolumeUp",
        _ => null
    };
}
