using System;
using Avalonia.Controls;
using Avalonia.Input;
using CommunityToolkit.Mvvm.DependencyInjection;
using Nebula.Desktop.ViewModel;

namespace Nebula.Desktop.View;

public sealed partial class AudioPlayerView : UserControl
{
    public AudioPlayerView()
    {
        InitializeComponent();
        DataContext = Ioc.Default.GetRequiredService<AudioPlayerViewModel>();
    }


    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        // TODO: This is hacky
        if (sender is ProgressBar progressBar && DataContext is AudioPlayerViewModel viewModel)
        {
            double mousePos = e.GetPosition(progressBar).X;
            double ratio = mousePos / progressBar.Bounds.Width;
            double value = ratio * progressBar.Maximum;
            viewModel.SetPosition(TimeSpan.FromSeconds(value));
        }
    }
}