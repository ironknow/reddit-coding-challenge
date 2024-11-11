namespace RedditTracker.Core;

public interface IRedditApiClient:IDisposable {
    List<Post> ListAndMonitorNewPosts(Action<List<Post>> callback);
    void StopMonitoringNewPosts();

    List<Post> ListAndMonitorHotPosts(Action<List<Post>> callback);

    void StopMonitoringHotPosts();
}
