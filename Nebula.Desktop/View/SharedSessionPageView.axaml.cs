using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Nebula.Desktop.View;

public sealed partial class SharedSessionPageView : UserControl
{
    public SharedSessionPageView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}