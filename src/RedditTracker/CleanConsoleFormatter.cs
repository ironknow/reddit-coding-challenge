using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace RedditTracker;

public sealed class CleanConsoleFormatter:ConsoleFormatter {
    public CleanConsoleFormatter(IOptionsMonitor<ConsoleFormatterOptions> options) : base(nameof(CleanConsoleFormatter)) { }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter
    ) {
        textWriter.WriteLine(logEntry.State);
    }
}
