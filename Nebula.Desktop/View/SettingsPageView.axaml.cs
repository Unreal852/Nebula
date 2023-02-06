using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Nebula.Desktop.View;

public sealed partial class SettingsPageView : UserControl
{
    public SettingsPageView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}