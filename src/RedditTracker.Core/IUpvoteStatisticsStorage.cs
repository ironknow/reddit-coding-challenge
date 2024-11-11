using RedditTracker.Core.Models;

namespace RedditTracker.Core;

public interface IUpvoteStatisticsStorage {
    void UpdateStatistics(IEnumerable<UpvoteStatisticsRecord> stats);
}
