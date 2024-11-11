namespace RedditTracker.Core;

public interface IUpvoteStatisticsManager {
    void UpdateData(List<Post> posts);
}
