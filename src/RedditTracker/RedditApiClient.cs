using Reddit.Controllers;
using Reddit.Controllers.EventArgs;
using RedditTracker.Core;

namespace RedditTracker;


public sealed class RedditApiClient:IRedditApiClient {
    private readonly Subreddit _subreddit;
    private readonly TrackerSettings _settings;
    private Action<List<Post>>? _newPostCallback;
    private Action<List<Post>>? _hotPostCallback;
    
    public RedditApiClient(Reddit.RedditClient apiClient, TrackerSettings settings) {
        _subreddit=apiClient.Subreddit(settings.Subreddit).About();
        _settings = settings;
    }

    public List<Post> ListAndMonitorNewPosts(Action<List<Post>> callback) {
        var result = _subreddit.Posts.GetNew();
        _newPostCallback = callback;
        _subreddit.Posts.NewUpdated += OnNewPostsUpdated!;
        _subreddit.Posts.MonitorNew();
        return result;
    }

    public void StopMonitoringNewPosts() {
        // Undo our bindings in reverse order
        _subreddit.Posts.MonitorNew();
        _subreddit.Posts.NewUpdated -= OnNewPostsUpdated!;
        _newPostCallback = null;
    }
    
    public List<Post> ListAndMonitorHotPosts(Action<List<Post>> callback) {
        var result = _subreddit.Posts.GetHot();
        _hotPostCallback = callback;
        _subreddit.Posts.HotUpdated += OnHotPostsUpdated!;
        _subreddit.Posts.MonitorHot();
        return result;
    }

    public void StopMonitoringHotPosts() {
        // Undo our bindings in reverse order
        _subreddit.Posts.MonitorHot();
        _subreddit.Posts.HotUpdated -= OnHotPostsUpdated!;
        _hotPostCallback = null;
    }

    private void OnNewPostsUpdated(object sender, PostsUpdateEventArgs e) {
        _newPostCallback?.Invoke(e.NewPosts);
    }
    
    private void OnHotPostsUpdated(object sender, PostsUpdateEventArgs e) {
        _hotPostCallback?.Invoke(e.NewPosts);
    }

    public void Dispose() {
        // Make sure we don't create a memory leak with our event bindings
        _subreddit.Posts.NewUpdated -= OnNewPostsUpdated!;
        _subreddit.Posts.HotUpdated -= OnHotPostsUpdated!;
    }
}
