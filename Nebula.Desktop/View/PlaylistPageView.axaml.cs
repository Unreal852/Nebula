using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Nebula.Desktop.View;

public partial class PlaylistPageView : UserControl
{
    public PlaylistPageView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}