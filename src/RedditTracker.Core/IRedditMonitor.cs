namespace RedditTracker.Core;

public interface IRedditMonitor:IDisposable {
    void StartMonitoring();
    void StopMonitoring();
}
