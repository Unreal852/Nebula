using Serilog;

namespace Nebula.Common.Extensions;

public static class SerilogExtensions
{
    public static ILogger WithContext(this ILogger logger, string prefix)
    {
        return logger.ForContext("Context", prefix);
    }
}