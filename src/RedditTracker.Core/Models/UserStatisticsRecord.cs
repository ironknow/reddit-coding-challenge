namespace RedditTracker.Core.Models;

public sealed record UserStatisticsRecord(
    string Author,
    int NumPosts,
    int Rank,
    DateTime Timestamp);
