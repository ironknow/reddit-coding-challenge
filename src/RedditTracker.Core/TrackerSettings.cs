namespace RedditTracker.Core;

public class TrackerSettings {
    public string Subreddit { get; set; } = string.Empty;

    public string AppId { get; set; } = string.Empty;
    
    public string AppSecret { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public int TopUserLeaderboardSize { get; set; } = 10;

    public int TopVotesLeaderboardSize { get; set; } = 10;
}
