namespace RedditTracker.Core.Services;

public class RedditMonitor : IRedditMonitor {
    private readonly ILogger<RedditMonitor> _logger;
    private readonly IRedditApiClient _client;
    private readonly IUserStatisticsManager _userStatisticsManager;
    private readonly IUpvoteStatisticsManager _upvoteStatisticsManager;
    private bool _isRunning;

    public RedditMonitor(
        IRedditApiClient client,
        IUserStatisticsManager userStatisticsManager,
        IUpvoteStatisticsManager upvoteStatisticsManager,
        ILogger<RedditMonitor> logger
    ) {
        _client = client;
        _userStatisticsManager = userStatisticsManager;
        _upvoteStatisticsManager = upvoteStatisticsManager;
        _logger = logger;
    }

    public void StartMonitoring() {
        if (_isRunning) {
            throw new InvalidOperationException("Service is already running");
        }

        _isRunning = true;

        try {
            // Start the process of monitoring for new posts and building our statistics
            var newPosts = _client.ListAndMonitorNewPosts((posts)
                => _userStatisticsManager.UpdateData(posts));
            _userStatisticsManager.UpdateData(newPosts);

            // Start the process of monitoring hot posts
            var hotPosts = _client.ListAndMonitorHotPosts((posts)
                => _upvoteStatisticsManager.UpdateData(posts));
            _upvoteStatisticsManager.UpdateData(hotPosts);
        }
        catch (Exception exception) {
            // Something went wrong...
            // Log it
            _logger.LogCritical(exception, "Critical Exception Starting Monitor");
            
            // Shut down anything that may have been started
            StopMonitoring();
            
            // Flee!
            Environment.Exit(1);
        }
    }

    public void StopMonitoring() {
        if (!_isRunning) {
            throw new InvalidOperationException("Service is not running");
        }

        _isRunning = false;

        try {
            _client.StopMonitoringNewPosts();
            _client.StopMonitoringHotPosts();
        }
        catch (Exception exception) {
            // This is interesting, but not critical, because we are already shutting down
            _logger.LogError(exception, "Error During Shutdown");
        }
    }

    public void Dispose() {
        if (_isRunning) {
            StopMonitoring();
        }

        _client.Dispose();
    }
}
