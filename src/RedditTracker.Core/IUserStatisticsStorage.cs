using RedditTracker.Core.Models;

namespace RedditTracker.Core;

public interface IUserStatisticsStorage {

    void UpdateStatistics(IEnumerable<UserStatisticsRecord> stats);
}
