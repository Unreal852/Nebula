using System.Reflection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Nebula.Services.Abstractions;

namespace Nebula.Desktop.Services;

public sealed class AppService : IAppService
{
    private const string PreRelease = "-alpha";

    public string GetAppVersion()
    {
        var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
        return assemblyVersion is { }
                ? $"{assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Revision}{PreRelease}"
                : "0.0.0 (This should not happen)";
    }

    public void Shutdown()
    {
        if (Application.Current is { ApplicationLifetime: ClassicDesktopStyleApplicationLifetime appLife })
        {
            appLife.Shutdown();
        }
    }
}