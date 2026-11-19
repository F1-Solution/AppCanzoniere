using AppCanzoniere.Application.PageTurning;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.System;
using Windows.UI.Core;

namespace AppCanzoniere.MAUI.Platforms.Windows;

/// <summary>Forwards key presses of the window content to the page-turn key source, before any focused control sees them.</summary>
internal static class WindowKeyHook
{
    private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<UIElement, object> Attached = new();

    public static void Attach(object window)
    {
        if (window is not Microsoft.UI.Xaml.Window { Content: UIElement content } || Attached.TryGetValue(content, out _))
            return;

        Attached.Add(content, new object());
        content.PreviewKeyDown += OnPreviewKeyDown;
    }

    private static void OnPreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        try
        {
            if (sender is not UIElement element || IsTextInputFocused(element))
                return;

            var name = e.Key switch
            {
                VirtualKey.PageDown => "PageDown",
                VirtualKey.Down => "ArrowDown",
                VirtualKey.Right => "ArrowRight",
                VirtualKey.Space => "Space",
                VirtualKey.PageUp => "PageUp",
                VirtualKey.Up => "ArrowUp",
                VirtualKey.Left => "ArrowLeft",
                _ => null
            };
            if (name is null)
                return;

            var phase = e.KeyStatus.WasKeyDown ? KeyPhase.Repeat : KeyPhase.Press;
            var ctrl = IsDown(VirtualKey.Control);
            var alt = IsDown(VirtualKey.Menu);
            var source = IPlatformApplication.Current?.Services.GetService<IHardwareKeySource>();
            if (source is not null && source.OnKey(name, ctrl, alt, phase))
                e.Handled = true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Page-turn key handling failed: {ex}");
        }
    }

    private static bool IsDown(VirtualKey key) =>
        InputKeyboardSource.GetKeyStateForCurrentThread(key).HasFlag(CoreVirtualKeyStates.Down);

    private static bool IsTextInputFocused(UIElement element) =>
        FocusManager.GetFocusedElement(element.XamlRoot) is TextBox or PasswordBox or RichEditBox or AutoSuggestBox;
}
