namespace RedditTracker.Core.Models;

public sealed record UpvoteStatisticsRecord(
    string PostId,
    int Upvotes,
    int Downvotes,
    int Rank,
    DateTime Timestamp) {

    public int NetVotes => Upvotes - Downvotes;
}
