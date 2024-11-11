using RedditTracker.Core.Models;

namespace RedditTracker.Core.Services;

public class DummyUpvoteStatisticsStorage:IUpvoteStatisticsStorage {
    private readonly ILogger<DummyUpvoteStatisticsStorage> _logger;
    
    public DummyUpvoteStatisticsStorage(ILogger<DummyUpvoteStatisticsStorage> logger) {
        _logger = logger;
    }

    public void UpdateStatistics(IEnumerable<UpvoteStatisticsRecord> stats) {
        // Because this is a dummy, we aren't actually going to store the data
        // we are just going to write a log entry about it.  This also satisfies
        // The need for the user to get periodic updates :)
        var isFirst = true;
        foreach (var stat in stats) {
            if (isFirst) { // this saves us some monkeying around with LINQ to get the first record of an unknown IEnumerable,
                // which may have unexpected memory or performance impact
                _logger.LogInformation("Voting Statistics Have Arrived At {Date}!", stat.Timestamp);
                isFirst = false;
            }
            _logger.LogInformation("\t{Rank}. {Post} - {Upvotes} ({TotalVotes} Total)", stat.Rank, stat.PostId, stat.NetVotes, stat.Upvotes+stat.Downvotes);
        }
    }
}
