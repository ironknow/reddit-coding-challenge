using System.Net.Http.Headers;
using System.Text.Json;
using RedditListener;
using System.Threading;

await MainApp();

static async Task MainApp()
{
    // Instantiate config object
    Configuration Config = new Configuration();

    // Setup HTTP Client
    HttpClient Client = new HttpClient();
    Client = await RedditAPIUtility.SetupClient(Config, Client);

    // Get UNIX UTC Time at start of app
    DateTime StartTime = DateTime.UtcNow;
    double StartTimeUTC = ((DateTimeOffset)StartTime).ToUnixTimeSeconds();

    // Token to cancel when Escape is hit
    using var cts = new CancellationTokenSource();
    CancellationToken ct = cts.Token;

    Task periodicLoggingTask = Task.Run(async () =>
    {
        while (!ct.IsCancellationRequested)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"--- Periodic Summary at {DateTime.UtcNow} ---");
            Console.ResetColor();
            await Task.Delay(TimeSpan.FromMinutes(1), ct); // Log every minute
        }
    });

    try
    {
        do
        {
            while (!Console.KeyAvailable)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Process Started: " + DateTimeOffset.FromUnixTimeSeconds((long)StartTimeUTC));
                Console.WriteLine("Config loaded from: " + Config.SetFrom);
                Console.WriteLine("Press ESC to stop");
                Console.ResetColor();

                ParallelOptions ParallelOptions = new()
                {
                    MaxDegreeOfParallelism = Config.MaxDegreeOfParallelism
                };

                await Parallel.ForEachAsync(Config.SubRedditsToMonitor, ParallelOptions, async (Subreddit, ct) =>
                {
                    RedditReturnObject ReturnedData = new RedditReturnObject();
                    int ThreadSleep = 1000;
                    try
                    {
                        ReturnedData = await RedditAPIUtility.ProcessRedditPostsAsync(Client, StartTimeUTC, Subreddit, Config);
                        ThreadSleep = RedditAPIUtility.CalculateThreadSleep(ReturnedData.xRateLimitReset, ReturnedData.xRateLimitRemaining, Config.SubRedditsToMonitor.Length);

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"--- {Subreddit} Statistics ---");
                        Console.ResetColor();

                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("Top Posts:");
                        Console.ResetColor();

                        foreach (RedditPostSummary PostSummary in ReturnedData.PostSummaries)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine($"Title: {PostSummary.Title}");
                            Console.WriteLine($"Upvotes: {PostSummary.Upvotes}");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            Console.WriteLine($"Author: {PostSummary.Author}");
                            Console.WriteLine($"Created UTC: {DateTimeOffset.FromUnixTimeSeconds(PostSummary.CreatedUTC)}");
                            Console.WriteLine($"Permalink: {PostSummary.PermaLink}\n");
                            Console.ResetColor();
                        }

                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.WriteLine("Top Authors:");
                        Console.ResetColor();

                        foreach (var Entry in ReturnedData.AuthorCounts)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkCyan;
                            Console.WriteLine($"Author: {Entry.Key} | # Posts: {Entry.Value}");
                            Console.ResetColor();
                        }
                        Console.WriteLine("-----------------------");

                        await Task.Delay(ThreadSleep, ct); // Asynchronous delay instead of Thread.Sleep
                    }
                    catch (Exception ex)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error in subreddit {Subreddit}: {ex.Message}");
                        Console.ResetColor();
                    }
                });

                Console.Clear();
                await Task.Delay(1000, ct);
            }
        } while (Console.ReadKey(true).Key != ConsoleKey.Escape);
    }
    finally
    {
        cts.Cancel(); // Cancel periodic logging task
        await periodicLoggingTask; // Wait for periodic logging task to complete
    }
}
