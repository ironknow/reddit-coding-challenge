using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Moq;
using Reddit.Controllers;
using RedditTracker.Core.Models;
using RedditTracker.Core.Services;

namespace RedditTracker.Core.Tests.Services;

[TestClass]
[TestSubject(typeof(UpvoteStatisticsManager))]
public class UpvoteStatisticsManagerTest {

    private Mock<ILogger<UpvoteStatisticsManager>> _mockLogger;
    private Mock<IUpvoteStatisticsStorage> _mockStorage;

    [TestInitialize]
    public void Initialize() {
        _mockLogger = new Mock<ILogger<UpvoteStatisticsManager>>();
        _mockStorage = new Mock<IUpvoteStatisticsStorage>();
    }

    [TestMethod]
    public void UpdateData_Informs_Of_Change_When_Data_Has_Changed() {
        // 1. Arrange
        var settings = new TrackerSettings();
        var testSubject = new UpvoteStatisticsManager(
            _mockLogger.Object,
            _mockStorage.Object,
            settings
            );
        
        // We want to ensure that the storage service gets called if the data has changed
        var expectStorage = _mockStorage.Setup(obj
            => obj.UpdateStatistics(
                It.IsAny<IEnumerable<UpvoteStatisticsRecord>>()));

        // Add seed data
        var seedData = new List<Post> {
            new Post(null, "mock", "mock_subreddit", upVotes: 10, permalink:"/r/mock_subreddit/mock_post")
        };
        testSubject.UpdateData(seedData);

        // Now set the expectation to be verifyable--that is to say, we want to 
        // check that it gets called AFTER this point
        expectStorage.Verifiable(Times.Exactly(1));

        // 2. Act
        var testData = new List<Post> {
            new (null, "mock", "mock_subreddit", upVotes: 11, permalink:"/r/mock_subreddit/mock_post")
        };
        testSubject.UpdateData(testData);

        // 3. Assert
        _mockStorage.VerifyAll();

    }

    [TestMethod]
    public void UpdateData_Informs_Of_Change_On_First_Load() {
        // 1. Arrange
        var settings = new TrackerSettings();
        var testSubject = new UpvoteStatisticsManager(
            _mockLogger.Object,
            _mockStorage.Object,
            settings
        );
        
        // We want to ensure that the storage service gets called if the data has changed
        var expectStorage = _mockStorage.Setup(obj
            => obj.UpdateStatistics(
                It.IsAny<IEnumerable<UpvoteStatisticsRecord>>()));

        // Now set the expectation to be verifyable--that is to say, we want to 
        // check that it gets called AFTER this point
        expectStorage.Verifiable(Times.Exactly(1));

        // 2. Act
        var testData = new List<Post> {
            new (null, "mock", "mock_subreddit", upVotes: 11, permalink:"/r/mock_subreddit/mock_post")
        };
        testSubject.UpdateData(testData);

        // 3. Assert
        _mockStorage.VerifyAll();

    }

    [TestMethod]
    public void UpdateData_Informs_Of_Change_On_Add_Brand_New_Entry() {
        // 1. Arrange
        var settings = new TrackerSettings();
        var testSubject = new UpvoteStatisticsManager(
            _mockLogger.Object,
            _mockStorage.Object,
            settings
        );
        
        // We want to ensure that the storage service gets called if the data has changed
        var expectStorage = _mockStorage.Setup(obj
            => obj.UpdateStatistics(
                It.IsAny<IEnumerable<UpvoteStatisticsRecord>>()));

        // Add seed data
        var seedData = new List<Post> {
            new (null, "mock", "mock_subreddit", upVotes: 10, permalink:"/r/mock_subreddit/mock_post")
        };
        testSubject.UpdateData(seedData);

        // Now set the expectation to be verifyable--that is to say, we want to 
        // check that it gets called AFTER this point
        expectStorage.Verifiable(Times.Exactly(1));

        // 2. Act
        var testData = new List<Post> {
            new (null, "mock", "now_for_something_completely_different", upVotes: 42, permalink:"/r/mock_subreddit/different_post")
        };
        testSubject.UpdateData(testData);

        // 3. Assert
        _mockStorage.VerifyAll();

    }
    
    [TestMethod]
    public void UpdateData_Does_Not_Update_If_Data_Is_Identical() {
        // 1. Arrange
        var settings = new TrackerSettings();
        var testSubject = new UpvoteStatisticsManager(
            _mockLogger.Object,
            _mockStorage.Object,
            settings
        );
        
        // We want to ensure that the storage service gets called if the data has changed
        var expectStorage = _mockStorage.Setup(obj
            => obj.UpdateStatistics(
                It.IsAny<IEnumerable<UpvoteStatisticsRecord>>()));

        // Add seed data
        var seedData = new List<Post> {
            new (null, "mock", "mock_subreddit", upVotes: 10, permalink:"/r/mock_subreddit/mock_post")
        };
        testSubject.UpdateData(seedData);

        // Make sure this never gets called
        expectStorage.Verifiable(Times.Exactly(0));

        // 2. Act
        var testData = new List<Post> {
            new (null, "mock", "mock_subreddit", upVotes: 10, permalink:"/r/mock_subreddit/mock_post")
        };
        testSubject.UpdateData(testData);

        // 3. Assert
        _mockStorage.VerifyAll();

    }
}
