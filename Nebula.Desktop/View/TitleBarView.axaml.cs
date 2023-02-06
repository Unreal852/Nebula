using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Nebula.Desktop.ViewModel;

namespace Nebula.Desktop.View;

public sealed partial class TitleBarView : UserControl
{
    public TitleBarView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<TitleBarViewModel>();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}