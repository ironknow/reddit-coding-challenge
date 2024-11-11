using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedditTracker.Core;

namespace RedditTracker;

public sealed class RedditPollingService :IHostedService, IDisposable {
    private readonly TrackerSettings _settings;
    private readonly ILogger<RedditPollingService> _logger;
    private readonly IRedditMonitor _monitor;

    public RedditPollingService(
        TrackerSettings settings,
        ILogger<RedditPollingService> logger,
        IRedditMonitor monitor
    ) {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _monitor = monitor;
    }

    public Task StartAsync(CancellationToken cancellationToken) {
        _logger.LogDebug("Starting Process");
        _logger.LogInformation($"Scanning {_settings.Subreddit} for changes...");
        _monitor.StartMonitoring();
        return Task.CompletedTask;
        
    }

    public Task StopAsync(CancellationToken cancellationToken) {
        _logger.LogDebug("Stopping Process");
        _monitor.StopMonitoring();
        return Task.CompletedTask;
    }


    public void Dispose() {
        _monitor?.Dispose();
    }
}
