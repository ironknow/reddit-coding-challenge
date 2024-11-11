using RedditTracker.Core.Models;

namespace RedditTracker.Core.Services;

public sealed class UserStatisticsManager : IUserStatisticsManager {
    private readonly IUserStatisticsStorage _storage;
    private readonly ILogger<UserStatisticsManager> _logger;
    private readonly TrackerSettings _settings;

    private readonly IDictionary<string, ICollection<string>> _authorCounts =
        new Dictionary<string, ICollection<string>>();

    public UserStatisticsManager(
        ILogger<UserStatisticsManager> logger,
        IUserStatisticsStorage storage,
        TrackerSettings settings
    ) {
        _logger = logger;
        _storage = storage;
        _settings = settings;
    }


    public void UpdateData(List<Post> posts) {
        _logger.LogDebug("Processing incoming Data");
        var didChangeData = false;
        foreach (var post in posts) {
            var author = post.Author;
            var id = post.Id;

            // Add author to our tracked data, if not present
            if (!_authorCounts.ContainsKey(author)) {
                _authorCounts.Add(author, new List<string>());
                didChangeData = true;
            }


            // Add the ID of the post to the author, if not present
            var ids =
                _authorCounts
                    [author]; // Dev note, this prevents multiple keyed lookups!
            if (!ids.Contains(id)) {
                ids.Add(id);
                didChangeData = true;
            }
        }

        // Don't update our stats unless something changed
        if (didChangeData) {
            _logger.LogDebug("Updating statistics");
            RecomputeStatistics();
        }
    }

    private void RecomputeStatistics() {
        var topPosters = _authorCounts
            .Select(kv => new {
                Author = kv.Key, NumPosts = kv.Value.Count
            }) // Project into a simple type
            .OrderByDescending(i => i.NumPosts) // Top down
            .Take(_settings
                .TopUserLeaderboardSize) // Our configurable leaderboard size.
            .Enumerate((data, index) => new UserStatisticsRecord(data.Author,
                data.NumPosts, index, DateTime.UtcNow)) // Always use UTC
            .ToArray(); // We could technically just forward all this on.
        _storage.UpdateStatistics(topPosters);
    }
}
