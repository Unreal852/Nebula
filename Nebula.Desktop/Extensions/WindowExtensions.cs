using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Media;

namespace Nebula.Desktop.Extensions;

public static class WindowExtensions
{
    public static void TryEnableMicaEffect(this Window window, FluentAvaloniaTheme thm)
    {
        Color2? color = null;
        if (window.TryFindResource("SolidBackgroundFillColorBase", out object? resourceValue) && resourceValue != null)
            color = (Color)resourceValue;

        if (thm.RequestedTheme == FluentAvaloniaTheme.DarkModeString)
        {
            color ??= new Color2(42, 42, 42);
            color = color.Value.LightenPercent(-0.8f);
            window.Background = new ImmutableSolidColorBrush(color.Value, 0.78);
        }
        else if (thm.RequestedTheme == FluentAvaloniaTheme.LightModeString)
        {
            color ??= new Color2(243, 243, 243);
            color = color.Value.LightenPercent(0.5f);
            window.Background = new ImmutableSolidColorBrush(color.Value, 0.9);
        }
    }
}