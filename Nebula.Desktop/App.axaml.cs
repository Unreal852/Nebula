using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.DependencyInjection;
using Nebula.Desktop.DataTemplates;
using Nebula.Net.Services.Server;
using Nebula.Services.Abstractions;

namespace Nebula.Desktop;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        // Fetch services to init them
        _ = Ioc.Default.GetRequiredService<ILanguageService>();
        _ = Ioc.Default.GetRequiredService<IThemeService>();
    }

    public override void OnFrameworkInitializationCompleted()
    {
        DataTemplates.Add(Ioc.Default.GetRequiredService<ViewLocator>());
        if(ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownRequested += DesktopOnShutdownRequested;
            desktop.MainWindow = new MainWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

    private async void DesktopOnShutdownRequested(object? sender, ShutdownRequestedEventArgs e)
    {
        Ioc.Default.GetService<IAudioService>()!.Shutdown();
        await Ioc.Default.GetService<INetServerService>()!.Stop();
    }
}