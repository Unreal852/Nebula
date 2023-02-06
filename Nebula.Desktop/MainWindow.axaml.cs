using System;
using System.Runtime.InteropServices;
using Avalonia;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Windowing;
using Nebula.Desktop.Extensions;

namespace Nebula.Desktop;

public partial class MainWindow : AppWindow
{
    public static MainWindow Instance { get; private set; } = default!;

    public MainWindow()
    {
        Instance = this;
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        TitleBar.ExtendsContentIntoTitleBar = true;

        var thm = AvaloniaLocator.Current.GetRequiredService<FluentAvaloniaTheme>();
        UpdateTheme(thm, thm.RequestedTheme);
    }

    protected override void OnRequestedThemeChanged(FluentAvaloniaTheme sender, RequestedThemeChangedEventArgs args)
    {
        base.OnRequestedThemeChanged(sender, args);
        UpdateTheme(sender, args.NewTheme);
    }

    private void UpdateTheme(FluentAvaloniaTheme theme, string newTheme)
    {
        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if(IsWindows11 && newTheme != FluentAvaloniaTheme.HighContrastModeString)
                this.TryEnableMicaEffect(theme);
            else if(newTheme == FluentAvaloniaTheme.HighContrastModeString)
            {
                // Clear the local value here, and let the normal styles take over for HighContrast theme
                SetValue(BackgroundProperty, AvaloniaProperty.UnsetValue);
            }
        }
    }
}