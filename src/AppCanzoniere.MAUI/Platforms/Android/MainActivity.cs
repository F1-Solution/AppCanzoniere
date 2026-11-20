using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using AppCanzoniere.Application.PageTurning;

namespace AppCanzoniere.MAUI;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
#if DEBUG
        Ocr.OcrSelfTest.RunIfRequested(Intent?.GetStringExtra(Ocr.OcrSelfTest.ExtraName));
        SongSelfTest.RunIfRequested(Intent?.GetStringExtra(Ocr.OcrSelfTest.ExtraName));
#endif
    }

#if DEBUG
    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);
        Ocr.OcrSelfTest.RunIfRequested(intent?.GetStringExtra(Ocr.OcrSelfTest.ExtraName));
        SongSelfTest.RunIfRequested(intent?.GetStringExtra(Ocr.OcrSelfTest.ExtraName));
    }
#endif

    public override bool DispatchKeyEvent(KeyEvent? e)
    {
        try
        {
            if (e is not null && CurrentFocus is not EditText && NameOf(e.KeyCode) is { } name)
            {
                var source = IPlatformApplication.Current?.Services.GetService<IHardwareKeySource>();
                if (source is not null)
                {
                    var phase = e.Action == KeyEventActions.Up ? KeyPhase.Release : e.RepeatCount > 0 ? KeyPhase.Repeat : KeyPhase.Press;
                    if ((e.Action == KeyEventActions.Down || e.Action == KeyEventActions.Up) && source.OnKey(name, e.IsCtrlPressed, e.IsAltPressed, phase))
                        return true;
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
