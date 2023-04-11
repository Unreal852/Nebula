using System.Diagnostics;
using Serilog;

namespace Nebula.Desktop.Services.Logging;

public sealed class AvaloniaLoggerService : TraceListener
{
    private readonly ILogger _logger;

    public AvaloniaLoggerService(ILogger logger)
    {
        _logger = logger.WithContext("Avalonia");
    }

    public override void Write(string? message)
    {
        _logger.Information("{Message}", message);
    }

    public override void WriteLine(string? message)
    {
        _logger.Information("{Message}", message);
    }
}
