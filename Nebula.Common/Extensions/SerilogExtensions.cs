using Serilog;

namespace Nebula.Common.Extensions;

public static class SerilogExtensions
{
    public static ILogger WithPrefix(this ILogger logger, string prefix)
    {
        return logger.ForContext("Context", prefix);
    }
}