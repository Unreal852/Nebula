using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Nebula.Desktop.ViewModel;

namespace Nebula.Desktop.View;

public sealed partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<MainViewModel>();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}