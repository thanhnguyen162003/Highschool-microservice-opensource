using Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Infrastructure.Data;

public class AnalyseDbContext
{
    private readonly IMongoDatabase _database;

    public AnalyseDbContext(IConfiguration configuration)
    {
        var username = configuration["MongoDbSettings:Username"];
        var password = configuration["MongoDbSettings:Password"];
        var host = configuration["MongoDbSettings:Host"];
        var databaseName = configuration["MongoDbSettings:DatabaseName"];

        // Construct connection string with proper escaping
        var connectionString = $"mongodb+srv://{Uri.EscapeDataString(username)}:{Uri.EscapeDataString(password)}@{host}/";

        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }

    public IMongoCollection<Edge> Edge => _database.GetCollection<Edge>("Edge");
    public IMongoCollection<Node> Node => _database.GetCollection<Node>("Node");
    public IMongoCollection<Roadmap> Roadmap => _database.GetCollection<Roadmap>("Roadmap");
    public IMongoCollection<UserAnalyseEntity> UserAnalyseEntity => _database.GetCollection<UserAnalyseEntity>("UserAnalyseEntity");
    public IMongoCollection<RecommendedData> RecommendedData => _database.GetCollection<RecommendedData>("RecommendedData");
    public IMongoCollection<RecentView> RecentViews => _database.GetCollection<RecentView>("RecentView");
    public IMongoCollection<DocumentDay> DocumentDay => _database.GetCollection<DocumentDay>("DocumentDay");
    public IMongoCollection<FlashcardAnalyticRecord> FlashcardAnalyticRecords => _database.GetCollection<FlashcardAnalyticRecord>("FlashcardAnalyticRecord");
    public IMongoCollection<SessionAnalyticRecord> SessionAnalyticRecords => _database.GetCollection<SessionAnalyticRecord>("SessionAnalyticRecord");
    public IMongoCollection<UserLearningPatternRecord> UserLearningPatternRecords => _database.GetCollection<UserLearningPatternRecord>("UserLearningPatternRecord");
    public IMongoCollection<UserActivityModel> UserActivityModel => _database.GetCollection<UserActivityModel>("UserActivityModel");
    public IMongoCollection<UserRetentionModel> UserRetentionModel => _database.GetCollection<UserRetentionModel>("UserRetentionModel");
    public IMongoCollection<UserFlashcardLearningModel> UserFlashcardLearningModel => _database.GetCollection<UserFlashcardLearningModel>("UserFlashcardLearningModel");
    public IMongoCollection<UserLessonLearningModel> UserLessonLearningModel => _database.GetCollection<UserLessonLearningModel>("UserLessonLearningModel");
    public async Task EnsureIndicesCreatedAsync()
    {
        // Create index for RecentViews collection
        await RecentViews.Indexes.CreateOneAsync(
            new CreateIndexModel<RecentView>(
                Builders<RecentView>.IndexKeys
                    .Ascending(rv => rv.UserId) // Index on UserId
                    .Descending(rv => rv.Time)  // Sort by Time descending
            )
        );

        // Add similar index creation logic for other collections if necessary
    }
}
