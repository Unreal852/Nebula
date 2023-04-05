using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Nebula.Desktop.ViewModel;

namespace Nebula.Desktop.View;

public partial class PartyFlyoutView : UserControl
{
    public PartyFlyoutView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<PartyFlyoutViewModel>();
    }
}