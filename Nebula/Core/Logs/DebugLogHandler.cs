using System.Diagnostics;
using EasyLogger;
using EasyLogger.Format;
using EasyLogger.Format.Formatters;
using EasyLogger.Logging;

namespace Nebula.Core.Logs
{
    /// <summary>
    /// Provide a basic debug logger (<see cref="ILogHandler"/>) implementation.
    /// </summary>
    public class DebugLogHandler : ILogHandler
    {
        public DebugLogHandler() : this(new DefaultLogFormatter())
        {
            
        }

        public DebugLogHandler(ILogFormatter formatter)
        {
            Formatter = formatter;
        }
        
        /// <summary>
        /// Log Formatter
        /// </summary>
        public ILogFormatter Formatter { get; }

        public void Log(LogMessage message)
        {
            Debug.WriteLine(Formatter.Format(message));
        }
    }
}