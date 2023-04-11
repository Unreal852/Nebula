using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using CommunityToolkit.Mvvm.DependencyInjection;
using Nebula.Desktop.Services;
using Nebula.Desktop.Services.Logging;
using Serilog;

namespace Nebula.Desktop;

public static class Program
{
    private static readonly bool RegisterAvaloniaLogger = false;

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        ConfigureLogger();

        Log.Information("Initializing App");

        try
        {
            Ioc.Default.ConfigureServices(new ServiceProvider());

#if DEBUG
            if (RegisterAvaloniaLogger)
            {
                Trace.Listeners.Clear();
                Trace.Listeners.Add(Ioc.Default.GetService<AvaloniaLoggerService>()!);
            }
#endif

            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                Log.Fatal(e.Exception, "Task exception in {ObjectType}", sender?.GetType());
                Log.CloseAndFlush();
            };

            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Fatal error in main thread, {Message}", ex.Message);
            Log.Information("Exiting App");
            Log.CloseAndFlush();
            throw;
        }
        finally
        {
            Log.Information("Exiting App");
            Log.CloseAndFlush();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    private static AppBuilder BuildAvaloniaApp()
    {
        return AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .LogToTrace();
    }

    public static void ConfigureLogger()
    {
        const string outputTemplate
                = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{Context}] {Message:lj}{NewLine}{Exception}";
        string filePath = Path.Combine(AppContext.BaseDirectory, "logs", "logs_.log");
        Log.Logger = new LoggerConfiguration()
                    .Enrich
                        .WithProperty("Context", "*")
                    .WriteTo
                        .File(filePath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 5, outputTemplate: outputTemplate)
                    .CreateLogger();
    }
}