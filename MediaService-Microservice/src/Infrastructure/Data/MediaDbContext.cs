using Domain.Entities;
using Domain.Entities.SqlEntites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Infrastructure.Data;

public class MediaDbContext
{
    private readonly IMongoDatabase _database;

    public MediaDbContext()
    {
    }
    public MediaDbContext(IConfiguration configuration)
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
    public IMongoCollection<UserImageAvatar> UserImageAvatars => _database.GetCollection<UserImageAvatar>("UserImageAvatar");
    public IMongoCollection<VideoFile> VideoFiles => _database.GetCollection<VideoFile>("VideoFile");
    public IMongoCollection<UserImageCertificate> UserImageCertificates => _database.GetCollection<UserImageCertificate>("UserImageCertificate");
    public IMongoCollection<SubjectImage> SubjectImages => _database.GetCollection<SubjectImage>("SubjectImage");
    public IMongoCollection<ExamContent> ExamContents => _database.GetCollection<ExamContent>("ExamContent");
    public IMongoCollection<DiscussionAttachment> DiscussionAttachments => _database.GetCollection<DiscussionAttachment>("DiscussionAttachment");
    public IMongoCollection<TheoryFile> TheoryFiles => _database.GetCollection<TheoryFile>("TheoryFile");
    public IMongoCollection<DocumentFile> DocumentFiles => _database.GetCollection<DocumentFile>("DocumentFile");
    public IMongoCollection<NewsFile> NewsFiles => _database.GetCollection<NewsFile>("NewsFile");
    public IMongoCollection<News> News => _database.GetCollection<News>("News");
    public IMongoCollection<NewsTag> NewsTags => _database.GetCollection<NewsTag>("NewsTag");
    public IMongoCollection<Domain.Entities.SqlEntites.Tag> Tags => _database.GetCollection<Domain.Entities.SqlEntites.Tag>("Tag");
}
