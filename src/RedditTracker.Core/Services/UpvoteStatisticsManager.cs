using RedditTracker.Core.Models;

namespace RedditTracker.Core.Services;

public sealed class UpvoteStatisticsManager : IUpvoteStatisticsManager {
    private readonly IUpvoteStatisticsStorage _storage;
    private readonly ILogger<UpvoteStatisticsManager> _logger;
    private readonly TrackerSettings _settings;

    private readonly IDictionary<string, VotingRecord> _postVotes =
        new Dictionary<string, VotingRecord>();

    private class VotingRecord {
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
    }

    public UpvoteStatisticsManager(
        ILogger<UpvoteStatisticsManager> logger,
        IUpvoteStatisticsStorage storage,
        TrackerSettings settings
    ) {
        _logger = logger;
        _settings = settings;
        _storage = storage;
    }

    public void UpdateData(List<Post> posts) {
        _logger.LogDebug("Processing incoming Data");
        var didChangeData = false;
        foreach (var post in posts) {
            var id = post.Permalink;
            if (_postVotes.TryGetValue(id,out var record)) {
                if (record.UpVotes != post.UpVotes ||
                    record.DownVotes != post.DownVotes) {
                    record.UpVotes = post.UpVotes;
                    record.DownVotes = post.DownVotes;
                    didChangeData = true;
                }
            }
            else {
                record = new VotingRecord
                    { UpVotes = post.UpVotes, DownVotes = post.DownVotes };
                _postVotes.Add(post.Permalink, record);
                didChangeData = true;
            }
        }

        if (didChangeData) {
            _logger.LogDebug("Updating statistics");
            RecomputeStatistics();
        }
    }

    private void RecomputeStatistics() {
        var topPosts = _postVotes
            .Select(kv => new {
                PostId = kv.Key, 
                UpVotes = kv.Value.UpVotes,
                DownVotes= kv.Value.DownVotes
            }) // Project into a simple type
            // It is important to note that the requirements say we are interested in the posts
            // wit the most UPVOTES, not the net total highest vote
            .OrderByDescending(i => i.UpVotes) // Top down
            .Take(_settings
                .TopVotesLeaderboardSize) // Our configurable leaderboard size.
            .Enumerate((data, index) => new UpvoteStatisticsRecord(data.PostId,
                data.UpVotes, data.DownVotes, index+1, DateTime.UtcNow)) // Always use UTC
            .ToArray(); // We could technically just forward all this on.
        _storage.UpdateStatistics(topPosts);
    }
}
