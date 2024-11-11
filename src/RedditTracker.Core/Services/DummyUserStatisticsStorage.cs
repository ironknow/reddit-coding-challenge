using RedditTracker.Core.Models;

namespace RedditTracker.Core.Services;

/// <summary>
/// This is a dummy class used to help demonstrate how we might store data for this application.
/// </summary>
public class DummyUserStatisticsStorage:IUserStatisticsStorage {
    private readonly ILogger<DummyUserStatisticsStorage> _logger;
    
    public DummyUserStatisticsStorage(ILogger<DummyUserStatisticsStorage> logger) {
        _logger = logger;
    }

    public void UpdateStatistics(IEnumerable<UserStatisticsRecord> stats) {
        // Because this is a dummy, we aren't actually going to store the data
        // we are just going to write a log entry about it.  This also satisfies
        // The need for the user to get periodic updates :)
        var isFirst = true;
        foreach (var stat in stats) {
            if (isFirst) { // this saves us some monkeying around with LINQ to get the first record of an unknown IEnumerable,
                           // which may have unexpected memory or performance impact
                _logger.LogInformation("User Statistics Have Arrived At {Date}!", stat.Timestamp);
                isFirst = false;
            }
            _logger.LogInformation("\t{Rank}. {Author} - {NumPosts}", stat.Rank, stat.Author, stat.NumPosts);
        }
    }
}
