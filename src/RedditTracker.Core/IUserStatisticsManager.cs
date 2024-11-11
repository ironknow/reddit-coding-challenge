namespace RedditTracker.Core;

public interface IUserStatisticsManager {
    void UpdateData(List<Post> posts);
}
