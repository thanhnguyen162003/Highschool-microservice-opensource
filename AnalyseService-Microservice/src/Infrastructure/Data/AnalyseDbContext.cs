using Domain.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Infrastructure.Data;

public class AnalyseDbContext
{
    private readonly IMongoDatabase _database;

    public AnalyseDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDbConnection");
        var databaseName = configuration["MongoDbSettings:DatabaseName"];
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
