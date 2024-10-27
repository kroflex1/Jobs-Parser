using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Parser.Services;

public class MongoDbService
{
    private readonly IMongoDatabase _database;

    public MongoDbService(IOptions<MongoDbSettings> mongoDbSettings, IMongoClient mongoClient)
    {
        _database = mongoClient.GetDatabase(mongoDbSettings.Value.DatabaseName);
    }
    
    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
}